// <copyright file="EditorScanner.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AgentHub.Services;

using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using AgentHub.Models;

/// <summary>
/// 扫描和解析 AI 编辑器配置的服务.
/// </summary>
public static class EditorScanner
{
    private static readonly string UserConfigDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        ".agenthub");

    private static readonly string UserConfigPath = Path.Combine(UserConfigDir, "config.json");

    /// <summary>
    /// Scans for all known and user-defined editors.
    /// </summary>
    /// <returns>List of detected editors.</returns>
    public static List<AiEditor> ScanAll()
    {
        var editors = new List<AiEditor>();
        var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        var knownEditors = new[]
        {
            (Name: "Claude Code", Icon: "🤖", Path: Path.Combine(homeDir, ".claude.json"), McpKey: "mcpServers"),
            (Name: "Cursor", Icon: "📝", Path: Path.Combine(homeDir, ".cursor", "mcp.json"), McpKey: "mcpServers"),
            (Name: "Windsurf", Icon: "🏄", Path: Path.Combine(homeDir, ".codeium", "windsurf", "mcp_config.json"), McpKey: "mcpServers"),
            (Name: "Opencode", Icon: "⌨️", Path: Path.Combine(homeDir, ".config", "opencode", "opencode.json"), McpKey: "mcp"),
            (Name: "Continue", Icon: "▶️", Path: Path.Combine(homeDir, ".continue", "config.json"), McpKey: "mcpServers"),
            (Name: "Trae", Icon: "🌲", Path: Path.Combine(homeDir, ".trae", "mcp.json"), McpKey: "mcpServers"),
            (Name: "Cline", Icon: "🔮", Path: Path.Combine(appDataDir, "Code", "User", "globalStorage", "saoudrizwan.claude-dev", "settings", "cline_mcp_settings.json"), McpKey: "mcpServers"),
            (Name: "Roo Code", Icon: "🦘", Path: Path.Combine(appDataDir, "Code", "User", "globalStorage", "rooveterinaryinc.roo-cline", "settings", "cline_mcp_settings.json"), McpKey: "mcpServers"),
        };

        foreach (var def in knownEditors)
        {
            if (!File.Exists(def.Path))
            {
                continue;
            }

            var editor = new AiEditor
            {
                Name = def.Name,
                Icon = def.Icon,
                ConfigPath = def.Path,
                IsDetected = true,
                McpKey = def.McpKey,
            };

            editor.ConfigFiles.Add(def.Path);

            // Editors with multiple config sources per official docs.
            if (def.Name == "Claude Code")
            {
                AddClaudeExtraConfigFiles(editor, homeDir);
            }
            else if (def.Name == "Opencode")
            {
                AddOpencodeExtraConfigFiles(editor);
            }

            LoadMcpServers(editor);
            LoadSkillUsage(editor);
            editors.Add(editor);
        }

        editors.AddRange(LoadUserEditors());
        return editors;
    }

    /// <summary>
    /// Loads MCP servers from all config files of an editor.
    /// </summary>
    /// <param name="editor">The editor to load servers for.</param>
    public static void LoadMcpServers(AiEditor editor)
    {
        editor.McpServers.Clear();

        // Load from all known config files.
        foreach (var configFile in editor.ConfigFiles)
        {
            LoadMcpFromFile(editor, configFile, editor.McpKey);
        }

        // Fallback: if no ConfigFiles, use ConfigPath directly.
        if (editor.ConfigFiles.Count == 0 && File.Exists(editor.ConfigPath))
        {
            LoadMcpFromFile(editor, editor.ConfigPath, editor.McpKey);
        }
    }

    /// <summary>
    /// Loads skill usage from an editor's config (Claude Code only).
    /// </summary>
    /// <param name="editor">The editor to load skill usage for.</param>
    public static void LoadSkillUsage(AiEditor editor)
    {
        if (!File.Exists(editor.ConfigPath))
        {
            return;
        }

        try
        {
            var json = JsonNode.Parse(File.ReadAllText(editor.ConfigPath));
            var skillUsage = json?["skillUsage"]?.AsObject();
            if (skillUsage == null)
            {
                return;
            }

            editor.SkillUsage.Clear();

            foreach (var skill in skillUsage.OrderByDescending(x => x.Value?["lastUsedAt"]?.GetValue<long>() ?? 0))
            {
                var usageCount = skill.Value?["usageCount"]?.GetValue<int>() ?? 0;
                var lastUsed = skill.Value?["lastUsedAt"]?.GetValue<long>() ?? 0;
                var lastUsedDate = lastUsed > 0
                    ? DateTimeOffset.FromUnixTimeMilliseconds(lastUsed).ToLocalTime().ToString("yyyy-MM-dd HH:mm")
                    : "Never";

                editor.SkillUsage.Add(new SkillUsage
                {
                    Name = skill.Key,
                    UsageCount = usageCount,
                    LastUsed = lastUsedDate,
                });
            }
        }
        catch (Exception)
        {
            // Skill usage section may not exist or be malformed.
        }
    }

    /// <summary>
    /// Saves a user-defined editor to config file.
    /// </summary>
    /// <param name="editor">The editor to save.</param>
    public static void SaveUserEditor(AiEditor editor)
    {
        Directory.CreateDirectory(UserConfigDir);

        JsonNode? root = null;
        if (File.Exists(UserConfigPath))
        {
            try
            {
                root = JsonNode.Parse(File.ReadAllText(UserConfigPath));
            }
            catch (Exception)
            {
                root = null;
            }
        }

        root ??= new JsonObject();

        var customEditors = root["customEditors"]?.AsArray() ?? new JsonArray();
        root["customEditors"] = customEditors;

        customEditors.Add(new JsonObject
        {
            ["name"] = editor.Name,
            ["icon"] = editor.Icon,
            ["configPath"] = editor.ConfigPath,
            ["mcpKey"] = editor.McpKey,
        });

        var options = new JsonSerializerOptions { WriteIndented = true, };
        File.WriteAllText(UserConfigPath, root.ToJsonString(options));
    }

    /// <summary>
    /// Removes a user-defined editor from config file.
    /// </summary>
    /// <param name="editorName">The editor name to remove.</param>
    public static void RemoveUserEditor(string editorName)
    {
        if (!File.Exists(UserConfigPath))
        {
            return;
        }

        try
        {
            var root = JsonNode.Parse(File.ReadAllText(UserConfigPath));
            var customEditors = root?["customEditors"]?.AsArray();
            if (customEditors == null)
            {
                return;
            }

            for (int i = customEditors.Count - 1; i >= 0; i--)
            {
                if (customEditors[i]?["name"]?.GetValue<string>() == editorName)
                {
                    customEditors.RemoveAt(i);
                }
            }

            var options = new JsonSerializerOptions { WriteIndented = true, };
            File.WriteAllText(UserConfigPath, root!.ToJsonString(options));
        }
        catch (Exception)
        {
            // Config file may be malformed.
        }
    }

    private static List<AiEditor> LoadUserEditors()
    {
        var editors = new List<AiEditor>();
        if (!File.Exists(UserConfigPath))
        {
            return editors;
        }

        try
        {
            var json = JsonNode.Parse(File.ReadAllText(UserConfigPath));
            var customEditors = json?["customEditors"]?.AsArray();
            if (customEditors == null)
            {
                return editors;
            }

            foreach (var item in customEditors)
            {
                var name = item?["name"]?.GetValue<string>() ?? string.Empty;
                var configPath = item?["configPath"]?.GetValue<string>() ?? string.Empty;
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(configPath))
                {
                    continue;
                }

                var editor = new AiEditor
                {
                    Name = name,
                    Icon = item?["icon"]?.GetValue<string>() ?? "🔧",
                    ConfigPath = configPath,
                    IsUserDefined = true,
                    McpKey = item?["mcpKey"]?.GetValue<string>() ?? "mcpServers",
                };

                LoadMcpServers(editor);
                LoadSkillUsage(editor);
                editors.Add(editor);
            }
        }
        catch (Exception)
        {
            // User config may be malformed.
        }

        return editors;
    }

    /// <summary>
    /// Adds Claude Code extra configuration file paths per official docs.
    /// See: https://code.claude.com/docs/en/settings.
    /// </summary>
    /// <param name="editor">The Claude Code editor.</param>
    /// <param name="homeDir">User home directory.</param>
    private static void AddClaudeExtraConfigFiles(AiEditor editor, string homeDir)
    {
        // Project-scoped: .mcp.json in current working directory.
        var projectMcp = Path.Combine(Directory.GetCurrentDirectory(), ".mcp.json");
        if (File.Exists(projectMcp))
        {
            editor.ConfigFiles.Add(projectMcp);
        }

        // Managed: C:\Program Files\ClaudeCode\managed-mcp.json (Windows).
        var managedMcp = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
            "ClaudeCode",
            "managed-mcp.json");
        if (File.Exists(managedMcp))
        {
            editor.ConfigFiles.Add(managedMcp);
        }
    }

    /// <summary>
    /// Adds Opencode extra configuration file paths per official docs.
    /// See: https://opencode.ai/docs/zh-cn/config/.
    /// </summary>
    /// <param name="editor">The Opencode editor.</param>
    private static void AddOpencodeExtraConfigFiles(AiEditor editor)
    {
        // Project-scoped: opencode.json in current working directory.
        var projectConfig = Path.Combine(Directory.GetCurrentDirectory(), "opencode.json");
        if (File.Exists(projectConfig))
        {
            editor.ConfigFiles.Add(projectConfig);
        }
    }

    /// <summary>
    /// Loads MCP servers from a single config file and adds them to the editor.
    /// </summary>
    /// <param name="editor">The editor.</param>
    /// <param name="configFile">The config file path.</param>
    /// <param name="mcpKey">The JSON key for the MCP section.</param>
    private static void LoadMcpFromFile(AiEditor editor, string configFile, string mcpKey)
    {
        if (!File.Exists(configFile))
        {
            return;
        }

        try
        {
            var json = JsonNode.Parse(File.ReadAllText(configFile));
            var mcpObj = json?[mcpKey]?.AsObject();
            if (mcpObj == null)
            {
                return;
            }

            foreach (var server in mcpObj)
            {
                var info = server.Value?.AsObject();
                if (info == null)
                {
                    continue;
                }

                // Skip if already loaded from a higher-priority config.
                if (editor.McpServers.Any(s => s.Name == server.Key))
                {
                    continue;
                }

                editor.McpServers.Add(new McpServer
                {
                    Name = server.Key,
                    Type = info["type"]?.GetValue<string>() ?? "unknown",
                    CommandOrUrl = ParseCommandOrUrl(info),
                    Enabled = ParseEnabled(info),
                    SourcePath = configFile,
                });
            }
        }
        catch (Exception)
        {
            // Config file may be malformed; skip silently.
        }
    }

    private static string ParseCommandOrUrl(JsonObject info)
    {
        var cmdNode = info["command"];
        if (cmdNode != null)
        {
            // String command (Claude/Cursor/Windsurf format).
            if (cmdNode is JsonValue)
            {
                var cmd = cmdNode.GetValue<string>();
                var args = info["args"]?.AsArray();
                return args != null && args.Count > 0
                    ? cmd + " " + string.Join(" ", args.Select(a => a?.ToString() ?? string.Empty))
                    : cmd;
            }

            // Array command (Opencode format).
            if (cmdNode is JsonArray cmdArray && cmdArray.Count > 0)
            {
                return string.Join(" ", cmdArray.Select(c => c?.ToString() ?? string.Empty));
            }
        }

        return info["url"]?.GetValue<string>() ?? string.Empty;
    }

    private static bool ParseEnabled(JsonObject info)
    {
        // "enabled" field (Opencode).
        if (info["enabled"] is JsonValue enabledVal)
        {
            return enabledVal.GetValue<bool>();
        }

        // "disabled" field (Cline/Roo Code, inverted).
        if (info["disabled"] is JsonValue disabledVal)
        {
            return !disabledVal.GetValue<bool>();
        }

        return true;
    }
}

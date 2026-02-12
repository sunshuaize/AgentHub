// <copyright file="AiEditor.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AgentHub.Models;

using System.Collections.ObjectModel;

/// <summary>
/// 表示一个 AI 编辑器/助手.
/// </summary>
public class AiEditor
{
    /// <summary>
    /// Gets or sets the editor name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the editor icon emoji.
    /// </summary>
    public string Icon { get; set; } = "🤖";

    /// <summary>
    /// Gets or sets the primary config file path.
    /// </summary>
    /// <value>主配置文件路径.</value>
    public string ConfigPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets the list of all config file paths scanned for this editor.
    /// </summary>
    /// <value>已扫描的全部配置文件路径列表.</value>
    public List<string> ConfigFiles { get; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether the editor was auto-detected.
    /// </summary>
    /// <value>值为 true 表示自动检测到；否则为 false.</value>
    public bool IsDetected { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the editor is user-defined.
    /// </summary>
    /// <value>值为 true 表示用户自定义；否则为 false.</value>
    public bool IsUserDefined { get; set; }

    /// <summary>
    /// Gets or sets the JSON key for MCP servers section.
    /// </summary>
    /// <value>MCP 服务器的 JSON 键名.</value>
    public string McpKey { get; set; } = "mcpServers";

    /// <summary>
    /// Gets the MCP servers collection.
    /// </summary>
    /// <value>MCP 服务器集合.</value>
    public ObservableCollection<McpServer> McpServers { get; } = new();

    /// <summary>
    /// Gets the skill usage collection.
    /// </summary>
    /// <value>技能使用统计集合.</value>
    public ObservableCollection<SkillUsage> SkillUsage { get; } = new();

    /// <summary>
    /// Gets the display name with icon.
    /// </summary>
    /// <value>带图标的显示名称.</value>
    public string DisplayName => $"{this.Icon} {this.Name}";

    /// <summary>
    /// Gets the status text.
    /// </summary>
    /// <value>状态描述文本.</value>
    public string StatusText => this.IsDetected ? "自动检测" : "用户添加";
}

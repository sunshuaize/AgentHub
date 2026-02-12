// <copyright file="MainViewModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AgentHub.ViewModels;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using AgentHub.Models;
using AgentHub.Services;

/// <summary>
/// Main view model responsible for loading and managing editors and skills data.
/// </summary>
public class MainViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    public MainViewModel()
    {
        this.LoadData();
    }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets the detected AI editors collection.
    /// </summary>
    public ObservableCollection<AiEditor> Editors { get; } = new();

    /// <summary>
    /// Gets the available skills collection.
    /// </summary>
    public ObservableCollection<Skill> AvailableSkills { get; } = new();

    /// <summary>
    /// Adds a user-defined editor and persists it.
    /// </summary>
    /// <param name="editor">The editor to add.</param>
    public void AddEditor(AiEditor editor)
    {
        EditorScanner.LoadMcpServers(editor);
        EditorScanner.LoadSkillUsage(editor);
        EditorScanner.SaveUserEditor(editor);
        this.Editors.Add(editor);
    }

    /// <summary>
    /// Removes a user-defined editor.
    /// </summary>
    /// <param name="editor">The editor to remove.</param>
    public void RemoveEditor(AiEditor editor)
    {
        if (!editor.IsUserDefined)
        {
            return;
        }

        EditorScanner.RemoveUserEditor(editor.Name);
        this.Editors.Remove(editor);
    }

    /// <summary>
    /// Refreshes an editor's MCP servers and skill usage data.
    /// </summary>
    /// <param name="editor">The editor to refresh.</param>
    public void RefreshEditor(AiEditor editor)
    {
        EditorScanner.LoadMcpServers(editor);
        EditorScanner.LoadSkillUsage(editor);
    }

    /// <summary>
    /// Raises the property changed event.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private static string ParseSkillDescription(string skillFilePath)
    {
        var content = File.ReadAllText(skillFilePath);
        var lines = content.Split('\n');

        foreach (var line in lines.Skip(2))
        {
            var trimmed = line.Trim();
            if (string.IsNullOrEmpty(trimmed))
            {
                continue;
            }

            if (trimmed.StartsWith("description:", StringComparison.OrdinalIgnoreCase))
            {
                return trimmed.Substring("description:".Length).Trim();
            }

            if (trimmed.StartsWith("#") || trimmed.StartsWith("<!--"))
            {
                break;
            }
        }

        return string.Empty;
    }

    private void LoadData()
    {
        // Auto-scan for editors.
        var editors = EditorScanner.ScanAll();
        foreach (var editor in editors)
        {
            this.Editors.Add(editor);
        }

        // Load available skills from well-known directories.
        this.LoadAvailableSkills();
    }

    private void LoadAvailableSkills()
    {
        var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        // Check multiple skill directories from various editors.
        var skillPaths = new[]
        {
            Path.Combine(homeDir, ".agents", "skills"),
            Path.Combine(homeDir, ".gemini", "antigravity", "skills"),
            Path.Combine(homeDir, ".config", "opencode", "skills"),
            Path.Combine(Directory.GetCurrentDirectory(), ".opencode", "skills"),
        };

        foreach (var skillsPath in skillPaths)
        {
            if (!Directory.Exists(skillsPath))
            {
                continue;
            }

            foreach (var skillDir in Directory.GetDirectories(skillsPath))
            {
                var skillName = Path.GetFileName(skillDir);

                // Skip if already added (from another directory).
                if (this.AvailableSkills.Any(s => s.Name == skillName))
                {
                    continue;
                }

                var skillFile = Path.Combine(skillDir, "SKILL.md");
                string description = string.Empty;

                if (File.Exists(skillFile))
                {
                    description = ParseSkillDescription(skillFile);
                }

                this.AvailableSkills.Add(new Skill
                {
                    Name = skillName,
                    Description = description,
                    SourcePath = File.Exists(skillFile) ? skillFile : skillDir,
                });
            }
        }

        var sorted = this.AvailableSkills.OrderBy(s => s.Name).ToList();
        this.AvailableSkills.Clear();
        foreach (var skill in sorted)
        {
            this.AvailableSkills.Add(skill);
        }
    }
}

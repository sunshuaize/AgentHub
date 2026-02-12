// <copyright file="Skill.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AgentHub.Models;

/// <summary>
/// 表示技能信息.
/// </summary>
public class Skill
{
    /// <summary>
    /// Gets or sets the skill name.
    /// </summary>
    /// <value>技能名称.</value>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the skill description.
    /// </summary>
    /// <value>技能描述.</value>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the source file path.
    /// </summary>
    /// <value>技能来源文件路径.</value>
    public string SourcePath { get; set; } = string.Empty;
}

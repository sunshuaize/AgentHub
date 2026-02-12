// <copyright file="SkillUsage.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AgentHub.Models;

/// <summary>
/// 表示技能使用记录.
/// </summary>
public class SkillUsage
{
    /// <summary>
    /// Gets or sets 技能名称.
    /// </summary>
    /// <value>技能名称.</value>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets 使用次数.
    /// </summary>
    /// <value>使用次数.</value>
    public int UsageCount { get; set; }

    /// <summary>
    /// Gets or sets 最后使用时间.
    /// </summary>
    /// <value>最后使用时间.</value>
    public string LastUsed { get; set; } = string.Empty;
}

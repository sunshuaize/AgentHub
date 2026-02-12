// <copyright file="McpServer.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AgentHub.Models;

/// <summary>
/// 表示 MCP 服务器配置.
/// </summary>
public class McpServer
{
    /// <summary>
    /// Gets or sets the server name.
    /// </summary>
    /// <value>服务器名称.</value>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the server type.
    /// </summary>
    /// <value>服务器类型.</value>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the command or URL.
    /// </summary>
    /// <value>命令或 URL.</value>
    public string CommandOrUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the server is enabled.
    /// </summary>
    /// <value>如果启用服务器则为 true；否则为 false.</value>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the source config file path.
    /// </summary>
    /// <value>配置来源文件路径.</value>
    public string SourcePath { get; set; } = string.Empty;
}

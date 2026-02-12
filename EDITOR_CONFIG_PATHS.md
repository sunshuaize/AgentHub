# AgentHub - AI 编辑器配置扫描路径文档

> 本文档记录了 AgentHub 自动扫描的所有 AI 编辑器的配置文件位置、格式差异和扫描规则。
>
> 最后更新：2026-02-12

---

## 目录

1. [Claude Code](#1-claude-code)
2. [Cursor](#2-cursor)
3. [Windsurf](#3-windsurf)
4. [Opencode](#4-opencode)
5. [Continue](#5-continue)
6. [Trae](#6-trae)
7. [Cline](#7-cline)
8. [Roo Code](#8-roo-code)
9. [Skills 扫描目录](#9-skills-扫描目录)
10. [用户自定义编辑器](#10-用户自定义编辑器)
11. [配置格式差异对照表](#11-配置格式差异对照表)

---

## 1. Claude Code

- **图标**：🤖
- **官方文档**：https://code.claude.com/docs/en/settings
- **MCP 文档**：https://code.claude.com/docs/en/mcp
- **MCP JSON 键名**：`mcpServers`

### 配置文件位置（按优先级从低到高）

| 优先级 | 作用域 | 路径 (Windows) | 说明 |
|--------|--------|----------------|------|
| 1 | 用户级 (User) | `%USERPROFILE%\.claude.json` | 主配置文件，包含 MCP 服务器、偏好设置、skillUsage 等 |
| 2 | 项目级 (Project) | `<项目根目录>\.mcp.json` | 项目共享 MCP 配置，可签入 Git |
| 3 | 托管级 (Managed) | `C:\Program Files\ClaudeCode\managed-mcp.json` | 企业 IT 管理员部署的强制配置 |

### 其他相关文件（非 MCP）

| 路径 | 说明 |
|------|------|
| `%USERPROFILE%\.claude\settings.json` | 用户级设置（权限、hooks 等） |
| `<项目>\.claude\settings.json` | 项目级设置（共享） |
| `<项目>\.claude\settings.local.json` | 项目级本地设置（不签入 Git） |
| `%USERPROFILE%\.claude\CLAUDE.md` | 用户级系统提示词 |
| `<项目>\CLAUDE.md` | 项目级系统提示词 |

### MCP 配置格式示例

```json
{
  "mcpServers": {
    "server-name": {
      "type": "stdio",
      "command": "/path/to/server",
      "args": ["--arg1", "value1"],
      "env": {
        "API_KEY": "xxx"
      }
    },
    "remote-server": {
      "type": "http",
      "url": "https://mcp.example.com/mcp",
      "headers": {
        "Authorization": "Bearer token"
      }
    }
  }
}
```

### 特有字段

- `skillUsage`：记录技能使用统计（`usageCount`, `lastUsedAt`）
- `type` 可选值：`stdio`、`http`、`sse`
- `command` 为字符串，`args` 为单独数组

---

## 2. Cursor

- **图标**：📝
- **官方文档**：https://cursor.com/docs
- **MCP 文档**：https://cursor.com/docs/context/mcp
- **Rules 文档**：https://cursor.com/docs/context/rules
- **Skills 文档**：https://cursor.com/docs/context/skills
- **MCP JSON 键名**：`mcpServers`

### MCP 配置文件位置

| 作用域 | 路径 (Windows) | 说明 |
|--------|----------------|------|
| 全局 | `%USERPROFILE%\.cursor\mcp.json` | 全局 MCP 配置，适用于所有工作区 |
| 项目级 | `<项目根目录>\.cursor\mcp.json` | 项目级 MCP 配置，仅当前项目有效 |

> **管理入口**：通过命令面板（`Ctrl+Shift+P`）搜索 "MCP"，或 Settings > Developer > Edit Config > MCP Tools

### 其他相关文件（非 MCP）

| 路径 | 说明 |
|------|------|
| `%APPDATA%\Cursor\User\settings.json` | VS Code 兼容的用户全局设置 |
| `<项目>\.cursor\rules\` | 项目级 Rules 目录（存放 `.mdc` 规则文件，可版本控制） |
| `<项目>\.cursorrules` | 项目级系统提示词（纯文本/Markdown，旧格式但仍支持） |
| `<项目>\AGENTS.md` | Agent 提示词指令（Markdown，简化替代 Rules） |
| `<项目>\.cursor\hooks.json` | Agent Hooks 配置（如 `afterFileEdit` 自动触发 lint/format） |

### Rules 类型说明

| 类型 | 位置 | 说明 |
|------|------|------|
| User Rules | Cursor Settings > Rules (UI) | 用户级，适用于所有项目和对话 |
| Project Rules | `<项目>\.cursor\rules\*.mdc` | 项目级，可签入 Git，支持规则继承和堆叠 |
| `.cursorrules` | `<项目>\.cursorrules` | 项目级，旧格式，纯文本/Markdown |
| Team Rules | 管理仪表盘 (Team/Enterprise) | 组织级，通过管理后台统一配置 |
| `AGENTS.md` | `<项目>\AGENTS.md` | 项目级，简化版 Agent 指令 |

### MCP 配置格式示例

```json
{
  "mcpServers": {
    "server-name": {
      "command": "npx",
      "args": ["-y", "@modelcontextprotocol/server-filesystem"],
      "env": {}
    },
    "remote-server": {
      "url": "https://mcp.example.com/sse",
      "env": {}
    }
  }
}
```

### 格式特点

- 与 Claude Code 格式基本一致
- `command` 为字符串，`args` 为数组
- 支持 stdio 和 SSE 连接方式
- 没有 `type` 字段时默认为 stdio；远程服务器使用 `url` 字段
- 没有 `enabled`/`disabled` 字段

---

## 3. Windsurf

- **图标**：🏄
- **MCP JSON 键名**：`mcpServers`

### 配置文件位置

| 作用域 | 路径 (Windows) | 说明 |
|--------|----------------|------|
| 全局 | `%USERPROFILE%\.codeium\windsurf\mcp_config.json` | 全局 MCP 配置 |

### MCP 配置格式示例

```json
{
  "mcpServers": {
    "server-name": {
      "command": "npx",
      "args": ["-y", "some-mcp-server"],
      "env": {}
    }
  }
}
```

### 格式特点

- 与 Claude Code / Cursor 格式一致
- `command` 为字符串，`args` 为数组

---

## 4. Opencode

- **图标**：⌨️
- **官方文档**：https://opencode.ai/docs/zh-cn/config/
- **MCP 文档**：https://opencode.ai/docs/zh-cn/mcp-servers/
- **MCP JSON 键名**：`mcp`（注意：不是 `mcpServers`）

### 配置文件位置（按优先级从低到高）

| 优先级 | 作用域 | 路径 (Windows) | 说明 |
|--------|--------|----------------|------|
| 1 | Remote | `.well-known/opencode` 端点 | 组织远程默认配置（自动获取） |
| 2 | 全局 (Global) | `%USERPROFILE%\.config\opencode\opencode.json` | 用户全局配置 |
| 3 | 自定义 (Custom) | `OPENCODE_CONFIG` 环境变量指定 | 自定义覆盖 |
| 4 | 项目级 (Project) | `<项目根目录>\opencode.json` | 项目特定设置（最高优先级） |
| 5 | .opencode 目录 | `<项目根目录>\.opencode\` | agents/commands/skills 等 |
| 6 | 内联配置 | `OPENCODE_CONFIG_CONTENT` 环境变量 | 运行时覆盖 |

> **注意**：配置文件是**合并**而非替换。仅冲突键按优先级覆盖。

### 其他相关目录

| 路径 | 说明 |
|------|------|
| `%USERPROFILE%\.config\opencode\agents\` | 全局 Agents |
| `%USERPROFILE%\.config\opencode\skills\` | 全局 Skills |
| `%USERPROFILE%\.config\opencode\commands\` | 全局 Commands |
| `%USERPROFILE%\.config\opencode\plugins\` | 全局 Plugins |
| `%USERPROFILE%\.config\opencode\themes\` | 全局 Themes |
| `<项目>\.opencode\agents\` | 项目级 Agents |
| `<项目>\.opencode\skills\` | 项目级 Skills |

> 子目录使用**复数名称**（agents/、skills/），向后兼容支持单数（agent/、skill/）。

### MCP 配置格式示例

```json
{
  "$schema": "https://opencode.ai/config.json",
  "mcp": {
    "my-local-server": {
      "type": "local",
      "command": ["npx", "-y", "@modelcontextprotocol/server-everything"],
      "enabled": true,
      "environment": {
        "MY_ENV_VAR": "value"
      }
    },
    "my-remote-server": {
      "type": "remote",
      "url": "https://mcp.example.com",
      "enabled": true,
      "headers": {
        "Authorization": "Bearer MY_API_KEY"
      }
    }
  }
}
```

### 格式特点（⚠️ 与 Claude Code 有显著差异）

| 字段 | Claude Code | Opencode |
|------|-------------|----------|
| MCP 键名 | `mcpServers` | `mcp` |
| `type` 值 | `stdio` / `http` / `sse` | `local` / `remote` |
| `command` 格式 | 字符串 | **数组** |
| 参数字段 | `args`（数组） | 包含在 `command` 数组中 |
| 环境变量字段 | `env` | `environment` |
| 启用字段 | 无（默认启用） | `enabled`（布尔值） |

---

## 5. Continue

- **图标**：▶️
- **MCP JSON 键名**：`mcpServers`

### 配置文件位置

| 作用域 | 路径 (Windows) | 说明 |
|--------|----------------|------|
| 全局 | `%USERPROFILE%\.continue\config.json` | 全局配置 |

### MCP 配置格式示例

```json
{
  "mcpServers": {
    "server-name": {
      "command": "npx",
      "args": ["-y", "some-mcp-server"],
      "env": {}
    }
  }
}
```

### 格式特点

- 与 Claude Code 格式基本一致

---

## 6. Trae

- **图标**：🌲
- **MCP JSON 键名**：`mcpServers`

### 配置文件位置

| 作用域 | 路径 (Windows) | 说明 |
|--------|----------------|------|
| 全局 | `%USERPROFILE%\.trae\mcp.json` | 全局 MCP 配置 |

### MCP 配置格式

- 与 Claude Code 格式一致

---

## 7. Cline

- **图标**：🔮
- **MCP JSON 键名**：`mcpServers`
- **依赖**：VS Code 扩展

### 配置文件位置

| 作用域 | 路径 (Windows) | 说明 |
|--------|----------------|------|
| 全局 | `%APPDATA%\Code\User\globalStorage\saoudrizwan.claude-dev\settings\cline_mcp_settings.json` | VS Code 扩展全局存储 |

### MCP 配置格式示例

```json
{
  "mcpServers": {
    "server-name": {
      "command": "npx",
      "args": ["-y", "some-mcp-server"],
      "env": {},
      "disabled": false,
      "autoApprove": ["tool1", "tool2"]
    }
  }
}
```

### 格式特点

| 字段 | 说明 |
|------|------|
| `disabled` | 布尔值，`true` = 已禁用（注意：与 `enabled` **相反**） |
| `autoApprove` | 自动批准的工具列表 |

---

## 8. Roo Code

- **图标**：🦘
- **MCP JSON 键名**：`mcpServers`
- **依赖**：VS Code 扩展（Cline 分支）

### 配置文件位置

| 作用域 | 路径 (Windows) | 说明 |
|--------|----------------|------|
| 全局 | `%APPDATA%\Code\User\globalStorage\rooveterinaryinc.roo-cline\settings\cline_mcp_settings.json` | VS Code 扩展全局存储 |

### 格式特点

- 与 Cline 格式一致（同为 Cline 分支）
- 使用 `disabled` 而非 `enabled`

---

## 9. Skills 扫描目录

AgentHub 从以下目录扫描可用 Skills：

| 路径 (Windows) | 来源 | 说明 |
|----------------|------|------|
| `%USERPROFILE%\.agents\skills\` | Claude Code / 通用 | 每个子目录为一个 Skill，含 `SKILL.md` |
| `%USERPROFILE%\.gemini\antigravity\skills\` | Gemini / Antigravity | 同上 |
| `%USERPROFILE%\.config\opencode\skills\` | Opencode 全局 | Opencode 全局 Skills |
| `<项目>\.opencode\skills\` | Opencode 项目级 | Opencode 项目级 Skills |

### Skill 目录结构

```
skills/
├── skill-name/
│   ├── SKILL.md          # 必需，包含描述（description: ...）
│   ├── scripts/          # 可选，辅助脚本
│   ├── examples/         # 可选，参考实现
│   └── resources/        # 可选，模板/资源
```

### Skill 描述解析规则

从 `SKILL.md` 文件提取描述：
1. 跳过前 2 行（YAML frontmatter `---`）
2. 查找 `description:` 行（不区分大小写）
3. 提取 `:` 后面的文本作为描述
4. 遇到 `#` 标题或 `<!--` 注释则停止

---

## 10. 用户自定义编辑器

用户可通过 AgentHub 的"➕ 添加编辑器"功能手动添加不在自动扫描列表中的编辑器。

### 用户配置文件

- **路径**：`%USERPROFILE%\.agenthub\config.json`
- **创建时机**：首次添加自定义编辑器时自动创建

### 配置格式

```json
{
  "customEditors": [
    {
      "name": "My Custom Editor",
      "icon": "🔧",
      "configPath": "C:\\path\\to\\config.json",
      "mcpKey": "mcpServers"
    }
  ]
}
```

### 字段说明

| 字段 | 类型 | 说明 |
|------|------|------|
| `name` | string | 编辑器显示名称 |
| `icon` | string | emoji 图标 |
| `configPath` | string | 配置文件绝对路径 |
| `mcpKey` | string | MCP 服务器的 JSON 键名（默认 `mcpServers`） |

---

## 11. 配置格式差异对照表

| 特性 | Claude Code | Opencode | Cursor | Windsurf | Cline/Roo Code | Continue | Trae |
|------|-------------|----------|--------|----------|----------------|----------|------|
| **MCP 键名** | `mcpServers` | `mcp` | `mcpServers` | `mcpServers` | `mcpServers` | `mcpServers` | `mcpServers` |
| **type 值** | stdio/http/sse | local/remote | stdio | stdio | stdio | stdio | stdio |
| **command 格式** | 字符串 | 数组 | 字符串 | 字符串 | 字符串 | 字符串 | 字符串 |
| **参数字段** | `args` 数组 | 含在 command 中 | `args` 数组 | `args` 数组 | `args` 数组 | `args` 数组 | `args` 数组 |
| **环境变量** | `env` | `environment` | `env` | `env` | `env` | `env` | `env` |
| **启用/禁用** | 无 | `enabled` | 无 | 无 | `disabled`（反向） | 无 | 无 |
| **多层配置** | ✅ 3 层 | ✅ 6 层 | ✅ 2 层 | ❌ 1 层 | ❌ 1 层 | ❌ 1 层 | ❌ 1 层 |
| **Skill 使用统计** | ✅ `skillUsage` | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |

---

## 解析优先级说明

当同一编辑器有多个配置文件时，AgentHub 按以下规则处理：

1. **先加载的优先**：ConfigFiles 列表中排在前面的文件优先
2. **避免重复**：如果同名 MCP 服务器已从更高优先级文件加载，跳过低优先级的同名服务器
3. **SourcePath 精准**：每个 MCP 服务器的 `SourcePath` 精确记录其实际来源文件
4. **配置合并**：不同名的服务器从所有文件中收集

---

## 参考链接

- Claude Code Settings: https://code.claude.com/docs/en/settings
- Claude Code MCP: https://code.claude.com/docs/en/mcp
- Cursor Docs: https://cursor.com/docs
- Cursor MCP: https://cursor.com/docs/context/mcp
- Cursor Rules: https://cursor.com/docs/context/rules
- Cursor Skills: https://cursor.com/docs/context/skills
- Opencode Config: https://opencode.ai/docs/zh-cn/config/
- Opencode MCP: https://opencode.ai/docs/zh-cn/mcp-servers/

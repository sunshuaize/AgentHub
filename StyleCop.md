# AgentHub Global Rules

本文件包含 AgentHub 项目的全局编码规则和规范，所有代理和开发工作都必须遵循。

## 重要说明

### 注释语言规范
**规则**: XML 文档注释的内容使用中文，但固定术语使用英文。

**说明**:
- ✅ 注释描述内容可以使用中文（如：主窗口、加载所有数据）
- ✅ 固定术语必须使用英文（如：`Gets or sets`、`Gets`、`indicating whether`）
- ✅ 标点符号必须使用英文（如：`.` 结尾）

**示例**:
```csharp
// ✅ 正确 - 中文描述 + 英文术语 + 英文标点
/// <summary>
/// Gets or sets 服务器名称.
/// </summary>
public string Name { get; set; }

// ❌ 错误 - 中文术语 + 中文标点
/// <summary>
/// 获取或设置服务器名称。
/// </summary>
public string Name { get; set; }
```

---

本文档记录了 StyleCop.Analyzers 的配置规范和常见问题解决方案。

## 已修复的规范问题

### 1. 文件头注释 (SA1633, SA1636)
**规则**: 所有 .cs 文件必须在文件开头添加版权信息注释。

**标准格式**:
```csharp
// <copyright file="FileName.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
```

**重要说明**:
- 使用 `<copyright>` XML 标签格式
- company 属性使用 "PlaceholderCompany"（StyleCop 标准占位符）
- 不要添加额外的分隔线
- 每行都以 `//` 开头

### 2. XML 文档注释 (SA1600, SA1604)
**规则**: 所有公共成员必须添加 XML 文档注释。

**类注释**:
```csharp
/// <summary>
/// 类的描述。
/// </summary>
public class MyClass
```

**属性注释** (SA1623):
```csharp
/// <summary>
/// Gets or sets 属性名称.
/// </summary>
/// <value>属性名称的值.</value>
public string MyProperty { get; set; }
```

**布尔属性**:
```csharp
/// <summary>
/// Gets or sets a value indicating whether 启用功能.
/// </summary>
/// <value>如果启用则为 <c>true</c>；否则为 <c>false</c>.</value>
public bool IsEnabled { get; set; }
```

**只读属性**:
```csharp
/// <summary>
/// Gets 集合.
/// </summary>
/// <value>集合实例.</value>
public ObservableCollection<string> Items { get; } = new();
```

**重要**: 必须使用英文格式，以 "Gets or sets" 开头，不要使用中文"获取或设置"。

### 3. 文档标点符号 (SA1629)
**规则**: 所有 XML 文档注释文本必须以英文句号结尾，不能使用中文句号。

```csharp
// ❌ 错误 - 使用中文句号
/// <summary>
/// 加载数据。
/// </summary>
private void LoadData()

// ✅ 正确 - 使用英文句号
/// <summary>
/// Loads the data.
/// </summary>
private void LoadData()
```

**重要**: StyleCop 只识别英文句号 `.`，不识别中文句号 `。`

### 4. Using 指令位置 (SA1200)
**规则**: Using 指令应放在命名空间声明内部。

```csharp
namespace AgentHub.ViewModels;

using System.Collections.ObjectModel;
using System.ComponentModel;
```

### 5. Using 指令排序 (SA1208, SA1210)
**规则**: 
- System 命名空间优先
- 按字母顺序排列
- 第三方命名空间在后

```csharp
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using AgentHub.Models;
```

### 6. 成员排序 (SA1201, SA1202)
**规则**: 成员应按照以下顺序排列：
1. 字段
2. 构造函数
3. 属性、事件、索引器
4. 方法（public -> protected -> private）

```csharp
public class MyClass
{
    // 事件
    public event EventHandler? MyEvent;

    // 属性
    public string Name { get; set; }

    // 构造函数（在属性之后，但这个规则比较复杂）
    public MyClass() { }

    // 公共方法
    public void DoSomething() { }

    // 保护方法
    protected void OnEvent() { }

    // 私有方法
    private void LoadData() { }
}
```

### 7. 构造函数文档标准文本 (SA1642)
**规则**: 构造函数摘要文档应该以 "初始化" 开头
**标准格式**:
```csharp
/// <summary>
/// 初始化 <see cref="ClassName"/> 类的新实例。
/// </summary>
public ClassName()
{
}
```

### 8. This 前缀 (SA1101)
**规则**: 调用实例成员时应使用 this. 前缀
**示例**:
```csharp
public void LoadData()
{
    this.LoadClaudeCodeConfig();
    this.LoadOpencodeConfig();
}
```

### 9. 空字符串 (SA1122)
**规则**: 使用 string.Empty 代替 ""
**示例**:
```csharp
// ❌ 错误
string name = "";

// ✅ 正确
string name = string.Empty;
```

### 10. 多行初始化器尾随逗号 (SA1413)
**规则**: 多行初始化器最后一项后应添加逗号
**示例**:
```csharp
// ❌ 错误
var server = new McpServer
{
    Name = "test",
    Type = "stdio"
};

// ✅ 正确
var server = new McpServer
{
    Name = "test",
    Type = "stdio",
};
```

### 11. 单一类型文件 (SA1402)
**规则**: 每个文件应只包含一个类型定义
**解决方案**: 将 DataModels.cs 拆分为多个文件（McpServer.cs、SkillUsage.cs、Skill.cs）

### 12. 文件名匹配类型名 (SA1649)
**规则**: 文件名应与文件中第一个类型名匹配
**解决方案**: 重命名文件或拆分文件

## 修复完成总结

所有 StyleCop 警告已全部修复，项目当前编译状态：
- ✅ 0 个错误
- ✅ 0 个警告
- ✅ 完全符合 StyleCop.Analyzers 规范

### 最新修复 (2026-02-12)

1. **SA1636** - 使用标准版权声明格式
   - 使用 `<copyright>` XML 标签
   - company 属性使用 "PlaceholderCompany"

2. **SA1629** - 中文标点符号替换为英文
   - 所有 `。` → `.`

3. **SA1623** - 属性文档使用英文标准格式
   - `获取或设置` → `Gets or sets`
   - `获取` → `Gets`
   - `指示` → `indicating`

4. **SA1642** - 构造函数文档标准化
    - 使用标准格式："初始化 <see cref="ClassName"/> 类的新实例."

5. **SA1201** - 成员顺序规范化
   - 正确顺序：字段 → 构造函数 → 事件 → 属性 → 方法

6. **SA1518** - 删除文件末尾空行
   - 确保每个文件以 `}` 结束，无多余空行

### 注释语言规范总结

**核心原则**：中文描述 + 英文术语 + 英文标点

| 项目 | 规则 | 示例 |
|------|--------|------|
| 类描述 | 中文 + 英文句号 | `主视图模型.` |
| 构造函数 | 标准英文格式 | `Initializes a new instance...` |
| 属性(get/set) | "Gets or sets" + 中文描述 | `Gets or sets 服务器名称.` |
| 属性(get only) | "Gets" + 中文描述 | `Gets 服务器集合.` |
| 布尔属性 | "Gets or sets a value indicating whether" | `Gets or sets a value indicating whether...` |

## 配置文件说明

### stylecop.json 关键配置

```json
{
  "documentationRules": {
    "fileNamingConvention": "metadata",
    "documentExposedElements": true,
    "documentInternalElements": false,
    "documentPrivateElements": false
  },
  "namingRules": {
    "allowCommonHungarianPrefixes": false,
    "requireHungarianPrefix": false,
    "allowUnderscoresInPublicNames": false
  },
  "layoutRules": {
    "indentationSize": 4,
    "tabSize": 4,
    "useTabs": false,
    "maxLineLength": 120,
    "newLinesBetweenMembers": "always"
  }
}
```

**说明**:
- 只需要为公共元素添加文档注释
- 内部和私有成员不需要文档注释
- 不允许匈牙利命名法
- 不允许公共名称中有下划线
- 使用空格缩进，不使用 Tab
- 最大行长度 120 字符
- 成员之间始终需要空行

## 规范遵循状态

✅ **所有 StyleCop 警告已修复** 
- 0 个错误
- 0 个警告
- 完全符合 StyleCop.Analyzers 规范

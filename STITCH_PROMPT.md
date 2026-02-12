# AgentHub - Google Stitch UI 提示词

---

## 提示词（英文，适用于 Stitch）

```
Design a **Windows PC desktop client application** UI called "AgentHub". This is NOT a website or web app — it is a native Windows desktop application built with WPF (Windows Presentation Foundation), similar to apps like VS Code, JetBrains Toolbox, or Docker Desktop. It is a management dashboard for viewing and managing AI editor configurations (MCP servers & Skills) across multiple AI coding assistants.

**Important: This is a PC client application.** It should have:
- A native Windows application window with standard title bar (minimize, maximize, close buttons)
- Desktop app proportions (approximately 1400×750 pixels)
- Native-feeling controls (list boxes, tree views, buttons, text inputs) — not web-style cards or mobile layouts
- No browser chrome, no URL bar, no tabs — this is a standalone executable window

## Layout: Three-Column Master-Detail

### Header Bar (top, full width)
- Dark background (#1E1E2E gradient to #2D2D40)
- Left: App logo/icon + "AgentHub" title in bold white text, with tagline "MCP & Skills Manager" in muted gray (#888)
- Right: a small refresh icon button + app version badge "v1.0"

### Column 1: AI Editors Sidebar (fixed 220px, left)
- Light background (#FAFAFA)
- Section title: "AI 编辑器" with a small icon
- Scrollable list of detected editors, each row shows:
  - Emoji icon (🤖, 📝, 🏄, ⌨️, ▶️, 🌲, 🔮, 🦘)
  - Editor name (Claude Code, Cursor, Windsurf, Opencode, Continue, Trae, Cline, Roo Code)
  - Small status badge: "Auto-detected" (green) or "User-added" (blue)
- Currently selected editor has a highlighted background (soft blue-purple accent)
- Bottom action buttons:
  - "➕ Add Editor" button (dark, primary style)
  - "🗑️ Remove Selected" button (red, only visible when a user-defined editor is selected)
  - "🔄 Refresh" button (outline style)

### Column 2: MCP & Skills Tree (fixed 300px, center)
- White background
- Top section header bar (#F0F0F0): shows selected editor name + icon (e.g. "🤖 Claude Code - MCP / Skills")
- Tree view below with two root nodes, both expanded:
  - "📡 MCP Servers (3)" — expandable, children are individual server names like "filesystem", "github", "agenthub-local"
  - "🧩 Skills (12)" — expandable, children are skill names like "algorithmic-art", "canvas-design", "frontend-design"
- Each tree item has a subtle hover effect
- Selected tree item highlighted

### Column 3: Detail Panel (flex, fills remaining space, right)
- White background with generous padding
- Shows contextual detail based on what is selected:

**When an Editor is selected (overview):**
- Large header: "🤖 Claude Code"
- Status: "Auto-detected" with green badge
- MCP Count: "3 servers"
- Section: "📁 Config File Sources" with clickable file paths:
  - `C:\Users\User\.claude.json` (blue, underlined, with tooltip "Ctrl+Click to open")
  - `C:\Users\User\project\.mcp.json`
- Footer hint: "👈 Select an MCP server or Skill from the tree to view details"

**When an MCP Server is selected:**
- Header: "📡 MCP Server"
- Separator line
- Detail rows (label: value grid):
  - Name: filesystem
  - Type: stdio
  - Status: ✅ Enabled (green) or ⛔ Disabled (red)
  - Command/URL: `npx -y @modelcontextprotocol/server-filesystem`
- Separator
- Source file: clickable path in blue

**When a Skill is selected:**
- Header: "🧩 Skill"
- Name: frontend-design
- Description: "Create distinctive, production-grade frontend interfaces..."
- Source: clickable path
- Section "📊 Usage Stats" (if available):
  - Usage Count: 15
  - Last Used: 2026-02-10 14:30

## Modal Dialog: "Add Editor"
Show a separate modal dialog overlaying the main window:
- Title: "添加 AI 编辑器"
- Form fields:
  - Editor Name (text input)
  - Config File Path (text input + "Browse..." button)
  - MCP Config Key (dropdown: "mcpServers" / "mcp", editable)
  - Icon (small text input, default "🔧")
- Bottom buttons: "Cancel" (outline) and "OK" (dark primary)

## Design Requirements
- **This is a Windows PC desktop client**, NOT a web page or mobile app. Render it as a native Windows application window
- Include a standard Windows title bar at the top with "AgentHub - MCP & Skills Viewer" as the window title, and minimize/maximize/close buttons
- Clean, professional look inspired by VS Code / JetBrains Toolbox / Docker Desktop
- Color palette: dark header (#1E1E2E), white content areas, soft gray borders (#E0E0E0), accent blue-purple (#6366F1) for selections
- Font: Microsoft YaHei UI or Segoe UI (standard Windows system fonts), clean sans-serif
- Subtle shadows and rounded corners on panels
- Smooth hover effects on list items and buttons
- Use clear visual hierarchy with proper spacing
- The UI language is primarily Chinese (Simplified) with English technical terms
- Native desktop controls: WPF-style ListBox, TreeView, TextBox, ComboBox, Button — not web-style components
- Window size approximately 1400×750, no scrollbar on the main window (only inside panels)
- Show realistic sample data in the mockup
```

---


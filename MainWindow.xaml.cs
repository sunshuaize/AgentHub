// <copyright file="MainWindow.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AgentHub;

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using AgentHub.Models;
using AgentHub.ViewModels;

/// <summary>
/// Main window — Stitch-inspired layout.
/// </summary>
public partial class MainWindow : Window
{
    // ========== Design tokens (matching Stitch output) ==========
    private static readonly SolidColorBrush Primary = B("#6467F2");
    private static readonly SolidColorBrush PrimarySubtle = B("#EEF2FF");
    private static readonly SolidColorBrush PrimaryBorder = B("#C7D2FE");
    private static readonly SolidColorBrush TextDark = B("#1E293B");
    private static readonly SolidColorBrush TextMedium = B("#64748B");
    private static readonly SolidColorBrush TextLight = B("#94A3B8");
    private static readonly SolidColorBrush CardBorder = B("#E2E8F0");
    private static readonly SolidColorBrush CardBg = B("#F8FAFC");
    private static readonly SolidColorBrush GreenBadgeBg = B("#D1FAE5");
    private static readonly SolidColorBrush GreenText = B("#059669");
    private static readonly SolidColorBrush RedBadgeBg = B("#FEE2E2");
    private static readonly SolidColorBrush RedText = B("#DC2626");

    private MainViewModel? viewModel;
    private AiEditor? currentEditor;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        this.InitializeComponent();
        this.viewModel = new MainViewModel();
        this.DataContext = this.viewModel;

        if (this.viewModel.Editors.Count > 0)
        {
            this.EditorsListBox.SelectedIndex = 0;
        }
    }

    // ── helpers ──────────────────────────────────────────────────────
    private static SolidColorBrush B(string hex)
    {
        var c = (Color)ColorConverter.ConvertFromString(hex);
        var b = new SolidColorBrush(c);
        b.Freeze();
        return b;
    }

    // ── Events ──────────────────────────────────────────────────────
    private void OnEditorChanged(object sender, SelectionChangedEventArgs e)
    {
        if (this.EditorsListBox.SelectedItem is AiEditor editor)
        {
            this.currentEditor = editor;
            this.CenterHeaderText.Text = "Explorer";
            this.PopulateTreeView(editor);
            this.ShowEditorOverview();

            this.RemoveEditorBtn.Visibility = editor.IsUserDefined
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
    }

    private void PopulateTreeView(AiEditor editor)
    {
        this.McpServersNode.Items.Clear();
        this.SkillsNode.Items.Clear();

        foreach (var s in editor.McpServers)
        {
            this.McpServersNode.Items.Add(new TreeViewItem
            {
                Header = s.Name,
                Tag = s,
                FontSize = 12.5,
                FontWeight = FontWeights.Normal,
                Foreground = TextMedium,
                Padding = new Thickness(2, 3, 2, 3),
            });
        }

        this.McpServersNode.Header = $"📡  MCP Servers ({editor.McpServers.Count})";

        if (this.viewModel != null)
        {
            foreach (var sk in this.viewModel.AvailableSkills)
            {
                this.SkillsNode.Items.Add(new TreeViewItem
                {
                    Header = sk.Name,
                    Tag = sk,
                    FontSize = 12.5,
                    FontWeight = FontWeights.Normal,
                    Foreground = TextMedium,
                    Padding = new Thickness(2, 3, 2, 3),
                });
            }

            this.SkillsNode.Header = $"🧩  Skills ({this.viewModel.AvailableSkills.Count})";
        }
    }

    private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if (e.NewValue is TreeViewItem { Tag: not null } item)
        {
            this.DisplayItemDetails(item.Tag);
        }
    }

    private void OnAddEditor(object sender, RoutedEventArgs e)
    {
        var dlg = new AddEditorDialog { Owner = this };
        if (dlg.ShowDialog() == true && this.viewModel != null)
        {
            var ed = new AiEditor
            {
                Name = dlg.EditorName,
                Icon = dlg.EditorIcon,
                ConfigPath = dlg.ConfigFilePath,
                IsUserDefined = true,
                McpKey = dlg.McpKey,
            };
            this.viewModel.AddEditor(ed);
            this.EditorsListBox.SelectedItem = ed;
        }
    }
    private void OnMinimizeWindow(object sender, RoutedEventArgs e)
    {
        this.WindowState = WindowState.Minimized;
    }

    private void OnMaximizeRestoreWindow(object sender, RoutedEventArgs e)
    {
        if (this.WindowState == WindowState.Maximized)
        {
            this.WindowState = WindowState.Normal;
            this.MaximizeBtn.Content = "⬜";
        }
        else
        {
            this.WindowState = WindowState.Maximized;
            this.MaximizeBtn.Content = "❐";
        }
    }

    private void OnCloseWindow(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void OnRemoveEditor(object sender, RoutedEventArgs e)
    {
        if (this.currentEditor is not { IsUserDefined: true } || this.viewModel == null)
        {
            return;
        }

        if (MessageBox.Show($"确定要移除 \"{this.currentEditor.Name}\" 吗？", "确认",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        {
            this.viewModel.RemoveEditor(this.currentEditor);
            this.currentEditor = null;
            this.DetailsPanel.Children.Clear();
            if (this.viewModel.Editors.Count > 0)
            {
                this.EditorsListBox.SelectedIndex = 0;
            }
        }
    }

    private void OnRefreshEditor(object sender, RoutedEventArgs e)
    {
        if (this.currentEditor == null || this.viewModel == null)
        {
            return;
        }

        this.viewModel.RefreshEditor(this.currentEditor);
        this.PopulateTreeView(this.currentEditor);
        this.ShowEditorOverview();
    }

    // ── Detail Panel Rendering (Stitch-style) ───────────────────────

    private void DisplayItemDetails(object item)
    {
        this.DetailsPanel.Children.Clear();
        if (item is McpServer srv)
        {
            this.RenderMcpDetail(srv);
        }
        else if (item is Skill sk)
        {
            this.RenderSkillDetail(sk);
        }
    }

    /// <summary>
    /// Shows editor overview (Stitch right-panel style).
    /// </summary>
    private void ShowEditorOverview()
    {
        this.DetailsPanel.Children.Clear();
        if (this.currentEditor == null)
        {
            return;
        }

        var ed = this.currentEditor;

        // ── Detail Header section ──
        var headerSection = new Border
        {
            Padding = new Thickness(32, 28, 32, 24),
            BorderBrush = CardBorder,
            BorderThickness = new Thickness(0, 0, 0, 1),
        };

        var headerGrid = new Grid();
        headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        // -- Large emoji avatar box --
        var avatarBox = new Border
        {
            Width = 72,
            Height = 72,
            CornerRadius = new CornerRadius(16),
            Background = PrimarySubtle,
            BorderBrush = PrimaryBorder,
            BorderThickness = new Thickness(1),
            Margin = new Thickness(0, 0, 20, 0),
            Effect = new DropShadowEffect
            {
                Color = ((SolidColorBrush)Primary).Color,
                Opacity = 0.08,
                BlurRadius = 12,
                ShadowDepth = 2,
            },
        };
        avatarBox.Child = new TextBlock
        {
            Text = ed.Icon,
            FontSize = 36,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };
        Grid.SetColumn(avatarBox, 0);
        headerGrid.Children.Add(avatarBox);

        // -- Title + Active badge + description --
        var infoPanel = new StackPanel { VerticalAlignment = VerticalAlignment.Center };

        var titleRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 4) };
        titleRow.Children.Add(new TextBlock
        {
            Text = ed.Name,
            FontSize = 26,
            FontWeight = FontWeights.Bold,
            Foreground = TextDark,
            VerticalAlignment = VerticalAlignment.Center,
        });

        // Active / User-added badge
        var badgeBg = ed.IsUserDefined ? B("#DBEAFE") : GreenBadgeBg;
        var badgeFg = ed.IsUserDefined ? B("#2563EB") : GreenText;
        var badgeText = ed.IsUserDefined ? "USER-ADDED" : "● ACTIVE";

        var badge = new Border
        {
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(8, 3, 8, 3),
            Margin = new Thickness(12, 0, 0, 0),
            Background = badgeBg,
            BorderBrush = ed.IsUserDefined ? B("#BFDBFE") : B("#A7F3D0"),
            BorderThickness = new Thickness(1),
            VerticalAlignment = VerticalAlignment.Center,
        };
        badge.Child = new TextBlock
        {
            Text = badgeText,
            FontSize = 10,
            FontWeight = FontWeights.Bold,
            Foreground = badgeFg,
        };
        titleRow.Children.Add(badge);
        infoPanel.Children.Add(titleRow);

        infoPanel.Children.Add(new TextBlock
        {
            Text = $"AgentHub is managing {ed.McpServers.Count} MCP servers and your configured skills.",
            FontSize = 12.5,
            Foreground = TextLight,
            TextWrapping = TextWrapping.Wrap,
            MaxWidth = 420,
            LineHeight = 18,
        });

        Grid.SetColumn(infoPanel, 1);
        headerGrid.Children.Add(infoPanel);
        headerSection.Child = headerGrid;
        this.DetailsPanel.Children.Add(headerSection);

        // ── Stat Cards Row ──
        var statsGrid = new Grid { Margin = new Thickness(32, 20, 32, 0) };
        statsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        statsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(8) });
        statsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        statsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(8) });
        statsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        int skillCount = this.viewModel?.AvailableSkills.Count ?? 0;

        var c1 = this.CreateStatCard("MCP SERVERS", ed.McpServers.Count.ToString("D2"), "Healthy", GreenText);
        Grid.SetColumn(c1, 0);
        statsGrid.Children.Add(c1);

        var c2 = this.CreateStatCard("ACTIVE SKILLS", skillCount.ToString("D2"), "Enabled", Primary);
        Grid.SetColumn(c2, 2);
        statsGrid.Children.Add(c2);

        var c3 = this.CreateStatCard("CONFIG FILES", ed.ConfigFiles.Count > 0 ? ed.ConfigFiles.Count.ToString() : "1", "Sources", TextLight);
        Grid.SetColumn(c3, 4);
        statsGrid.Children.Add(c3);

        this.DetailsPanel.Children.Add(statsGrid);

        // ── Configuration Files section ──
        var filesSection = new StackPanel { Margin = new Thickness(32, 24, 32, 0) };
        filesSection.Children.Add(new TextBlock
        {
            Text = "Configuration Files",
            FontSize = 13,
            FontWeight = FontWeights.Bold,
            Foreground = TextDark,
            Margin = new Thickness(0, 0, 0, 12),
        });

        var filePaths = ed.ConfigFiles.Count > 0
            ? ed.ConfigFiles
            : new System.Collections.Generic.List<string> { ed.ConfigPath };

        string[] labels = { "SYSTEM CONFIG", "MCP BRIDGE", "PROJECT CONFIG", "USER CONFIG" };
        int i = 0;
        foreach (var fp in filePaths)
        {
            var label = i < labels.Length ? labels[i] : $"CONFIG {i + 1}";
            filesSection.Children.Add(this.CreateConfigFileCard(label, fp));
            i++;
        }

        this.DetailsPanel.Children.Add(filesSection);
    }

    /// <summary>
    /// Renders MCP Server detail (Stitch style).
    /// </summary>
    private void RenderMcpDetail(McpServer server)
    {
        // Header
        var headerSection = new Border
        {
            Padding = new Thickness(32, 28, 32, 24),
            BorderBrush = CardBorder,
            BorderThickness = new Thickness(0, 0, 0, 1),
        };

        var headerGrid = new Grid();
        headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        var avatar = new Border
        {
            Width = 72, Height = 72,
            CornerRadius = new CornerRadius(16),
            Background = PrimarySubtle,
            BorderBrush = PrimaryBorder,
            BorderThickness = new Thickness(1),
            Margin = new Thickness(0, 0, 20, 0),
        };
        avatar.Child = new TextBlock
        {
            Text = "📡",
            FontSize = 36,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };
        Grid.SetColumn(avatar, 0);
        headerGrid.Children.Add(avatar);

        var info = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
        var titleRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 4) };
        titleRow.Children.Add(new TextBlock
        {
            Text = server.Name,
            FontSize = 26,
            FontWeight = FontWeights.Bold,
            Foreground = TextDark,
        });

        var statusBg = server.Enabled ? GreenBadgeBg : RedBadgeBg;
        var statusFg = server.Enabled ? GreenText : RedText;
        var statusText = server.Enabled ? "● ENABLED" : "● DISABLED";

        var statusBadge = new Border
        {
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(8, 3, 8, 3),
            Margin = new Thickness(12, 0, 0, 0),
            Background = statusBg,
            BorderThickness = new Thickness(1),
            BorderBrush = server.Enabled ? B("#A7F3D0") : B("#FECACA"),
            VerticalAlignment = VerticalAlignment.Center,
        };
        statusBadge.Child = new TextBlock
        {
            Text = statusText,
            FontSize = 10,
            FontWeight = FontWeights.Bold,
            Foreground = statusFg,
        };
        titleRow.Children.Add(statusBadge);
        info.Children.Add(titleRow);

        info.Children.Add(new TextBlock
        {
            Text = $"MCP Server · Type: {server.Type}",
            FontSize = 12.5,
            Foreground = TextLight,
        });

        Grid.SetColumn(info, 1);
        headerGrid.Children.Add(info);
        headerSection.Child = headerGrid;
        this.DetailsPanel.Children.Add(headerSection);

        // Detail rows
        var content = new StackPanel { Margin = new Thickness(32, 24, 32, 0) };

        content.Children.Add(new TextBlock
        {
            Text = "Server Details",
            FontSize = 13,
            FontWeight = FontWeights.Bold,
            Foreground = TextDark,
            Margin = new Thickness(0, 0, 0, 12),
        });

        var detailCard = this.CreateInfoCard();
        var panel = (StackPanel)detailCard.Child;

        panel.Children.Add(this.Kvp("类型 (Type)", server.Type));
        panel.Children.Add(this.Sep());
        panel.Children.Add(this.Kvp("命令/URL", server.CommandOrUrl));
        panel.Children.Add(this.Sep());
        panel.Children.Add(this.Kvp("状态", server.Enabled ? "✅ 已启用" : "⛔ 未启用"));
        content.Children.Add(detailCard);

        // Source file
        if (!string.IsNullOrEmpty(server.SourcePath))
        {
            content.Children.Add(new TextBlock
            {
                Text = "Source File",
                FontSize = 13,
                FontWeight = FontWeights.Bold,
                Foreground = TextDark,
                Margin = new Thickness(0, 16, 0, 12),
            });
            content.Children.Add(this.CreateConfigFileCard("CONFIG FILE", server.SourcePath));
        }

        this.DetailsPanel.Children.Add(content);
    }

    /// <summary>
    /// Renders Skill detail (Stitch style).
    /// </summary>
    private void RenderSkillDetail(Skill skill)
    {
        // Header
        var headerSection = new Border
        {
            Padding = new Thickness(32, 28, 32, 24),
            BorderBrush = CardBorder,
            BorderThickness = new Thickness(0, 0, 0, 1),
        };

        var headerGrid = new Grid();
        headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        var avatar = new Border
        {
            Width = 72, Height = 72,
            CornerRadius = new CornerRadius(16),
            Background = PrimarySubtle,
            BorderBrush = PrimaryBorder,
            BorderThickness = new Thickness(1),
            Margin = new Thickness(0, 0, 20, 0),
        };
        avatar.Child = new TextBlock
        {
            Text = "🧩",
            FontSize = 36,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };
        Grid.SetColumn(avatar, 0);
        headerGrid.Children.Add(avatar);

        var info = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
        info.Children.Add(new TextBlock
        {
            Text = skill.Name,
            FontSize = 26,
            FontWeight = FontWeights.Bold,
            Foreground = TextDark,
            Margin = new Thickness(0, 0, 0, 4),
        });
        if (!string.IsNullOrEmpty(skill.Description))
        {
            info.Children.Add(new TextBlock
            {
                Text = skill.Description,
                FontSize = 12.5,
                Foreground = TextLight,
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 420,
                LineHeight = 18,
            });
        }

        Grid.SetColumn(info, 1);
        headerGrid.Children.Add(info);
        headerSection.Child = headerGrid;
        this.DetailsPanel.Children.Add(headerSection);

        // Details
        var content = new StackPanel { Margin = new Thickness(32, 24, 32, 0) };

        // Usage stats
        if (this.currentEditor != null)
        {
            var usage = this.FindSkillUsage(skill.Name);
            if (usage != null)
            {
                content.Children.Add(new TextBlock
                {
                    Text = "Usage Statistics",
                    FontSize = 13,
                    FontWeight = FontWeights.Bold,
                    Foreground = TextDark,
                    Margin = new Thickness(0, 0, 0, 12),
                });

                var statsGrid = new Grid();
                statsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                statsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(8) });
                statsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                var usageCard = this.CreateStatCard("USAGE COUNT", usage.UsageCount.ToString(), "times", Primary);
                Grid.SetColumn(usageCard, 0);
                statsGrid.Children.Add(usageCard);

                var lastCard = this.CreateStatCard("LAST USED", usage.LastUsed, string.Empty, TextLight);
                Grid.SetColumn(lastCard, 2);
                statsGrid.Children.Add(lastCard);

                content.Children.Add(statsGrid);
            }
        }

        // Source file
        if (!string.IsNullOrEmpty(skill.SourcePath))
        {
            content.Children.Add(new TextBlock
            {
                Text = "Source Location",
                FontSize = 13,
                FontWeight = FontWeights.Bold,
                Foreground = TextDark,
                Margin = new Thickness(0, 16, 0, 12),
            });
            content.Children.Add(this.CreateConfigFileCard("SKILL FILE", skill.SourcePath));
        }

        this.DetailsPanel.Children.Add(content);
    }

    private SkillUsage? FindSkillUsage(string skillName)
    {
        if (this.currentEditor == null)
        {
            return null;
        }

        foreach (var u in this.currentEditor.SkillUsage)
        {
            if (u.Name == skillName)
            {
                return u;
            }
        }

        return null;
    }

    // ── Reusable UI Components ──────────────────────────────────────

    /// <summary>
    /// Creates a stat card (like the Stitch 3-column stats).
    /// </summary>
    private Border CreateStatCard(string label, string value, string sublabel, SolidColorBrush subColor)
    {
        var card = new Border
        {
            Background = CardBg,
            BorderBrush = CardBorder,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(16, 14, 16, 14),
        };

        var panel = new StackPanel();
        panel.Children.Add(new TextBlock
        {
            Text = label,
            FontSize = 10,
            FontWeight = FontWeights.Bold,
            Foreground = TextLight,
            Margin = new Thickness(0, 0, 0, 4),
        });

        var valueRow = new StackPanel { Orientation = Orientation.Horizontal };
        valueRow.Children.Add(new TextBlock
        {
            Text = value,
            FontSize = 22,
            FontWeight = FontWeights.Bold,
            Foreground = TextDark,
            VerticalAlignment = VerticalAlignment.Bottom,
        });
        if (!string.IsNullOrEmpty(sublabel))
        {
            valueRow.Children.Add(new TextBlock
            {
                Text = sublabel,
                FontSize = 11.5,
                Foreground = subColor,
                FontWeight = FontWeights.Medium,
                Margin = new Thickness(8, 0, 0, 2),
                VerticalAlignment = VerticalAlignment.Bottom,
            });
        }

        panel.Children.Add(valueRow);
        card.Child = panel;
        return card;
    }

    /// <summary>
    /// Creates a config file card (Stitch style file row with open icon).
    /// </summary>
    private Border CreateConfigFileCard(string label, string filePath)
    {
        var card = new Border
        {
            Background = CardBg,
            BorderBrush = CardBorder,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(16, 14, 16, 14),
            Margin = new Thickness(0, 0, 0, 8),
            Cursor = Cursors.Hand,
            Tag = filePath,
        };

        card.MouseEnter += (s, e) =>
        {
            if (s is Border b)
            {
                b.BorderBrush = B("#A5B4FC");
            }
        };
        card.MouseLeave += (s, e) =>
        {
            if (s is Border b)
            {
                b.BorderBrush = CardBorder;
            }
        };
        card.MouseLeftButtonDown += this.OnFilePathClicked;

        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var textPanel = new StackPanel();
        textPanel.Children.Add(new TextBlock
        {
            Text = label,
            FontSize = 10,
            FontWeight = FontWeights.Bold,
            Foreground = TextLight,
            Margin = new Thickness(0, 0, 0, 3),
        });
        textPanel.Children.Add(new TextBlock
        {
            Text = filePath,
            FontSize = 12.5,
            Foreground = TextDark,
            TextTrimming = TextTrimming.CharacterEllipsis,
            FontFamily = new FontFamily("Consolas, Microsoft YaHei UI"),
        });

        var openIcon = new TextBlock
        {
            Text = "📂",
            FontSize = 16,
            Foreground = TextLight,
            VerticalAlignment = VerticalAlignment.Center,
            ToolTip = "Ctrl + 点击打开文件",
        };

        Grid.SetColumn(textPanel, 0);
        Grid.SetColumn(openIcon, 1);
        grid.Children.Add(textPanel);
        grid.Children.Add(openIcon);

        card.Child = grid;
        return card;
    }

    /// <summary>
    /// Creates a general info card container.
    /// </summary>
    private Border CreateInfoCard()
    {
        var card = new Border
        {
            Background = CardBg,
            BorderBrush = CardBorder,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(18, 12, 18, 12),
        };
        card.Child = new StackPanel();
        return card;
    }

    /// <summary>
    /// Creates a key-value row inside a card.
    /// </summary>
    private Grid Kvp(string key, string value)
    {
        var g = new Grid { Margin = new Thickness(0, 6, 0, 6) };
        g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(110) });
        g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        var k = new TextBlock
        {
            Text = key,
            FontSize = 12.5,
            FontWeight = FontWeights.SemiBold,
            Foreground = TextLight,
            VerticalAlignment = VerticalAlignment.Top,
        };

        var v = new TextBlock
        {
            Text = value,
            FontSize = 12.5,
            Foreground = TextDark,
            TextWrapping = TextWrapping.Wrap,
            FontFamily = new FontFamily("Consolas, Microsoft YaHei UI"),
        };

        Grid.SetColumn(k, 0);
        Grid.SetColumn(v, 1);
        g.Children.Add(k);
        g.Children.Add(v);
        return g;
    }

    /// <summary>
    /// Creates a thin separator line.
    /// </summary>
    private Border Sep()
    {
        return new Border
        {
            Height = 1,
            Background = B("#F1F5F9"),
            Margin = new Thickness(0, 3, 0, 3),
        };
    }

    /// <summary>
    /// Handles Ctrl+Click on a file path card.
    /// </summary>
    private void OnFilePathClicked(object sender, MouseButtonEventArgs e)
    {
        if (Keyboard.Modifiers != ModifierKeys.Control)
        {
            return;
        }

        string? filePath = null;
        if (sender is Border b)
        {
            filePath = b.Tag as string;
        }
        else if (sender is TextBlock tb)
        {
            filePath = tb.Tag as string;
        }

        if (string.IsNullOrEmpty(filePath))
        {
            return;
        }

        if (!System.IO.File.Exists(filePath))
        {
            MessageBox.Show($"文件不存在：{filePath}", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            Process.Start("explorer.exe", $"/select,\"{filePath}\"");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"无法打开文件：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}

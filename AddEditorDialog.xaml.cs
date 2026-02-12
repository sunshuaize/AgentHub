// <copyright file="AddEditorDialog.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AgentHub;

using System.Windows;
using System.Windows.Controls;

/// <summary>
/// Dialog for adding a custom AI editor.
/// </summary>
public partial class AddEditorDialog : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AddEditorDialog"/> class.
    /// </summary>
    public AddEditorDialog()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// Gets the editor name entered by the user.
    /// </summary>
    public string EditorName => this.NameBox.Text.Trim();

    /// <summary>
    /// Gets the config file path entered by the user.
    /// </summary>
    public string ConfigFilePath => this.PathBox.Text.Trim();

    /// <summary>
    /// Gets the editor icon entered by the user.
    /// </summary>
    public string EditorIcon => this.IconBox.Text.Trim();

    /// <summary>
    /// Gets the MCP key entered by the user.
    /// </summary>
    public string McpKey
    {
        get
        {
            var text = this.McpKeyBox.Text?.Trim();
            if (!string.IsNullOrEmpty(text))
            {
                return text;
            }

            if (this.McpKeyBox.SelectedItem is ComboBoxItem item)
            {
                return item.Content?.ToString() ?? "mcpServers";
            }

            return "mcpServers";
        }
    }

    private void OnBrowse(object sender, RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
            Title = "选择编辑器配置文件",
        };

        if (dialog.ShowDialog() == true)
        {
            this.PathBox.Text = dialog.FileName;
        }
    }

    private void OnOk(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(this.NameBox.Text))
        {
            MessageBox.Show("请填写编辑器名称", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(this.PathBox.Text))
        {
            MessageBox.Show("请选择配置文件路径", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!System.IO.File.Exists(this.PathBox.Text))
        {
            MessageBox.Show("配置文件不存在", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        this.DialogResult = true;
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        this.DialogResult = false;
    }
}

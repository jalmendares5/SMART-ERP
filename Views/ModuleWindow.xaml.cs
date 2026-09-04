using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SMART_ERP.Views;

public partial class ModuleWindow : Window
{
    public ModuleWindow(string moduleTitle, UserControl moduleView, IEnumerable<string> ribbonOptions)
    {
        InitializeComponent();

        Owner = Application.Current.MainWindow;

        Title = $"{moduleTitle} - SMART ERP";
        TxtModuleTitle.Text = moduleTitle;
        ModuleContent.Content = moduleView;

        BuildRibbon(ribbonOptions);
    }

    private void BuildRibbon(IEnumerable<string> options)
    {
        RibbonPanel.Children.Clear();

        foreach (var option in options)
        {
            var button = new Button
            {
                Content = new StackPanel
                {
                    Width = 105,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = "◼",
                            FontSize = 20,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Foreground = Brushes.RoyalBlue
                        },
                        new TextBlock
                        {
                            Text = option,
                            TextWrapping = TextWrapping.Wrap,
                            TextAlignment = TextAlignment.Center,
                            Margin = new Thickness(0, 3, 0, 0),
                            FontSize = 12,
                            Foreground = Brushes.MidnightBlue
                        }
                    }
                },
                Padding = new Thickness(10, 8, 10, 8),
                Margin = new Thickness(2, 0, 2, 0),
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand
            };

            RibbonPanel.Children.Add(button);
        }
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}


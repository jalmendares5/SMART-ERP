using System.Windows;

namespace SMART_ERP.Views;

public partial class CatalogTextInputWindow : Window
{
    public string Value => TxtValue.Text;

    public string SecondaryValue => TxtSecondaryValue.Text;

    public string TertiaryValue => TxtTertiaryValue.Text;

    public CatalogTextInputWindow(
        string title,
        string label,
        string initialValue = "",
        string secondaryLabel = "",
        string secondaryValue = "",
        string tertiaryLabel = "",
        string tertiaryValue = "")
    {
        InitializeComponent();

        TxtTitle.Text = title;
        TxtLabel.Text = label;
        TxtValue.Text = initialValue;

        if (!string.IsNullOrWhiteSpace(secondaryLabel))
        {
            TxtSecondaryLabel.Text = secondaryLabel;
            TxtSecondaryValue.Text = secondaryValue;

            TxtSecondaryLabel.Visibility = Visibility.Visible;
            TxtSecondaryValue.Visibility = Visibility.Visible;
        }

        if (!string.IsNullOrWhiteSpace(tertiaryLabel))
        {
            TxtTertiaryLabel.Text = tertiaryLabel;
            TxtTertiaryValue.Text = tertiaryValue;

            TxtTertiaryLabel.Visibility = Visibility.Visible;
            TxtTertiaryValue.Visibility = Visibility.Visible;
        }

        Loaded += (_, _) =>
        {
            TxtValue.Focus();
            TxtValue.SelectAll();
        };
    }

    private void BtnSave_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TxtValue.Text))
        {
            MessageBox.Show(
                "Debe ingresar un valor.",
                "SMART ERP",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        DialogResult = true;
        Close();
    }

    private void BtnCancel_Click(
        object sender,
        RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}

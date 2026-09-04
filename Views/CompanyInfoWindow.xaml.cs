using Microsoft.Win32;
using SMART_ERP.Services;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace SMART_ERP.Views;

public partial class CompanyInfoWindow : Window
{
    private string _logoPath = "/Resources/SmartERP.png";

    public CompanyInfoWindow()
    {
                Owner = Application.Current.MainWindow;
InitializeComponent();
        LoadData();
    }

    private void LoadData()
    {
        var data = CompanyInfoService.Current;

        TxtCompanyName.Text = data.CompanyName;
        TxtSlogan.Text = data.Slogan;
        TxtRtn.Text = data.Rtn;
        TxtPhone.Text = data.Phone;
        TxtEmail.Text = data.Email;
        TxtAddress.Text = data.Address;

        _logoPath = string.IsNullOrWhiteSpace(data.LogoPath) ? "/Resources/SmartERP.png" : data.LogoPath;
        LoadLogoPreview(_logoPath);
    }

    private void BtnSelectImage_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Seleccionar logo",
            Filter = "Imágenes|*.png;*.jpg;*.jpeg;*.bmp;*.gif"
        };

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        _logoPath = dialog.FileName;
        LoadLogoPreview(_logoPath);
    }

    private void BtnClearImage_Click(object sender, RoutedEventArgs e)
    {
        _logoPath = "/Resources/SmartERP.png";
        LoadLogoPreview(_logoPath);
    }

    private void LoadLogoPreview(string path)
    {
        try
        {
            ImgLogo.Source = new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute));
        }
        catch
        {
            ImgLogo.Source = new BitmapImage(new Uri("/Resources/SmartERP.png", UriKind.Relative));
            _logoPath = "/Resources/SmartERP.png";
        }
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        var data = CompanyInfoService.Current;

        data.CompanyName = TxtCompanyName.Text.Trim();
        data.Slogan = TxtSlogan.Text.Trim();
        data.Rtn = TxtRtn.Text.Trim();
        data.Phone = TxtPhone.Text.Trim();
        data.Email = TxtEmail.Text.Trim();
        data.Address = TxtAddress.Text.Trim();
        data.LogoPath = _logoPath;

        CompanyInfoService.Save();

        DialogResult = true;
        Close();
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}


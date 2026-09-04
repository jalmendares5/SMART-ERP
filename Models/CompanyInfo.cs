using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SMART_ERP.Models;

public class CompanyInfo : INotifyPropertyChanged
{
    private string _companyName = "TALLER AUTOMOTRIZ DARWIN";
    private string _slogan = "ESPECIALISTAS EN EQUIPO PESADO";
    private string _rtn = "RTN";
    private string _phone = "TEL 99855817";
    private string _email = "TALLERAUTOMOTRIZDARWIN@GMAIL.COM";
    private string _address = "ALDEA EL CASTAÑO, KM 43";
    private string _logoPath = "/Resources/SmartERP.png";

    public event PropertyChangedEventHandler? PropertyChanged;

    public string CompanyName
    {
        get => _companyName;
        set => SetProperty(ref _companyName, value);
    }

    public string Slogan
    {
        get => _slogan;
        set => SetProperty(ref _slogan, value);
    }

    public string Rtn
    {
        get => _rtn;
        set => SetProperty(ref _rtn, value);
    }

    public string Phone
    {
        get => _phone;
        set => SetProperty(ref _phone, value);
    }

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public string Address
    {
        get => _address;
        set => SetProperty(ref _address, value);
    }

    public string LogoPath
    {
        get => _logoPath;
        set => SetProperty(ref _logoPath, value);
    }

    private void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (Equals(field, value))
        {
            return;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

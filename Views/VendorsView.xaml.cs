using SMART_ERP.Models;
using SMART_ERP.Services;
using System.Globalization;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SMART_ERP.Views;

public partial class VendorsView : UserControl
{
    private int _selectedVendorId;

    public VendorsView()
    {
        InitializeComponent();

        LoadVendors();
    }

    private void LoadVendors(string term = "")
    {
        DgVendedores.ItemsSource = VendorService.Search(term);
    }

    private void TxtBuscar_TextChanged(
        object sender,
        TextChangedEventArgs e)
    {
        LoadVendors(TxtBuscar.Text);
    }

    private void DgVendedores_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (DgVendedores.SelectedItem is Vendor vendor)
        {
            _selectedVendorId = vendor.Id;
        }
        else
        {
            _selectedVendorId = 0;
        }
    }

    private void DgVendedores_MouseDoubleClick(
        object sender,
        MouseButtonEventArgs e)
    {
        if (DgVendedores.SelectedItem is Vendor vendor)
        {
            EditVendor(vendor);
        }
    }

    private void BtnNuevo_Click(
        object sender,
        RoutedEventArgs e)
    {
        CreateVendor();
    }

    private void BtnEditar_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (DgVendedores.SelectedItem is not Vendor vendor)
        {
            MessageBox.Show(
                "Selecciona un vendedor para editar.",
                "Validación",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        EditVendor(vendor);
    }

    private void BtnEliminar_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (DgVendedores.SelectedItem is not Vendor vendor)
        {
            MessageBox.Show(
                "Selecciona un vendedor para eliminar.",
                "Validación",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        var result = MessageBox.Show(
            $"¿Deseas eliminar al vendedor '{vendor.Name}'?",
            "Confirmar eliminación",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes)
        {
            return;
        }

        VendorService.Delete(vendor.Id);

        LoadVendors(TxtBuscar.Text);

        DgVendedores.SelectedItem = null;
        _selectedVendorId = 0;

        MessageBox.Show(
            "Vendedor eliminado correctamente.",
            "Éxito",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    // ========================================================
    // API PUBLICA PARA LA BARRA SUPERIOR DEL ERP
    // ========================================================

    public void CreateNewVendor()
    {
        CreateVendor();
    }

    public void EditSelectedVendor()
    {
        if (DgVendedores.SelectedItem is not Vendor vendor)
        {
            MessageBox.Show(
                "Selecciona un vendedor para editar.",
                "Validación",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        EditVendor(vendor);
    }

    public void DeleteSelectedVendor()
    {
        BtnEliminar_Click(
            this,
            new RoutedEventArgs());
    }
    private void CreateVendor()
    {
        var vendor = new Vendor
        {
            Id = 0,
            Code = VendorService.GenerateNextCode(),
            Name = "",
            CommissionPercentage = 3m,
            Phone = "",
            Email = "",
            IdentityNumber = "",
            Address = "",
            EntryDate = DateTime.Today,
            Note = "",
            PhotoPath = "",
            IsActive = true
        };

        var saved = ShowVendorDialog(
            "NUEVO VENDEDOR",
            vendor,
            isNew: true);

        if (!saved)
        {
            return;
        }

        if (!VendorService.Save(vendor))
        {
            ShowVendorSaveError();
            return;
        }

        LoadVendors(TxtBuscar.Text);

        MessageBox.Show(
            $"Vendedor {vendor.Code} guardado correctamente.",
            "Éxito",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private void EditVendor(Vendor original)
    {
        var vendor = new Vendor
        {
            Id = original.Id,
            Code = original.Code,
            Name = original.Name,
            CommissionPercentage = original.CommissionPercentage,
            Phone = original.Phone,
            Email = original.Email,
            IdentityNumber = original.IdentityNumber,
            Address = original.Address,
            EntryDate = original.EntryDate,
            Note = original.Note,
            PhotoPath = original.PhotoPath,
            IsActive = original.IsActive
        };

        var saved = ShowVendorDialog(
            "EDITAR VENDEDOR",
            vendor,
            isNew: false);

        if (!saved)
        {
            return;
        }

        if (!VendorService.Save(vendor))
        {
            ShowVendorSaveError();
            return;
        }

        LoadVendors(TxtBuscar.Text);

        MessageBox.Show(
            $"Vendedor {vendor.Code} actualizado correctamente.",
            "Éxito",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private bool ShowVendorDialog(
        string title,
        Vendor vendor,
        bool isNew)
    {
        var window = new Window
        {
            Title = title,
            Width = 760,
            Height = 610,
            MinWidth = 760,
            MinHeight = 610,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            ResizeMode = ResizeMode.NoResize,
            ShowInTaskbar = false,
            Background = Brushes.White
        };

        if (Application.Current?.MainWindow is Window owner)
        {
            window.Owner = owner;
        }

        var main = new Grid { Margin = new Thickness(22) };
        main.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        main.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        main.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        var titleBlock = new TextBlock
        {
            Text = title,
            FontSize = 22,
            FontWeight = FontWeights.Bold,
            Foreground = new SolidColorBrush(Color.FromRgb(25, 118, 210)),
            Margin = new Thickness(0, 0, 0, 14)
        };
        Grid.SetRow(titleBlock, 0);
        main.Children.Add(titleBlock);

        var content = new Grid();
        content.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        content.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(18) });
        content.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(230) });
        Grid.SetRow(content, 1);
        main.Children.Add(content);

        var left = new Grid();
        for (var i = 0; i < 6; i++)
        {
            left.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        }
        Grid.SetColumn(left, 0);
        content.Children.Add(left);

        StackPanel Field(string label, Control control, Thickness margin)
        {
            var panel = new StackPanel { Margin = margin };
            panel.Children.Add(CreateLabel(label));
            panel.Children.Add(control);
            return panel;
        }

        TextBox TextField(string value, double height = 32) => new()
        {
            Text = value,
            Height = height,
            Padding = new Thickness(9, 6, 9, 6),
            FontSize = 13
        };

        var topGrid = new Grid { Margin = new Thickness(0, 0, 0, 8) };
        topGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150) });
        topGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        var codeBox = TextField(vendor.Code);
        codeBox.IsReadOnly = true;
        codeBox.Background = new SolidColorBrush(Color.FromRgb(245, 245, 245));
        topGrid.Children.Add(Field("Código", codeBox, new Thickness(0)));
        var activeBox = new CheckBox
        {
            Content = "HABILITADO",
            IsChecked = vendor.IsActive,
            FontSize = 13,
            FontWeight = FontWeights.SemiBold,
            VerticalAlignment = VerticalAlignment.Bottom,
            Margin = new Thickness(18, 0, 0, 8)
        };
        Grid.SetColumn(activeBox, 1);
        topGrid.Children.Add(activeBox);
        Grid.SetRow(topGrid, 0);
        left.Children.Add(topGrid);

        var nameBox = TextField(vendor.Name, 34);
        var namePanel = Field("Nombre *", nameBox, new Thickness(0, 0, 0, 8));
        Grid.SetRow(namePanel, 1);
        left.Children.Add(namePanel);

        var contacts = new Grid { Margin = new Thickness(0, 0, 0, 8) };
        contacts.ColumnDefinitions.Add(new ColumnDefinition());
        contacts.ColumnDefinitions.Add(new ColumnDefinition());
        var phoneBox = TextField(vendor.Phone);
        var emailBox = TextField(vendor.Email);
        phoneBox.TextChanged += (_, _) => FormatNumericGroups(phoneBox, 4, 4);
        FormatNumericGroups(phoneBox, 4, 4);
        contacts.Children.Add(Field("Teléfono", phoneBox, new Thickness(0, 0, 8, 0)));
        var emailPanel = Field("Correo", emailBox, new Thickness(8, 0, 0, 0));
        Grid.SetColumn(emailPanel, 1);
        contacts.Children.Add(emailPanel);
        Grid.SetRow(contacts, 2);
        left.Children.Add(contacts);

        var identityRow = new Grid { Margin = new Thickness(0, 0, 0, 8) };
        identityRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        identityRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(140) });
        var identityBox = TextField(vendor.IdentityNumber);
        identityBox.TextChanged += (_, _) => FormatNumericGroups(identityBox, 4, 4, 5);
        FormatNumericGroups(identityBox, 4, 4, 5);
        identityRow.Children.Add(Field("N.º Identidad", identityBox, new Thickness(0, 0, 8, 0)));
        var datePicker = new DatePicker
        {
            SelectedDate = vendor.EntryDate,
            Height = 32,
            Padding = new Thickness(6, 4, 6, 4),
            FontSize = 13
        };
        var datePanel = Field("Fecha de ingreso", datePicker, new Thickness(8, 0, 0, 0));
        Grid.SetColumn(datePanel, 1);
        identityRow.Children.Add(datePanel);
        Grid.SetRow(identityRow, 3);
        left.Children.Add(identityRow);

        var addressBox = TextField(vendor.Address);
        var addressPanel = Field("Dirección", addressBox, new Thickness(0, 0, 0, 8));
        Grid.SetRow(addressPanel, 4);
        left.Children.Add(addressPanel);

        var bottomFields = new Grid();
        bottomFields.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        bottomFields.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(105) });
        var noteBox = new TextBox
        {
            Text = vendor.Note,
            Height = 60,
            Padding = new Thickness(9, 6, 9, 6),
            AcceptsReturn = true,
            TextWrapping = TextWrapping.Wrap,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            FontSize = 13
        };
        bottomFields.Children.Add(Field("Nota", noteBox, new Thickness(0, 0, 8, 0)));
        var commissionBox = TextField(vendor.CommissionPercentage.ToString("0.##", CultureInfo.InvariantCulture));
        var commissionPanel = Field("Comisión %", commissionBox, new Thickness(8, 0, 0, 0));
        Grid.SetColumn(commissionPanel, 1);
        bottomFields.Children.Add(commissionPanel);
        Grid.SetRow(bottomFields, 5);
        left.Children.Add(bottomFields);

        var photoColumn = new StackPanel();
        Grid.SetColumn(photoColumn, 2);
        content.Children.Add(photoColumn);
        photoColumn.Children.Add(CreateLabel("Foto"));
        var photoArea = new Border
        {
            Height = 200,
            BorderBrush = new SolidColorBrush(Color.FromRgb(190, 190, 190)),
            BorderThickness = new Thickness(1),
            Background = new SolidColorBrush(Color.FromRgb(250, 250, 250)),
            Margin = new Thickness(0, 0, 0, 8)
        };
        var photoContent = new StackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        photoContent.Children.Add(new TextBlock
        {
            Text = "\uE77B",
            FontFamily = new FontFamily("Segoe MDL2 Assets"),
            FontSize = 62,
            Foreground = new SolidColorBrush(Color.FromRgb(170, 170, 170)),
            HorizontalAlignment = HorizontalAlignment.Center
        });
        var photoPathText = new TextBlock
        {
            TextAlignment = TextAlignment.Center,
            TextWrapping = TextWrapping.Wrap,
            Foreground = new SolidColorBrush(Color.FromRgb(100, 100, 100)),
            Margin = new Thickness(12, 8, 12, 0)
        };
        photoContent.Children.Add(photoPathText);
        photoArea.Child = photoContent;
        photoColumn.Children.Add(photoArea);

        var photoButtons = new Grid();
        photoButtons.ColumnDefinitions.Add(new ColumnDefinition());
        photoButtons.ColumnDefinitions.Add(new ColumnDefinition());
        photoButtons.ColumnDefinitions.Add(new ColumnDefinition());
        var deletePhotoButton = new Button { Content = "Eliminar", Height = 30, Margin = new Thickness(0, 0, 4, 0) };
        var exportPhotoButton = new Button { Content = "Exportar", Height = 30, Margin = new Thickness(2, 0, 2, 0) };
        var imagePhotoButton = new Button { Content = "Imagen", Height = 30, Margin = new Thickness(4, 0, 0, 0) };
        photoButtons.Children.Add(deletePhotoButton);
        Grid.SetColumn(exportPhotoButton, 1);
        photoButtons.Children.Add(exportPhotoButton);
        Grid.SetColumn(imagePhotoButton, 2);
        photoButtons.Children.Add(imagePhotoButton);
        photoColumn.Children.Add(photoButtons);

        var buttons = new Grid { Margin = new Thickness(0, 14, 0, 0) };
        buttons.ColumnDefinitions.Add(new ColumnDefinition());
        buttons.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        var clearButton = new Button { Content = "Limpiar", Height = 38, Width = 125, HorizontalAlignment = HorizontalAlignment.Left };
        buttons.Children.Add(clearButton);
        var actionButtons = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
        var cancelButton = new Button { Content = "Cancelar", Height = 38, Width = 125, Margin = new Thickness(0, 0, 8, 0) };
        var saveNewButton = new Button { Content = "Guardar y Nuevo", Height = 38, Width = 125, Margin = new Thickness(0, 0, 8, 0) };
        var saveCloseButton = new Button { Content = "Guardar y Cerrar", Height = 38, Width = 125 };
        actionButtons.Children.Add(cancelButton);
        actionButtons.Children.Add(saveNewButton);
        actionButtons.Children.Add(saveCloseButton);
        Grid.SetColumn(actionButtons, 1);
        buttons.Children.Add(actionButtons);
        Grid.SetRow(buttons, 2);
        main.Children.Add(buttons);
        window.Content = main;

        var selectedPhotoPath = vendor.PhotoPath;
        void RefreshPhotoText() => photoPathText.Text = string.IsNullOrWhiteSpace(selectedPhotoPath)
            ? "Sin imagen"
            : Path.GetFileName(selectedPhotoPath);
        RefreshPhotoText();

        bool ApplyValues()
        {
            var name = nameBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show(window, "El nombre del vendedor es obligatorio.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                nameBox.Focus();
                return false;
            }
            if (!decimal.TryParse(commissionBox.Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var commission) || commission < 0 || commission > 100)
            {
                MessageBox.Show(window, "La comisión debe estar entre 0 y 100.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                commissionBox.Focus();
                commissionBox.SelectAll();
                return false;
            }
            vendor.Name = name;
            vendor.CommissionPercentage = commission;
            vendor.Phone = phoneBox.Text.Trim();
            vendor.Email = emailBox.Text.Trim();
            vendor.IdentityNumber = identityBox.Text.Trim();
            vendor.Address = addressBox.Text.Trim();
            vendor.EntryDate = datePicker.SelectedDate?.Date ?? DateTime.Today;
            vendor.Note = noteBox.Text.Trim();
            vendor.PhotoPath = selectedPhotoPath;
            vendor.IsActive = activeBox.IsChecked ?? true;
            return true;
        }

        void ClearFields()
        {
            nameBox.Clear(); phoneBox.Clear(); emailBox.Clear(); identityBox.Clear(); addressBox.Clear();
            noteBox.Clear(); commissionBox.Text = "3"; datePicker.SelectedDate = DateTime.Today;
            activeBox.IsChecked = true; selectedPhotoPath = ""; RefreshPhotoText(); nameBox.Focus();
        }

        imagePhotoButton.Click += (_, _) =>
        {
            var dialog = new OpenFileDialog { Filter = "Imágenes|*.jpg;*.jpeg;*.png;*.bmp;*.gif|Todos los archivos|*.*" };
            if (dialog.ShowDialog(window) == true) { selectedPhotoPath = dialog.FileName; RefreshPhotoText(); }
        };
        deletePhotoButton.Click += (_, _) => { selectedPhotoPath = ""; RefreshPhotoText(); };
        exportPhotoButton.Click += (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(selectedPhotoPath) || !File.Exists(selectedPhotoPath))
            {
                MessageBox.Show(window, "No hay una imagen disponible para exportar.", "Foto", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            var dialog = new SaveFileDialog { FileName = Path.GetFileName(selectedPhotoPath), Filter = "Imágenes|*.jpg;*.jpeg;*.png;*.bmp;*.gif|Todos los archivos|*.*" };
            if (dialog.ShowDialog(window) == true) { File.Copy(selectedPhotoPath, dialog.FileName, true); }
        };

        var saved = false;
        clearButton.Click += (_, _) => ClearFields();
        saveCloseButton.Click += (_, _) => { if (ApplyValues()) { saved = true; window.DialogResult = true; window.Close(); } };
        saveNewButton.Click += (_, _) =>
        {
            if (!ApplyValues()) return;
            if (!VendorService.Save(vendor))
            {
                ShowVendorSaveError();
                return;
            }
            LoadVendors(TxtBuscar.Text);
            vendor.Id = 0;
            vendor.Code = VendorService.GenerateNextCode();
            codeBox.Text = vendor.Code;
            ClearFields();
        };
        cancelButton.Click += (_, _) => { window.DialogResult = false; window.Close(); };
        window.KeyDown += (_, e) => { if (e.Key == Key.Escape) { window.DialogResult = false; window.Close(); } };
        nameBox.Focus();
        window.ShowDialog();
        return saved;
    }

    private static void ShowVendorSaveError()
    {
        MessageBox.Show(
            "No se pudo guardar el vendedor. Configure o active una empresa con conexión a MariaDB e intente nuevamente.",
            "SMART ERP - Vendedores",
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
    }

    private bool ShowVendorDialogLegacy(
        string title,
        Vendor vendor,
        bool isNew)
    {
        var window = new Window
        {
            Title = title,
            Width = 760,
            Height = 500,
            MinWidth = 760,
            MinHeight = 500,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            ResizeMode = ResizeMode.NoResize,
            ShowInTaskbar = false,
            Background = Brushes.White
        };

        if (Application.Current?.MainWindow is Window owner)
        {
            window.Owner = owner;
        }

        // ====================================================
        // CONTENEDOR PRINCIPAL
        // ====================================================

        var main = new Grid
        {
            Margin = new Thickness(22)
        };

        main.RowDefinitions.Add(
            new RowDefinition { Height = GridLength.Auto });

        main.RowDefinitions.Add(
            new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

        main.RowDefinitions.Add(
            new RowDefinition { Height = GridLength.Auto });

        // ====================================================
        // TITULO
        // ====================================================

        var titleBlock = new TextBlock
        {
            Text = title,
            FontSize = 22,
            FontWeight = FontWeights.Bold,
            Foreground = new SolidColorBrush(
                Color.FromRgb(25, 118, 210)),
            Margin = new Thickness(0, 0, 0, 18)
        };

        Grid.SetRow(titleBlock, 0);
        main.Children.Add(titleBlock);

        // ====================================================
        // CONTENIDO
        // ====================================================

        var contentGrid = new Grid
        {
            Margin = new Thickness(0)
        };

        contentGrid.ColumnDefinitions.Add(
            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        contentGrid.ColumnDefinitions.Add(
            new ColumnDefinition { Width = new GridLength(24) });

        contentGrid.ColumnDefinitions.Add(
            new ColumnDefinition { Width = new GridLength(230) });

        contentGrid.RowDefinitions.Add(
            new RowDefinition { Height = GridLength.Auto });

        contentGrid.RowDefinitions.Add(
            new RowDefinition { Height = GridLength.Auto });

        contentGrid.RowDefinitions.Add(
            new RowDefinition { Height = GridLength.Auto });

        contentGrid.RowDefinitions.Add(
            new RowDefinition { Height = GridLength.Auto });

        contentGrid.RowDefinitions.Add(
            new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

        // ====================================================
        // CÓDIGO
        // ====================================================

        var codePanel = new StackPanel
        {
            Margin = new Thickness(0, 0, 0, 12)
        };

        codePanel.Children.Add(
            CreateLabel("Código"));

        var codeBox = new TextBox
        {
            Text = vendor.Code,
            Height = 36,
            Padding = new Thickness(10, 7, 10, 7),
            IsReadOnly = true,
            Background = new SolidColorBrush(
                Color.FromRgb(245, 245, 245)),
            BorderBrush = new SolidColorBrush(
                Color.FromRgb(190, 190, 190))
        };

        codePanel.Children.Add(codeBox);

        Grid.SetRow(codePanel, 0);
        Grid.SetColumn(codePanel, 0);
        contentGrid.Children.Add(codePanel);

        // ====================================================
        // ESTADO
        // ====================================================

        var activePanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            VerticalAlignment = VerticalAlignment.Bottom,
            Margin = new Thickness(0, 0, 0, 18)
        };

        var activeBox = new CheckBox
        {
            IsChecked = vendor.IsActive,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 0, 8, 0),
            FontSize = 14
        };

        var activeLabel = new TextBlock
        {
            Text = "HABILITADO",
            FontSize = 13,
            FontWeight = FontWeights.SemiBold,
            VerticalAlignment = VerticalAlignment.Center,
            Foreground = new SolidColorBrush(
                Color.FromRgb(55, 65, 81))
        };

        activePanel.Children.Add(activeBox);
        activePanel.Children.Add(activeLabel);

        Grid.SetRow(activePanel, 0);
        Grid.SetColumn(activePanel, 2);
        contentGrid.Children.Add(activePanel);

        // ====================================================
        // NOMBRE
        // ====================================================

        var namePanel = new StackPanel
        {
            Margin = new Thickness(0, 0, 0, 12)
        };

        namePanel.Children.Add(
            CreateLabel("Nombre *"));

        var nameBox = new TextBox
        {
            Text = vendor.Name,
            Height = 38,
            Padding = new Thickness(10, 7, 10, 7),
            FontSize = 13
        };

        namePanel.Children.Add(nameBox);

        Grid.SetRow(namePanel, 1);
        Grid.SetColumn(namePanel, 0);
        Grid.SetColumnSpan(namePanel, 3);
        contentGrid.Children.Add(namePanel);

        // ====================================================
        // TELÉFONO
        // ====================================================

        var phonePanel = new StackPanel
        {
            Margin = new Thickness(0, 0, 10, 12)
        };

        phonePanel.Children.Add(
            CreateLabel("Teléfono"));

        var phoneBox = new TextBox
        {
            Text = vendor.Phone,
            Height = 36,
            Padding = new Thickness(10, 7, 10, 7),
            FontSize = 13
        };

        phonePanel.Children.Add(phoneBox);

        Grid.SetRow(phonePanel, 2);
        Grid.SetColumn(phonePanel, 0);
        contentGrid.Children.Add(phonePanel);

        // ====================================================
        // CORREO
        // ====================================================

        var emailPanel = new StackPanel
        {
            Margin = new Thickness(10, 0, 0, 12)
        };

        emailPanel.Children.Add(
            CreateLabel("Correo"));

        var emailBox = new TextBox
        {
            Text = vendor.Email,
            Height = 36,
            Padding = new Thickness(10, 7, 10, 7),
            FontSize = 13
        };

        emailPanel.Children.Add(emailBox);

        Grid.SetRow(emailPanel, 2);
        Grid.SetColumn(emailPanel, 0);
        Grid.SetColumnSpan(emailPanel, 1);

        // Correo se coloca debajo del teléfono,
        // usando el espacio principal restante.
        Grid.SetColumn(emailPanel, 0);
        emailPanel.Margin = new Thickness(0, 0, 10, 12);

        // Reutilizamos una fila interna para teléfono/correo.
        var contactGrid = new Grid();

        contactGrid.ColumnDefinitions.Add(
            new ColumnDefinition());

        contactGrid.ColumnDefinitions.Add(
            new ColumnDefinition());

        Grid.SetRow(contactGrid, 2);
        Grid.SetColumn(contactGrid, 0);
        contentGrid.Children.Add(contactGrid);

        contentGrid.Children.Remove(phonePanel);

        Grid.SetColumn(phonePanel, 0);
        phonePanel.Margin = new Thickness(0, 0, 10, 12);
        contactGrid.Children.Add(phonePanel);

        Grid.SetColumn(emailPanel, 1);
        emailPanel.Margin = new Thickness(10, 0, 0, 12);
        contactGrid.Children.Add(emailPanel);

        // ====================================================
        // COMISIÓN
        // ====================================================

        var commissionPanel = new StackPanel
        {
            Margin = new Thickness(0, 0, 10, 12)
        };

        commissionPanel.Children.Add(
            CreateLabel("Comisión %"));

        var commissionBox = new TextBox
        {
            Text = vendor.CommissionPercentage
                .ToString(
                    "0.##",
                    CultureInfo.InvariantCulture),
            Height = 36,
            Padding = new Thickness(10, 7, 10, 7),
            FontSize = 13
        };

        commissionPanel.Children.Add(commissionBox);

        Grid.SetRow(commissionPanel, 3);
        Grid.SetColumn(commissionPanel, 0);
        contentGrid.Children.Add(commissionPanel);

        // ====================================================
        // FOTO
        // ====================================================

        var photoPanel = new Border
        {
            BorderBrush = new SolidColorBrush(
                Color.FromRgb(190, 190, 190)),
            BorderThickness = new Thickness(1),
            Background = new SolidColorBrush(
                Color.FromRgb(250, 250, 250)),
            Margin = new Thickness(0, 0, 0, 12),
            MinHeight = 210
        };

        var photoContent = new StackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };

        photoContent.Children.Add(
            new TextBlock
            {
                Text = "FOTO",
                FontSize = 13,
                FontWeight = FontWeights.SemiBold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            });

        photoContent.Children.Add(
            new TextBlock
            {
                Text = "\uE77B",
                FontFamily = new System.Windows.Media.FontFamily(
                    "Segoe MDL2 Assets"),
                FontSize = 64,
                Foreground = new SolidColorBrush(
                    Color.FromRgb(170, 170, 170)),
                HorizontalAlignment = HorizontalAlignment.Center
            });

        photoPanel.Child = photoContent;

        Grid.SetRow(photoPanel, 1);
        Grid.SetColumn(photoPanel, 2);
        Grid.SetRowSpan(photoPanel, 4);
        contentGrid.Children.Add(photoPanel);

        // ====================================================
        // AGREGAR CONTENIDO
        // ====================================================

        Grid.SetRow(contentGrid, 1);
        main.Children.Add(contentGrid);

        // ====================================================
        // BOTONES
        // ====================================================

        var buttons = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 12, 0, 0)
        };

        var saveButton = new Button
        {
            Content = isNew ? "Guardar" : "Guardar cambios",
            Padding = new Thickness(22, 9, 22, 9),
            Margin = new Thickness(0, 0, 10, 0),
            MinWidth = 120,
            Height = 40
        };

        var cancelButton = new Button
        {
            Content = "Cancelar",
            Padding = new Thickness(22, 9, 22, 9),
            MinWidth = 105,
            Height = 40
        };

        buttons.Children.Add(saveButton);
        buttons.Children.Add(cancelButton);

        Grid.SetRow(buttons, 2);
        main.Children.Add(buttons);

        window.Content = main;

        // ====================================================
        // GUARDADO
        // ====================================================

        bool saved = false;

        saveButton.Click += (_, _) =>
        {
            var name = nameBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show(
                    window,
                    "El nombre del vendedor es obligatorio.",
                    "Validación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                nameBox.Focus();

                return;
            }

            if (!decimal.TryParse(
                    commissionBox.Text.Trim(),
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out var commission))
            {
                MessageBox.Show(
                    window,
                    "La comisión no es válida.",
                    "Validación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                commissionBox.Focus();
                commissionBox.SelectAll();

                return;
            }

            if (commission < 0 || commission > 100)
            {
                MessageBox.Show(
                    window,
                    "La comisión debe estar entre 0 y 100.",
                    "Validación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                commissionBox.Focus();
                commissionBox.SelectAll();

                return;
            }

            vendor.Name = name;
            vendor.CommissionPercentage = commission;
            vendor.Phone = phoneBox.Text.Trim();
            vendor.Email = emailBox.Text.Trim();
            vendor.IsActive = activeBox.IsChecked ?? true;

            saved = true;

            window.DialogResult = true;
            window.Close();
        };

        cancelButton.Click += (_, _) =>
        {
            saved = false;

            window.DialogResult = false;
            window.Close();
        };

        window.KeyDown += (_, e) =>
        {
            if (e.Key == Key.Escape)
            {
                saved = false;
                window.DialogResult = false;
                window.Close();
            }

            if (e.Key == Key.Enter &&
                Keyboard.FocusedElement is TextBox)
            {
                // No guardar automáticamente con Enter.
                // Evita guardar accidentalmente.
            }
        };

        nameBox.Focus();

        window.ShowDialog();

        return saved;
    }

    private static TextBlock CreateLabel(string text)
    {
        return new TextBlock
        {
            Text = text,
            FontSize = 13,
            FontWeight = FontWeights.SemiBold,
            Margin = new Thickness(0, 0, 0, 5)
        };
    }

    private static void FormatNumericGroups(TextBox textBox, params int[] groupSizes)
    {
        var originalText = textBox.Text;
        var maximumDigits = groupSizes.Sum();
        var digits = new string(
            originalText
                .Where(char.IsDigit)
                .Take(maximumDigits)
                .ToArray());

        var groups = new List<string>();
        var position = 0;

        foreach (var size in groupSizes)
        {
            if (position >= digits.Length)
            {
                break;
            }

            var length = Math.Min(size, digits.Length - position);
            groups.Add(digits.Substring(position, length));
            position += length;
        }

        var formattedText = string.Join("-", groups);

        if (originalText == formattedText)
        {
            return;
        }

        var digitsBeforeCursor = originalText
            .Take(Math.Min(textBox.SelectionStart, originalText.Length))
            .Count(char.IsDigit);

        textBox.Text = formattedText;

        var cursor = 0;
        var countedDigits = 0;

        while (cursor < formattedText.Length && countedDigits < digitsBeforeCursor)
        {
            if (char.IsDigit(formattedText[cursor]))
            {
                countedDigits++;
            }

            cursor++;
        }

        textBox.SelectionStart = cursor;
    }
}





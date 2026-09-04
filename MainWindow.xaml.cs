using SMART_ERP.Services;
using SMART_ERP.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SMART_ERP
{
    public partial class MainWindow : Window
    {
        private sealed class RibbonAction
        {
            public required string Label { get; init; }
            public required Action Execute { get; init; }
        }

        private sealed class ModuleTabMeta
        {
            public required string ModuleKey { get; init; }
            public required List<RibbonAction> RibbonActions { get; init; }
            public required bool IsClosable { get; init; }
        }

        public MainWindow()
        {
            InitializeComponent();

            var currentUser = AuthenticationService.CurrentUser;
            if (currentUser != null)
            {
                UserNameText.Text = currentUser.FullName;
            }

            SetupKeyboardShortcuts();
            ThemeService.Initialize();

            OpenHomeTab();

            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            Loaded -= MainWindow_Loaded;

            bool connected =
                await CompanyConnectionService.InitializeActiveCompanyAsync();

            if (connected)
            {
                GetSalesView()?.RefreshVendors();
            }

            if (!connected &&
                CompanyConnectionService.GetAll().Any(x => x.IsActive))
            {
                MessageBox.Show(
                    "No fue posible conectar autom�ticamente con ninguna empresa configurada.",
                    "SMART ERP - Conexi�n",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void OpenHomeTab()
        {
            AddOrSelectModuleTab(
                moduleKey: "Inicio",
                tabHeader: "Dashboard",
                contentFactory: () => new DashboardView(),
                ribbonActions: BuildDashboardRibbonActions(),
                isClosable: false);
        }

        private void OpenSalesTab()
        {
            AddOrSelectModuleTab(
                moduleKey: "Ventas",
                tabHeader: "Ventas",
                contentFactory: () => new RegisterSaleView(),
                ribbonActions: BuildSalesRibbonActions(),
                isClosable: true);
        }

        private List<RibbonAction> BuildSalesRibbonActions()
        {
            return new List<RibbonAction>
            {
                new() { Label = "Nueva venta", Execute = SalesNew },
                new() { Label = "Guardar", Execute = SalesSave },
                new() { Label = "Cancelar", Execute = SalesCancel },
                new() { Label = "Configuración", Execute = SalesOpenSettings },
                new() { Label = "Listado de ventas", Execute = OpenSalesListTab },
                new() { Label = "Dashboard", Execute = OpenHomeTab }
            };
        }

        private void SalesNew()
        {
            var salesView = GetSalesView();
            if (salesView == null)
            {
                OpenSalesTab();
                salesView = GetSalesView();
            }

            salesView?.StartNewSale();
            SelectTabByKey("Ventas");
        }

        private void SalesSave()
        {
            var salesView = GetSalesView();
            if (salesView == null)
            {
                OpenSalesTab();
                salesView = GetSalesView();
            }

            salesView?.ExecuteSave();
        }

        private void SalesCancel()
        {
            var salesView = GetSalesView();
            if (salesView == null)
            {
                return;
            }

            salesView.ExecuteCancel();
        }

        private void SalesOpenSettings()
        {
            var salesView = GetSalesView();
            if (salesView == null)
            {
                OpenSalesTab();
                salesView = GetSalesView();
            }

            salesView?.OpenSettings();
        }

        private RegisterSaleView? GetSalesView()
        {
            var tab = GetTabByKey("Ventas");
            return tab?.Content as RegisterSaleView;
        }

        private void OpenSalesCaptureSettings()
        {
            var window = new SalesCaptureSettingsWindow
            {
                Owner = this
            };

            if (window.ShowDialog() == true)
            {
                if (GetSalesView() is RegisterSaleView salesView)
                {
                    salesView.StartNewSale();
                }
            }
        }

        private void OpenSalesListTab()
        {
            AddOrSelectModuleTab(
                moduleKey: "ListadoVentas",
                tabHeader: "Listado de ventas",
                contentFactory: () => new SalesListView(),
                ribbonActions: new List<RibbonAction>
                {
                    new() { Label = "Actualizar", Execute = RefreshSalesListTab },
                    new() { Label = "Ir a ventas", Execute = OpenSalesTab },
                    new() { Label = "Dashboard", Execute = OpenHomeTab }
                },
                isClosable: true);
        }

        private void OpenReportsTab()
        {
            AddOrSelectModuleTab(
                moduleKey: "Reportes",
                tabHeader: "Reportes",
                contentFactory: () => new ReportsView(),
                ribbonActions: BuildReportsRibbonActions(),
                isClosable: true);
        }

        private List<RibbonAction> BuildReportsRibbonActions()
        {
            return new List<RibbonAction>
            {
                new() { Label = "Actualizar", Execute = RefreshReportsTab },
                new() { Label = "Limpiar Filtros", Execute = ReportsClearFilters },
                new() { Label = "Ventas por vendedor", Execute = OpenSalesByVendorReportTab },
                new() { Label = "Exportar Excel", Execute = ReportsExportToExcel },
                new() { Label = "Exportar CSV", Execute = ReportsExportToCsv },
                new() { Label = "Dashboard", Execute = OpenHomeTab }
            };
        }

        public void RefreshReportsTab()
        {
            var tab = GetTabByKey("Reportes");
            if (tab?.Content is ReportsView view)
            {
                view.Reload();
            }
        }

        public void ReportsClearFilters()
        {
            var tab = GetTabByKey("Reportes");
            if (tab?.Content is ReportsView view)
            {
                view.ClearFilters();
            }
        }

        public void ReportsExportToExcel()
        {
            var tab = GetTabByKey("Reportes");
            if (tab?.Content is ReportsView view)
            {
                view.ExportToExcel();
            }
        }

        public void ReportsExportToCsv()
        {
            var tab = GetTabByKey("Reportes");
            if (tab?.Content is ReportsView view)
            {
                view.ExportToCsv();
            }
        }

        private void OpenDashboardTab()
        {
            SelectTabByKey("Inicio");
        }

        private List<RibbonAction> BuildDashboardRibbonActions()
        {
            return new List<RibbonAction>
            {
                new() { Label = "Ventas", Execute = OpenSalesTab },
                new() { Label = "Facturación", Execute = OpenInvoiceTab },
                new() { Label = "Compras", Execute = OpenPurchaseTab },
                new() { Label = "Tesorería", Execute = OpenCashTransactionTab },
                new() { Label = "Listado de ventas", Execute = OpenSalesListTab },
                new() { Label = "Reportes", Execute = OpenReportsTab },
                new() { Label = "Clientes", Execute = OpenCustomersTab },
                new() { Label = "Vendedores", Execute = OpenVendorsTab },
                new() { Label = "Productos", Execute = OpenProductsTab },
                new() { Label = "Cuentas por Cobrar", Execute = OpenAccountsReceivableTab },
                new() { Label = "Cuentas por Pagar", Execute = OpenAccountsPayableTab },
                new() { Label = "Cierre mensual", Execute = OpenMonthlyCloseTab },
                new() { Label = "Usuarios", Execute = OpenUsersTab },
                new() { Label = "Datos empresa", Execute = OpenCompanyInfoWindow },
            };
        }

        public void RefreshDashboardTab()
        {
            var tab = GetTabByKey("Inicio");
            if (tab?.Content is DashboardView view)
            {
                view.Reload();
            }
        }

        public void RefreshSalesListTab()
        {
            var tab = GetTabByKey("ListadoVentas");
            if (tab?.Content is SalesListView view)
            {
                view.Reload();
            }
        }

        private void OpenCompaniesTab()
        {
            AddOrSelectModuleTab(
                moduleKey: "Empresas",
                tabHeader: "Empresas",
                contentFactory: () => new CompaniesView(),
                ribbonActions: new List<RibbonAction>
                {
                    new() { Label = "Nueva empresa", Execute = CompaniesNew },
                    new() { Label = "Editar", Execute = CompaniesEdit },
                    new() { Label = "Eliminar", Execute = CompaniesDelete }
                },
                isClosable: true);
        }

        private CompaniesView? GetCompaniesView()
        {
            var tab = GetTabByKey("Empresas");

            if (tab == null)
                return null;

            return tab.Content as CompaniesView;
        }

        private void CompaniesNew()
        {
            var view = GetCompaniesView();

            if (view == null)
            {
                OpenCompaniesTab();
                view = GetCompaniesView();
            }

            if (view != null)
                view.CreateNewCompany();
        }

        private void CompaniesEdit()
        {
            var view = GetCompaniesView();

            if (view != null)
                view.EditSelectedCompany();
        }

        private void CompaniesDelete()
        {
            var view = GetCompaniesView();

            if (view != null)
                view.DeleteSelectedCompany();
        }

        private void OpenCompanyInfoWindow()
        {
            var window = new CompanyInfoWindow
            {
                Owner = this
            };

            window.ShowDialog();
        }

        private void OpenCustomersTab()
        {
            AddOrSelectModuleTab(
                moduleKey: "Clientes",
                tabHeader: "Clientes",
                contentFactory: () => new CustomersView(),
                ribbonActions: new List<RibbonAction>
                {
                    new() { Label = "Nuevo cliente", Execute = CustomersNew },
                    new() { Label = "Guardar", Execute = CustomersSave },
                    new() { Label = "Eliminar", Execute = CustomersDelete },
                    new() { Label = "Dashboard", Execute = OpenHomeTab }
                },
                isClosable: true);
        }

        private CustomersView? GetCustomersView()
        {
            var tab = GetTabByKey("Clientes");
            return tab?.Content as CustomersView;
        }

        private void CustomersNew()
        {
            var view = GetCustomersView();

            if (view == null)
            {
                OpenCustomersTab();
                view = GetCustomersView();
            }

            view?.CreateNewCustomer();
        }

        private void CustomersSave()
        {
            GetCustomersView()?.SaveCurrentCustomer();
        }

        private void CustomersDelete()
        {
            GetCustomersView()?.DeleteSelectedCustomer();
        }

        private void OpenVendorsTab()
        {
            AddOrSelectModuleTab(
                moduleKey: "Vendedores",
                tabHeader: "Vendedores",
                contentFactory: () => new VendorsView(),
                ribbonActions: new List<RibbonAction>
                {
                    new() { Label = "Nuevo vendedor", Execute = VendorsNew },
                    new() { Label = "Editar", Execute = VendorsEdit },
                    new() { Label = "Eliminar", Execute = VendorsDelete },
                    new() { Label = "Dashboard", Execute = OpenHomeTab }
                },
                isClosable: true);
        }

        private VendorsView? GetVendorsView()
        {
            var tab = GetTabByKey("Vendedores");

            if (tab == null)
            {
                return null;
            }

            return tab.Content as VendorsView;
        }

        private void VendorsNew()
        {
            var view = GetVendorsView();

            if (view == null)
            {
                OpenVendorsTab();
                view = GetVendorsView();
            }

            if (view != null)
            {
                view.CreateNewVendor();
            }
        }

        private void VendorsEdit()
        {
            var view = GetVendorsView();

            if (view != null)
            {
                view.EditSelectedVendor();
            }
        }

        private void VendorsDelete()
        {
            var view = GetVendorsView();

            if (view != null)
            {
                view.DeleteSelectedVendor();
            }
        }

        private void AddOrSelectModuleTab(string moduleKey, string tabHeader, Func<UserControl> contentFactory, List<RibbonAction> ribbonActions, bool isClosable)
        {
            var existing = GetTabByKey(moduleKey);
            if (existing != null)
            {
                if (existing.Tag is ModuleTabMeta existingMeta)
                {
                    existingMeta.RibbonActions.Clear();
                    existingMeta.RibbonActions.AddRange(ribbonActions);
                }

                ModuleTabs.SelectedItem = existing;
                UpdateRibbon(ribbonActions);
                return;
            }

            var tab = new TabItem
            {
                Content = contentFactory(),
                Tag = new ModuleTabMeta
                {
                    ModuleKey = moduleKey,
                    RibbonActions = ribbonActions,
                    IsClosable = isClosable
                }
            };

            tab.Header = BuildTabHeader(tab, tabHeader, isClosable);
            ModuleTabs.Items.Add(tab);
            ModuleTabs.SelectedItem = tab;
            UpdateRibbon(ribbonActions);
        }

        private TabItem? GetTabByKey(string moduleKey)
        {
            return ModuleTabs.Items
                .OfType<TabItem>()
                .FirstOrDefault(t => t.Tag is ModuleTabMeta meta && meta.ModuleKey == moduleKey);
        }

        private object BuildTabHeader(TabItem tab, string title, bool isClosable)
        {
            if (!isClosable)
            {
                return new TextBlock { Text = title };
            }

            var panel = new StackPanel { Orientation = Orientation.Horizontal };
            panel.Children.Add(new TextBlock { Text = title, Margin = new Thickness(0, 0, 6, 0) });

            var closeButton = new Button
            {
                Content = "✕",
                Width = 16,
                Height = 16,
                Padding = new Thickness(0),
                Margin = new Thickness(0, -1, 0, 0),
                FontSize = 10,
                Cursor = System.Windows.Input.Cursors.Hand,
                BorderThickness = new Thickness(0),
                Background = System.Windows.Media.Brushes.Transparent
            };

            closeButton.Click += (_, _) => CloseTab(tab);
            panel.Children.Add(closeButton);

            return panel;
        }

        private void CloseTab(TabItem tab)
        {
            if (tab.Tag is not ModuleTabMeta meta || !meta.IsClosable)
            {
                return;
            }

            bool wasSelected = Equals(ModuleTabs.SelectedItem, tab);
            ModuleTabs.Items.Remove(tab);

            if (wasSelected)
            {
                SelectTabByKey("Inicio");
            }
        }

        private void SelectTabByKey(string moduleKey)
        {
            var tab = GetTabByKey(moduleKey);
            if (tab != null)
            {
                ModuleTabs.SelectedItem = tab;
            }
        }

        private void ModuleTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ModuleTabs.SelectedItem is not TabItem tab || tab.Tag is not ModuleTabMeta meta)
            {
                RibbonPanel.Children.Clear();
                return;
            }

            UpdateRibbon(meta.RibbonActions);
        }

        private void UpdateRibbon(IEnumerable<RibbonAction> actions)
        {
            RibbonPanel.Children.Clear();

            var selectedModuleKey = (ModuleTabs.SelectedItem as TabItem)?.Tag is ModuleTabMeta selectedMeta
                ? selectedMeta.ModuleKey
                : string.Empty;

            foreach (var action in actions)
            {
                var isActive = IsRibbonActionActive(action.Label, selectedModuleKey);

                var button = new Button
                {
                    Style = (Style)FindResource("RibbonActionButtonStyle"),
                    BorderBrush = isActive ? System.Windows.Media.Brushes.DodgerBlue : System.Windows.Media.Brushes.Transparent,
                    Content = new StackPanel
                    {
                        Width = 116,
                        Children =
                        {
                            new TextBlock
                            {
                                Text = GetRibbonIcon(action.Label),
                                FontFamily = new System.Windows.Media.FontFamily("Segoe MDL2 Assets"),
                                FontSize = 18,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                Foreground = System.Windows.Media.Brushes.MidnightBlue
                            },
                            new TextBlock
                            {
                                Text = action.Label,
                                TextWrapping = TextWrapping.Wrap,
                                TextAlignment = TextAlignment.Center,
                                Margin = new Thickness(0, 3, 0, 0),
                                FontSize = 13,
                                FontWeight = isActive ? FontWeights.SemiBold : FontWeights.Normal,
                                Foreground = isActive ? System.Windows.Media.Brushes.DodgerBlue : System.Windows.Media.Brushes.MidnightBlue
                            }
                        }
                    }
                };

                button.Click += (_, _) => action.Execute();
                RibbonPanel.Children.Add(button);
            }
        }

        private static bool IsRibbonActionActive(string label, string selectedModuleKey)
        {
            if (string.IsNullOrWhiteSpace(label) || string.IsNullOrWhiteSpace(selectedModuleKey))
            {
                return false;
            }

            return label.Equals(selectedModuleKey, StringComparison.OrdinalIgnoreCase)
                   || (selectedModuleKey == "ListadoVentas" && label.Equals("Listado de ventas", StringComparison.OrdinalIgnoreCase))
                   || (selectedModuleKey == "Inicio" && label.Equals("Dashboard", StringComparison.OrdinalIgnoreCase));
        }

        private static string GetRibbonIcon(string label)
        {
            return label switch
            {
                "Ventas" => "\uE8C7",
                "Listado de ventas" => "\uE9D2",
                "Reportes" => "\uE9D2",
                "Dashboard" => "\uE9D9",
                "M�tricas" => "\uE9D9",
                "Cierre mensual" => "\uE787",
                "Clientes" => "\uE716",
                "Vendedores" => "\uE77B",
                "Usuarios" => "\uE77B",
                "Nueva venta" => "\uE710",
                "Guardar" => "\uE74E",
                "Cancelar" => "\uE711",
                "Configuraci�n" => "\uE713",
                "Actualizar" => "\uE72C",
                "Empresas" => "\uE8F1",
                "Nueva empresa" => "\uE710",
                "Conectar" => "\uE703",
                "Editar" => "\uE70F",
                "Eliminar" => "\uE74D",
                _ => "\uE10F"
            };
        }

        private void OpenUsersTab()
        {
            var usersWindow = new UsersView
            {
                Owner = this
            };
            usersWindow.ShowDialog();
        }

        private void OpenMonthlyCloseTab()
        {
            var monthlyCloseWindow = new MonthlyCloseView
            {
                Owner = this
            };
            monthlyCloseWindow.ShowDialog();
        }

        private void OpenSalesByVendorReportTab()
        {
            var salesByVendorReportWindow = new SalesByVendorReportView
            {
                Owner = this
            };
            salesByVendorReportWindow.ShowDialog();
        }

        private void OpenProductsTab()
        {
            var productsWindow = new ProductsView
            {
                Owner = this
            };
            productsWindow.ShowDialog();
        }

        private void OpenInvoiceTab()
        {
            var invoiceWindow = new InvoiceView
            {
                Owner = this
            };
            invoiceWindow.ShowDialog();
        }

        private void OpenAccountsReceivableTab()
        {
            var accountsReceivableWindow = new AccountsReceivableView
            {
                Owner = this
            };
            accountsReceivableWindow.ShowDialog();
        }

        private void OpenAccountsPayableTab()
        {
            var accountsPayableWindow = new AccountsPayableView
            {
                Owner = this
            };
            accountsPayableWindow.ShowDialog();
        }

        private void OpenPurchaseTab()
        {
            var purchaseWindow = new PurchaseView
            {
                Owner = this
            };
            purchaseWindow.ShowDialog();
        }

        private void OpenCashTransactionTab()
        {
            var cashTransactionWindow = new CashTransactionView
            {
                Owner = this
            };
            cashTransactionWindow.ShowDialog();
        }

        private void SetupKeyboardShortcuts()
        {
            // F5 - Actualizar
            this.KeyDown += (sender, e) =>
            {
                if (e.Key == System.Windows.Input.Key.F5)
                {
                    // Verificar qué pestaña está activa y actualizar
                    if (ModuleTabs.SelectedItem is TabItem tab && tab.Content is not null)
                    {
                        if (tab.Content is ReportsView)
                        {
                            RefreshReportsTab();
                        }
                        else if (tab.Content is DashboardView)
                        {
                            // Actualizar dashboard
                        }
                    }
                }
                // ESC - Cerrar ventana modal si existe
                else if (e.Key == System.Windows.Input.Key.Escape)
                {
                    // No cerrar la ventana principal, solo las modales
                }
                // F1 - Ayuda
                else if (e.Key == System.Windows.Input.Key.F1)
                {
                    System.Windows.MessageBox.Show(
                        "Atajos de teclado:\n" +
                        "F5 - Actualizar vista actual\n" +
                        "ESC - Cerrar ventana modal\n" +
                        "F1 - Mostrar ayuda\n" +
                        "Ctrl+N - Nuevo registro (cuando esté en módulo correspondiente)",
                        "Ayuda - Atajos de Teclado",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Information);
                }
            };
        }

        private void ThemeButton_Click(object sender, RoutedEventArgs e)
        {
            ThemeService.ToggleTheme();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            AuthenticationService.Logout();
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }
    }
}








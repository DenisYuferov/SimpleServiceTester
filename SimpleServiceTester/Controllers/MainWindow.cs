using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

using SimpleServiceTester.DataClasses;

namespace SimpleServiceTester.Controllers
{
    /// <summary>
    /// Controller for MainWindow
    /// </summary>
    public class MainWindow
    {
        /// <summary>
        /// MainWindow data
        /// </summary>
        private readonly DataClasses.MainWindow _mwData;

        #region Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mwData">MainWindow data</param>
        public MainWindow(DataClasses.MainWindow mwData)
        {
            _mwData = mwData;

            InitializeData();
        }
        /// <summary>
        /// Enable or disable user interface
        /// </summary>
        /// <param name="isEnabled"></param>
        public void EnableOrDisableUi(bool isEnabled)
        {
            _mwData.UiEnabled = isEnabled;
        }

        private void InitializeData()
        {
            _mwData.UiEnabled = true;
            _mwData.Title = GetAppName();
            _mwData.FontSize = 14;
            _mwData.MenuSettingsHeader = "Settings";
            _mwData.MenuFontSizeHeader = "Font Size";
            _mwData.MenuHelpHeader = "Help";
            _mwData.MenuLanguageHeader = "Language";
            _mwData.TreeViewItemHeader = "Services";
            _mwData.TreeViewItemIsExpanded = true;
            _mwData.MenuGetMethodsHeader = "Get methods";
            _mwData.MenuDeleteServiceHeader = "Delete service";
            _mwData.MenuAddCopyOfMethodHeader = "Add copy of the method";

            var checkBoxes = new ObservableCollection<CheckBox>();

            for (var i = _mwData.MinFontSize; i <= _mwData.MaxFontSize; i++)
            {
                var cb = new CheckBox { Content = i, Tag = double.Parse(i.ToString()), VerticalContentAlignment = VerticalAlignment.Center };
                cb.Click += CbFontSizeOnClick;

                checkBoxes.Add(cb);
            }
            _mwData.MenuFontSizeItems = checkBoxes;

            var sp = new StackPanel { Orientation = Orientation.Horizontal };
            sp.Children.Add(new TextBlock { Text = "Add service: ", Width = 30 });
            sp.Children.Add(new TextBox { MinWidth = 200, MaxWidth = 500, Width = Double.NaN, HorizontalContentAlignment = HorizontalAlignment.Left });

            var mi = new MenuItem { Header = sp, Tag = "ServicesMenuItem" };
            var binding = new Binding("FontSize") { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
            mi.SetBinding(Control.FontSizeProperty, binding);
            mi.Click += MiServicesOnClick;

            _mwData.TreeViewItemContextMenu = new ContextMenu { Items = { mi } };

            _mwData.TreeViewItems = new ObservableCollection<TreeViewItem>();
        }

        private void AddService(string servicePath)
        {
            var serviceName = Path.GetFileNameWithoutExtension(servicePath);

            var miGetMethods = new MenuItem { Name = "GetMethods" };
            miGetMethods.Click += MiServiceOnClick;
            SetBindings(miGetMethods, nameof(_mwData.MenuGetMethodsHeader));

            var miDeleteService = new MenuItem { Name = "DeleteService" };
            miDeleteService.Click += MiServiceOnClick;
            SetBindings(miDeleteService, nameof(_mwData.MenuDeleteServiceHeader));

            var tvi = new TreeViewItem
            {
                Header = new TextBlock {Text = serviceName, TextDecorations = TextDecorations.Underline},
                IsSelected = true, IsExpanded = true, ToolTip = servicePath, FontWeight = FontWeights.Bold
            };

            tvi.ContextMenu = new ContextMenu { Items = { miGetMethods, miDeleteService }, Tag = tvi };

            if (_mwData.TreeViewItems.FirstOrDefault(t => t.Header is TextBlock tb && tb.Text == serviceName) == null)
                _mwData.TreeViewItems.Add(tvi);
        }

        private void AddMethodsToService(string serviceAddress, TreeViewItem tvi)
        {
            var sp = new Models.ServiceParser(serviceAddress);
            var types = sp.GetTypes();

            foreach (var t in types)
            {
                if (!t.Name.Contains("Client")) continue;

                var type = types.SingleOrDefault(ty => ty.Name == "I" + t.Name.Replace("Client", ""));
                var data = new Tuple<string, string>(serviceAddress, type != null ? type.Name : t.Name);

                foreach (var mi in t.GetMethods())
                {
                    if (mi.DeclaringType != t) continue;

                    var tviMethod = AddNode(tvi, mi, data, types);
                    var pName = mi.ReturnParameter?.Name ?? "";
                    var pTypeName = mi.ReturnParameter?.ParameterType.Name ?? "void";

                    var methodToolTip = GetParamString(pName, pTypeName, true).TrimEnd(',', ' ') + $" {mi.Name}(";

                    tviMethod.ToolTip = methodToolTip.TrimEnd(',', ' ') + ")";

                    CreateMenuForMethod(tviMethod);
                }
            }
        }

        private TreeViewItem AddNode(TreeViewItem tvi, MethodInfo miInfo, Tuple<string, string> data, List<Type> types)
        {
            var fontStyle = FontStyles.Normal;
            var foreGround = new SolidColorBrush(Colors.Black);

            var si = new ServiceInfo
            {
                ServiceUrl = data.Item1, InterfaceName = data.Item2, MethodInfo = miInfo, Types = types
            };

            var tviNode = new TreeViewItem
            {
                Foreground = foreGround,
                Header = miInfo.Name,
                Tag = si,
                ToolTip = miInfo.Name,
                IsExpanded = false,
                FontWeight = FontWeights.Normal,
                ContextMenu = new ContextMenu { Visibility = Visibility.Hidden },
                FontStyle = fontStyle
            };

            var tabControl = new TabControl
            {
                Items = {AddTab(si, null)},
                ContextMenu = new ContextMenu {Visibility = Visibility.Hidden}
            };

            tviNode.Items.Add(tabControl);

            tvi.Items.Add(tviNode);

            return tviNode;
        }

        private TabItem AddTab(ServiceInfo sInfo, TabControl tabControl)
        {
            var tabCount = 0;

            if (tabControl != null) tabCount = tabControl.Items.Count;

            var ti = new TabItem
            {
                Content = new Views.MethodUserControl(this, sInfo),
                Header = sInfo.MethodInfo.Name + $" - {tabCount + 1}"
            };

            ti.MouseUp += TiOnMouseUp;

            return ti;
        }

        private void CreateMenuForMethod(TreeViewItem tvi)
        {
            var miOpenMethod = new MenuItem { Tag = tvi };
            miOpenMethod.Click += MiMethodOnClick;

            SetBindings(miOpenMethod, nameof(_mwData.MenuAddCopyOfMethodHeader));
            
            tvi.ContextMenu = new ContextMenu { Items = { miOpenMethod } };
        }
 
        private string GetAppName()
        {
            var name = "";

            var li = Application.ResourceAssembly.FullName.LastIndexOf(",", StringComparison.Ordinal);

            if (li <= 0) return name;

            name = Application.ResourceAssembly.FullName.Substring(0, li);

            li = name.LastIndexOf(",", StringComparison.Ordinal);

            if (li <= 0) return name;

            name = name.Substring(0, li);

            return name;
        }

        private string GetParamString(string paramName, string paramTypeName, bool stringWithComma)
        {
            return stringWithComma ? $"{paramTypeName} {paramName}, " : $"{paramTypeName}: {paramName}";
        }

        private void SetBindings(MenuItem mi, string headerPropertyName)
        {
            var headerBinding = new Binding(headerPropertyName)
            {
                Mode = BindingMode.OneWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };

            var fsBinding = new Binding(nameof(_mwData.FontSize))
            {
                Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };

            mi.SetBinding(HeaderedItemsControl.HeaderProperty, headerBinding);
            mi.SetBinding(Control.FontSizeProperty, fsBinding);
        }

        #endregion

        #region Event Handlers

        private void MiServicesOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (!(sender is MenuItem mi) || !(mi.Tag is string tag) || tag != "ServicesMenuItem" ||
                !(mi.Header is StackPanel sp) || sp.Children.Count < 2 ||
                !(sp.Children[1] is TextBox tb) || tb.Text.Length == 0) return;

            AddService(tb.Text);
        }

        private void CbFontSizeOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (!(sender is CheckBox cb)) return;

            foreach (var mfsi in _mwData.MenuFontSizeItems)
            {
                mfsi.IsChecked = mfsi.Tag == cb.Tag;
            }

            _mwData.FontSize = (double)cb.Tag;
        }

        private void MiServiceOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (!(sender is MenuItem mi) || !(mi.Parent is ContextMenu cm) || 
                !(cm.Tag is TreeViewItem tvi) || !(tvi.ToolTip is string serviceAddress) ||
                serviceAddress == ""
                ) return;

            if (mi.Name == "GetMethods") AddMethodsToService(serviceAddress, tvi);
            else if (mi.Name == "DeleteService") _mwData.TreeViewItems.Remove(tvi);
        }

        private void MiMethodOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (!(sender is MenuItem mi && mi.Tag is TreeViewItem tvi &&
                  tvi.Items.Count > 0 && tvi.Items[0] is TabControl tc && tvi.Tag is ServiceInfo si)) return;

            var ti = AddTab(si, tc);
            tc.Items.Add(ti);
            tc.SelectedItem = ti;
        }

        private void TiOnMouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (!(sender is TabItem ti && ti.Parent is TabControl tc)) return;

            if (mouseButtonEventArgs.ChangedButton == MouseButton.Middle) tc.Items.Remove(ti);
        }

        #endregion
    }
}

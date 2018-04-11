using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;

namespace SimpleServiceTester.Controllers
{
    public class MethodUserControl
    {
        /// <summary>
        /// MethodUserControl data
        /// </summary>
        private readonly DataClasses.MethodUserControl _mucData;
        /// <summary>
        /// MainWindow controller
        /// </summary>
        private readonly MainWindow _mwController;

        #region Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mucData"></param>
        /// <param name="mwController"></param>
        public MethodUserControl(DataClasses.MethodUserControl mucData, MainWindow mwController)
        {
            _mucData = mucData;
            _mwController = mwController;

            InitializeData();
        }

        public void EnableOrDisableUi()
        {
            _mwController.EnableOrDisableUi(false);
            
            var task = new Task(() => { Thread.Sleep(5000); });
            task.ContinueWith(task1 => _mwController.EnableOrDisableUi(true));
            task.Start();
        }

        private void InitializeData()
        {
            _mucData.MethodName = _mucData.ServiceInfo.MethodInfo.Name;
            _mucData.RequestHeader = "Request:";
            _mucData.ResponseHeader = "Response:";
            _mucData.ParamsHeader = "Params:";

            _mucData.TreeViewItems = new ObservableCollection<TreeViewItem>();

            FormTree();

            ProcessRequest();
        }

        private void FormTree()
        {
            foreach (var pi in _mucData.ServiceInfo.MethodInfo.GetParameters())
            {
                PrepareField(pi.Name, pi.ParameterType.Name, null);
            }
        }

        private void PrepareField(string fieldName, string fieldTypeName, TreeViewItem tvi)
        {
            Type type = null;
            var isArray = fieldTypeName.Contains("[]");

            if (!isArray) type = _mucData.ServiceInfo.Types.FirstOrDefault(p => p.Name == fieldTypeName);

            var isParam = !isArray && type == null;
            var pInfo = new DataClasses.ParamInfo { ParamName = fieldName, ParamTypeName = fieldTypeName };

            if (tvi != null && tvi.Header is CheckBox cb && cb.Content is string header && header.Contains("[]"))
            {
                fieldName = "";
                fieldTypeName += "[0]";
            }

            var paramStr = GetParamString(fieldName, fieldTypeName, false);
            paramStr = isParam ? (paramStr.Contains("]") ? paramStr.Replace(":", "") : paramStr) + " = " : paramStr;

            var tviParam = AddNode(tvi, paramStr, false, isParam, pInfo);

            if (isArray) PrepareField(fieldName, fieldTypeName.Replace("[]", ""), tviParam);

            if (type == null) return;

            foreach (var prop in type.GetProperties())
            {
                if (prop.PropertyType.Name == "ExtensionDataObject") continue;

                var ptName = GetPropertyTypeName(prop.PropertyType.FullName);

                PrepareField(prop.Name, ptName, tviParam);
            }
        }

        private TreeViewItem AddNode(TreeViewItem tvi, string header, bool isMethod, bool isParam, object data)
        {
            var fontStyle = FontStyles.Normal;
            var foreGround = new SolidColorBrush(Colors.Black);
            object headerObj = header;

            if (isMethod) fontStyle = FontStyles.Italic;
            else headerObj = FormNodeHeader(header, isParam);

            var tviNode = new TreeViewItem
            {
                Foreground = foreGround,
                Tag = data,
                Header = headerObj,
                ToolTip = header,
                IsExpanded = true,
                FontWeight = FontWeights.Normal,
                ContextMenu = new ContextMenu { Visibility = Visibility.Hidden },
                FontStyle = fontStyle,
            };

            if (tvi != null) tvi.Items.Add(tviNode);
            else _mucData.TreeViewItems.Add(tviNode);

            return tviNode;
        }

        private object FormNodeHeader(string header, bool isParam)
        {
            var cb = new CheckBox { Content = header, IsChecked = true };
            cb.Click += CbParameterOnClick;

            if (header.Contains("[]")) cb.Foreground = new SolidColorBrush(Colors.Blue);
            else if (!header.Contains("=")) cb.Foreground = new SolidColorBrush(Colors.Green);
            else if (!header.Contains("String") && !header.Contains("?")) cb.IsEnabled = false;

            if (!isParam) return cb;

            var tb = new TextBox { MinWidth = 100, MaxWidth = 500, Width = Double.NaN };
            tb.TextChanged += TbOnTextChanged;
            cb.Tag = tb;

            var panel = new StackPanel { Orientation = Orientation.Horizontal };
            panel.Children.Add(cb);
            panel.Children.Add(tb);

            return panel;
        }

        private string GetParamString(string paramName, string paramTypeName, bool stringWithComma)
        {
            return stringWithComma ? $"{paramTypeName} {paramName}, " : $"{paramTypeName}: {paramName}";
        }

        private string GetPropertyTypeName(string propertyTypeName)
        {
            if (!propertyTypeName.Contains("Nullable")) return propertyTypeName.Replace("System.", "");

            var ind = propertyTypeName.LastIndexOf("[", StringComparison.Ordinal);

            if (ind <= 0) return propertyTypeName;

            propertyTypeName = propertyTypeName.Substring(ind + 1);

            ind = propertyTypeName.IndexOf(",", StringComparison.Ordinal);

            if (ind <= 0) return propertyTypeName;

            return propertyTypeName.Substring(0, ind).Replace("System.", "") + "?";
        }

        private string GenerateSoapRequest()
        {
            const string s = "s", env = "Envelope", b = "Body", arr = "arr", ws = "ws";
            var request = $"<{s}:{env} xmlns:{s}=\"http://schemas.xmlsoap.org/soap/envelope/\" " +
                          $"xmlns:{arr}=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\" " +
                          $"xmlns:{ws} =\"http://schemas.datacontract.org/2004/07/WcfService\">" +
                          $"<{s}:{b}>" +
                          $"<{_mucData.MethodName} xmlns=\"http://tempuri.org/\">";

            foreach (TreeViewItem tvi in _mucData.TreeViewItems)
            {
                ProcessNode(tvi, ref request);
            }

            request += $"</{_mucData.MethodName}></{s}:{b}></{s}:{env}>";

            return request;
        }
        
        private void ProcessNode(TreeViewItem tvi, ref string requestPart)
        {
            if (!(tvi.Tag is DataClasses.ParamInfo pi)) return;

            if (tvi.Header is CheckBox cb && cb.Content is string cont)
            {
                if (cb.IsChecked == null || !cb.IsChecked.Value) return;

                var tag = $"{pi.ParamName}";

                if (cont.Contains("[") && !cont.Contains("[]")) tag = $"ws:{pi.ParamTypeName}";

                requestPart += $"<{tag}>";

                foreach (TreeViewItem item in tvi.Items)
                {
                    ProcessNode(item, ref requestPart);
                }

                requestPart += $"</{tag}>";
            }
            else if (tvi.Header is StackPanel sp && sp.Children[0] is CheckBox cbC && sp.Children[1] is TextBox tbC)
            {
                if (cbC.IsChecked == null || !cbC.IsChecked.Value) return;

                if (tvi.Parent is TreeViewItem tviParent && tviParent.Header is CheckBox cbP &&
                    cbP.Content is string contP && !contP.Contains("[]"))
                {
                    requestPart += $"<ws:{pi.ParamName}>{tbC.Text}</ws:{pi.ParamName}>";
                }
                else if (!cbC.Content.ToString().Contains("["))
                {
                    requestPart += $"<{pi.ParamName}>{tbC.Text}</{pi.ParamName}>";
                }
                else
                { 
                    requestPart += $"<arr:{pi.ParamTypeName}>{tbC.Text}</arr:{pi.ParamTypeName}>";
                }
            }
        }

        private void ProcessRequest()
        {
            _mucData.ResponseText = "";
            _mucData.Request = GenerateSoapRequest();
            _mucData.RequestText = XElement.Parse(_mucData.Request).ToString();
        }

        private void SendSoapPacket()
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("Content-Type", "text/xml;charset=utf-8");
                client.Headers.Add("SOAPAction", $"\"http://tempuri.org/{_mucData.ServiceInfo.InterfaceName}/{_mucData.MethodName}\"");

                var response = client.UploadString(_mucData.ServiceInfo.ServiceUrl, _mucData.Request);

                _mucData.ResponseText = XElement.Parse(response).ToString();
            }
        }

        #endregion

        #region Event Handlers

        private void CbParameterOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (!(sender is CheckBox cb) || !(cb.Tag is TextBox tb)) return;

            tb.IsEnabled = cb.IsChecked ?? false;

            ProcessRequest();
        }

        private void TbOnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            ProcessRequest();
        }

        public void GroupBoxRequestOnMouseDoubleClick(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            try
            {
                SendSoapPacket();
            }
            catch (Exception e)
            {
                _mucData.ResponseText = "";
                MessageBox.Show(e.ToString());
            }
        }

        #endregion
    }
}

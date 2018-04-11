using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;

namespace SimpleServiceTester.DataClasses
{
    public class MethodUserControl : INotifyPropertyChanged
    {
        #region Variables

        private string _paramsHeader, _requestHeader, _responseHeader, _requestText, _responseText;
        private ObservableCollection<TreeViewItem> _treeViewItems;
        private ObservableCollection<TabItem> _tabControlItems;

        public string Request, MethodName;
        public event PropertyChangedEventHandler PropertyChanged;
        public ServiceInfo ServiceInfo;

        #endregion

        #region Properties

        public string ParamsHeader
        {
            get => _paramsHeader;
            set
            {
                _paramsHeader = value;
                OnPropertyChanged(nameof(ParamsHeader));
            }
        }

        public string RequestHeader
        {
            get => _requestHeader;
            set
            {
                _requestHeader = value;
                OnPropertyChanged(nameof(RequestHeader));
            }
        }

        public string ResponseHeader
        {
            get => _responseHeader;
            set
            {
                _responseHeader = value;
                OnPropertyChanged(nameof(RequestHeader));
            }
        }

        public string RequestText
        {
            get => _requestText;
            set
            {
                _requestText = value;
                OnPropertyChanged(nameof(RequestText));
            }
        }

        public string ResponseText
        {
            get => _responseText;
            set
            {
                _responseText = value;
                OnPropertyChanged(nameof(ResponseText));
            }
        }

        public ObservableCollection<TreeViewItem> TreeViewItems
        {
            get => _treeViewItems;
            set
            {
                _treeViewItems = value;
                OnPropertyChanged(nameof(TreeViewItems));
            }
        }

        public ObservableCollection<TabItem> TabControlItems
        {
            get => _tabControlItems;
            set
            {
                _tabControlItems = value;
                OnPropertyChanged(nameof(TabControlItems));
            }
        }

        #endregion

        #region Methods

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}

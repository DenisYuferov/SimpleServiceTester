using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;

namespace SimpleServiceTester.DataClasses
{
    /// <summary>
    /// Data for MainWindow
    /// </summary>
    public class MainWindow : INotifyPropertyChanged
    {
        #region Variables

        public event PropertyChangedEventHandler PropertyChanged;
        public readonly int MinFontSize, MaxFontSize;

        private string _title, _menuSettingsHeader, _menuFontSizeHeader, _menuHelpHeader, _menuLanguageHeader,
                        _menuGetMethodsHeader, _menuDeleteServiceHeader, _menuAddCopyOfMethodHeader,
                        _treeViewItemHeader;  
        private double _fontSize;
        private bool _uiEnabled, _treeViewItemIsExpanded;
        private TabItem _selectedTabItem;
        private ContextMenu _treeViewItemContextMenu;

        private ObservableCollection<TreeViewItem> _treeViewItems;
        private ObservableCollection<CheckBox> _menuFontSizeItems, _menuLanguageItems;

        #endregion

        #region Properties

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public string MenuSettingsHeader
        {
            get => _menuSettingsHeader;
            set
            {
                _menuSettingsHeader = value;
                OnPropertyChanged(nameof(MenuSettingsHeader));
            }
        }

        public string MenuFontSizeHeader
        {
            get => _menuFontSizeHeader;
            set
            {
                _menuFontSizeHeader = value;
                OnPropertyChanged(nameof(MenuFontSizeHeader));
            }
        }

        public string MenuHelpHeader
        {
            get => _menuHelpHeader;
            set
            {
                _menuHelpHeader = value;
                OnPropertyChanged(nameof(MenuHelpHeader));
            }
        }

        public string MenuLanguageHeader
        {
            get => _menuLanguageHeader;
            set
            {
                _menuLanguageHeader = value;
                OnPropertyChanged(nameof(MenuLanguageHeader));
            }
        }

        public string MenuGetMethodsHeader
        {
            get => _menuGetMethodsHeader;
            set
            {
                _menuGetMethodsHeader = value;
                OnPropertyChanged(nameof(MenuGetMethodsHeader));
            } 
        }

        public string MenuDeleteServiceHeader
        {
            get => _menuDeleteServiceHeader;
            set
            {
                _menuDeleteServiceHeader = value;
                OnPropertyChanged(nameof(MenuDeleteServiceHeader));
            }
        }

        public string MenuAddCopyOfMethodHeader
        {
            get => _menuAddCopyOfMethodHeader;
            set
            {
                _menuAddCopyOfMethodHeader = value;
                OnPropertyChanged(nameof(MenuAddCopyOfMethodHeader));
            } 
        }

        public string TreeViewItemHeader
        {
            get => _treeViewItemHeader;
            set
            {
                _treeViewItemHeader = value;
                OnPropertyChanged(nameof(TreeViewItemHeader));
            }
        }

        public bool UiEnabled
        {
            get => _uiEnabled;
            set
            {
                _uiEnabled = value;
                OnPropertyChanged(nameof(UiEnabled));
            }
        }

        public bool TreeViewItemIsExpanded
        {
            get => _treeViewItemIsExpanded;
            set
            {
                _treeViewItemIsExpanded = true;
                OnPropertyChanged(nameof(TreeViewItemIsExpanded));
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

        public ObservableCollection<CheckBox> MenuLanguageItems
        {
            get => _menuLanguageItems;
            set
            {
                _menuLanguageItems = value;
                OnPropertyChanged(nameof(MenuLanguageItems));
            }
        }

        public ObservableCollection<CheckBox> MenuFontSizeItems
        {
            get => _menuFontSizeItems;
            set
            {
                _menuFontSizeItems = value;
                OnPropertyChanged(nameof(MenuFontSizeItems));
            }
        }

        public double FontSize
        {
            get => _fontSize;
            set
            {
                _fontSize = value;
                OnPropertyChanged(nameof(FontSize));
            }
        }

        public TabItem SelectedTabItem
        {
            get => _selectedTabItem;
            set
            {
                _selectedTabItem = value;
                OnPropertyChanged(nameof(SelectedTabItem));
            }
        }

        public ContextMenu TreeViewItemContextMenu
        {
            get => _treeViewItemContextMenu;
            set
            {
                _treeViewItemContextMenu = value;
                OnPropertyChanged(nameof(TreeViewItemContextMenu));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            MinFontSize = 8;
            MaxFontSize = 24;
        }
        /// <summary>
        /// On property changed
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}

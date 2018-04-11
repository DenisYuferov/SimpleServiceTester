namespace SimpleServiceTester.Views
{
    /// <summary>
    /// Logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// MainWindow Controller
        /// </summary>
        private readonly Controllers.MainWindow _mwController;
        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            var mwData = new DataClasses.MainWindow();
            DataContext = mwData;
            _mwController = new Controllers.MainWindow(mwData);
        }
    }
}

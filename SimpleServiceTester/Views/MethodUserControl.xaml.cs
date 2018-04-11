namespace SimpleServiceTester.Views
{
    /// <summary>
    /// Logic for MethodUserControl.xaml
    /// </summary>
    public partial class MethodUserControl
    {
        /// <summary>
        /// MethodUserControl Controller
        /// </summary>
        private readonly Controllers.MethodUserControl _mucController;
        /// <summary>
        /// Constructor
        /// </summary>
        public MethodUserControl(Controllers.MainWindow mwController, DataClasses.ServiceInfo serviceInfo)
        {
            InitializeComponent();

            var mucData = new DataClasses.MethodUserControl { ServiceInfo = serviceInfo };
            DataContext = mucData;
            _mucController = new Controllers.MethodUserControl(mucData, mwController);

            GroupBoxRequest.MouseDoubleClick += _mucController.GroupBoxRequestOnMouseDoubleClick;
        }
    }
}

using xdchat_client_wpf.ViewModels;

namespace xdchat_client_wpf {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {
        public MainWindow() {
            InitializeComponent();
            this.DataContext = new MainWindowVM();
        }
    }
}
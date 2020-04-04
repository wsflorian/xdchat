using System.Windows;

namespace xdchat_client_wpf {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App {
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            XdClient.Instance.Start();
        }

        protected override void OnExit(ExitEventArgs e) {
            XdClient.Instance.Stop();
            base.OnExit(e);
        }
    }
}
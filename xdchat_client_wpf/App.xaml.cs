using System.Windows;
using xdchat_client_wpf.ServerCon;
using XdChatShared.Scheduler;

namespace xdchat_client_wpf {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App {
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            XdScheduler.QueueSyncTask(XdClient.Instance.Start);
        }

        protected override void OnExit(ExitEventArgs e) {
            base.OnExit(e);
            
            XdScheduler.QueueSyncTask(() => {
                XdClient.Instance.Stop();
                
                // Program does not exit if there's still a pending network connection
                // so we need to manually exit after closing the network connection
                XdScheduler.QueueSyncTaskScheduled(() => System.Environment.Exit(0), 500);
            });
        }
    }
}
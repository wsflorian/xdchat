using System.Diagnostics;
using SimpleLogger.Logging;
using SimpleLogger.Logging.Formatters;

namespace xdchat_client_wpf.ServerCon {
    public class TraceLoggerHandler : ILoggerHandler {
        private readonly ILoggerFormatter _formatter = new XdLoggerFormatter();
        
        public void Publish(LogMessage logMessage) {
            Trace.WriteLine(_formatter.ApplyFormat(logMessage));
        }
    }
}
using System.Collections.Generic;
using xdchat_shared.Logger.Impl;

namespace XdChatShared.Logger {
    public class Logger {
        public ILogFormatter Formatter { get; set; } = new DefaultLogFormatter();
        public List<ILogPublisher> Publishers { get; } = new List<ILogPublisher>();
        
        public void Info(string message) => this.Log(XdLogLevel.Info, message);
        public void Warn(string message) => this.Log(XdLogLevel.Warn, message);
        public void Error(string message) => this.Log(XdLogLevel.Error, message);
        
        public void Log(XdLogLevel level, string message) {
            if (Formatter == null) {
                return;
            }
            
            XdLogMessage logMessage = new XdLogMessage(this, level, message);
            Publishers.ForEach(publisher => publisher.Handle(logMessage));
        }
    }
}
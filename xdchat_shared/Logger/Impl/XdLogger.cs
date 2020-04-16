using XdChatShared.Logger;

namespace xdchat_shared.Logger.Impl {
    public static class XdLogger {
        public static readonly XdChatShared.Logger.Logger Instance = new XdChatShared.Logger.Logger();
        
        public static void Info(string message) => Instance.Log(XdLogLevel.Info, message);
        public static void Warn(string message) => Instance.Log(XdLogLevel.Warn, message);
        public static void Error(string message) => Instance.Log(XdLogLevel.Error, message);
        public static void Log(XdLogLevel level, string message) => Instance.Log(XdLogLevel.Error, message);
    }
}
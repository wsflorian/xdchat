using SimpleLogger.Logging;
using SimpleLogger.Logging.Formatters;

namespace XdChatShared.Misc {
    public class XdLoggerFormatter : ILoggerFormatter {
        public string ApplyFormat(LogMessage logMessage) {
            return $"{(object) logMessage.DateTime:dd.MM.yyyy HH:mm:ss}: {(object) logMessage.Level} " +
                   $"[line: {(object) logMessage.LineNumber} {(object) logMessage.CallingClass} ->" +
                   $" {(object) logMessage.CallingMethod}()]: {(object) logMessage.Text}";
        }
    }
}
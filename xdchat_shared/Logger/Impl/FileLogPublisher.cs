using System.IO;
using System.Text;
using XdChatShared.Logger;

namespace xdchat_shared.Logger.Impl {
    public class FileLogPublisher : ILogPublisher {
        private readonly StreamWriter _writer;

        public FileLogPublisher(string filePath) {
            _writer = new StreamWriter(filePath, true, Encoding.UTF8);
        }

        public void Handle(XdLogMessage logMessage) {
            _writer.WriteLine(logMessage.ToString());
            _writer.Flush();
        }
    }
}
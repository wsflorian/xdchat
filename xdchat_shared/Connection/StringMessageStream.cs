using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XdChatShared {
    public class StringMessageStream : IDisposable {
        private readonly Stream stream;

        public StringMessageStream(Stream stream) {
            this.stream = stream;
        }

        public async Task WriteMessage(string message) {
            byte[] stringData = Encoding.UTF8.GetBytes(message);
            byte[] lengthData = BitConverter.GetBytes(stringData.Length);
            
            await stream.WriteAsync(lengthData, 0, lengthData.Length);
            await stream.WriteAsync(stringData, 0, stringData.Length);
            await stream.FlushAsync();
        }
        
        public async Task<string> ReadMessage() {
            byte[] lengthData = await ReadSurely(4);
            uint length = BitConverter.ToUInt32(lengthData, 0);

            return Encoding.UTF8.GetString(await ReadSurely(length));
        }

        private async Task<byte[]> ReadSurely(uint shallRead) {
            byte[] readBuf = new byte[shallRead];
            int totalRead = 0;

            while (totalRead < shallRead) {
                int amountRead = await stream.ReadAsync(readBuf, totalRead, (int) (shallRead - totalRead));
                if (amountRead == 0) {
                    throw new EndOfStreamException();
                }
                
                totalRead += amountRead;
            }

            return readBuf;
        }

        public void Dispose() {
            stream?.Dispose();
        }
    }
}
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XdChatShared.Scheduler;

namespace XdChatShared {
    public class DebugStream : Stream
    {
        public Stream parent;
        public override void Flush()
        {
            parent.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return parent.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return parent.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            parent.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            try
            {
                throw new Exception();
            }
            catch (Exception e)
            {
                Trace.WriteLine("Writing from " + offset + " to " + count + " data: " + ToHex(buffer) + " err:  " + e);
            }
            
            parent.Write(buffer, offset, count);
        }
        
        public static string ToHex(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-","");
        }

        public override bool CanRead => parent.CanRead;
        public override bool CanSeek => parent.CanSeek;
        public override bool CanWrite => parent.CanWrite;
        public override long Length => parent.Length;

        public override long Position
        {
            get => parent.Position;
            set { parent.Position = value; }
        }
    }
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
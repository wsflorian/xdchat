using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using XdChatShared.Packets;
using XdChatShared.Scheduler;

namespace XdChatShared {
    public abstract class XdConnection {
        private BinaryWriter writer;
        private BinaryReader reader;

        public TcpClient Client { get; private set; }
        public string RemoteIp { get; private set; }

        public bool Connected => this.Client != null && this.Client.Connected;

        protected void Initialize(TcpClient client) {
            this.Client = client;
            this.RemoteIp = ((IPEndPoint) this.Client.Client.RemoteEndPoint).Address.ToString();
            this.writer = new BinaryWriter(client.GetStream());

            XdScheduler.Instance.RunAsync("Connection-Thread", RunThread);
        }

        private void RunThread() {
            try {
                this.reader = new BinaryReader(this.Client.GetStream());

                while (this.Client.Connected) {
                    Packet packet = Packet.FromJson(reader.ReadString());
                    XdScheduler.Instance.RunSync(() => { OnPacketReceived(packet); });
                }

                XdScheduler.Instance.RunSync(() => OnDisconnect(null));
            } catch (Exception e) {
                XdScheduler.Instance.RunSync(() => OnDisconnect(e));
            } finally {
                this.Client = DisposeAndNull(this.Client);
                this.reader = DisposeAndNull(this.reader);
                this.writer = DisposeAndNull(this.writer);
            }
        }

        public void End() {
            XdScheduler.Instance.CheckIsSync();
            if (!this.Connected) return;

            this.Client = DisposeAndNull(this.Client);
        }

        private static dynamic DisposeAndNull(IDisposable disposable) {
            disposable?.Dispose();
            return null;
        }

        protected abstract void OnPacketReceived(Packet packet);

        protected abstract void OnDisconnect(Exception ex);

        public void Send(Packet packet) {
            XdScheduler.Instance.CheckIsSync();
            if (this.writer == null) return;

            this.writer.Write(Packet.ToJson(packet));
            this.writer.Flush();
        }
        
        // Format: hostname[:port] (e.g. 2.3.4.5, 1.2.3.4:1234)
        public static bool TryParseEndpoint(string input, ushort defaultPort, out string host, out ushort port) {
            int portIndex = input.IndexOf(':');
            
            if (portIndex == -1) {
                if (!Validation.IsValidHost(input)) {
                    host = null;
                    port = 0;
                    return false;
                }

                host = input;
                port = defaultPort;
                return true;
            }

            string inputHost = input.Substring(0, portIndex);
            string inputPort = input.Substring(portIndex + 1);

            if (!ushort.TryParse(inputPort, out port) || !Validation.IsValidHost(inputHost)) {
                host = null;
                port = 0;
                return false;
            }

            host = inputHost;
            return true;
        }
    }
}
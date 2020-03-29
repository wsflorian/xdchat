using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using XdChatShared.Packets;
using XdChatShared.Scheduler;

namespace XdChatShared {
    public abstract class XdConnection {
        private StringMessageStream messageStream;
        
        public TcpClient Client { get; private set; }
        public string RemoteIp { get; private set; }

        public bool Connected => this.Client != null && this.Client.Connected;

        public virtual void Initialize(TcpClient client) {
            this.Client = client;
            this.RemoteIp = ((IPEndPoint) this.Client.Client.RemoteEndPoint).Address.ToString();
            this.messageStream = new StringMessageStream(client.GetStream());

            XdScheduler.QueueAsyncTask(RunReadTask, true);
        }

        private async void RunReadTask() {
            try {
                while (this.Client.Connected) {
                    Packet packet = Packet.FromJson(await messageStream.ReadMessage());
                    XdScheduler.QueueSyncTask(() => { OnPacketReceived(packet); });
                }

                XdScheduler.QueueSyncTask(() => OnDisconnect(null));
            } catch (Exception e) {
                XdScheduler.QueueSyncTask(() => OnDisconnect(e));
            } finally {
                this.Client = DisposeAndNull(this.Client);
                this.messageStream = DisposeAndNull(this.messageStream);
            }
        }

        public void End() {
            XdScheduler.CheckIsMainThread();
            if (!this.Connected) return;

            this.Client = DisposeAndNull(this.Client);
        }

        private static dynamic DisposeAndNull(IDisposable disposable) {
            disposable?.Dispose();
            return null;
        }

        protected abstract void OnPacketReceived(Packet packet);

        protected abstract void OnDisconnect(Exception ex);

        public async Task Send(Packet packet) {
            XdScheduler.CheckIsMainThread();
            if (this.messageStream == null) return;
            
            await this.messageStream.WriteMessage(Packet.ToJson(packet));
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
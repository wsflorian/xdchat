using System;
using System.Net;
using System.Net.Sockets;
using JetBrains.Annotations;
using XdChatShared.Misc;
using XdChatShared.Packets;
using XdChatShared.Scheduler;

namespace XdChatShared.Connection {
    public abstract class XdConnection {
        private StringMessageStream _messageStream;
        
        [CanBeNull]
        public TcpClient Client { get; private set; }
        
        [CanBeNull]
        public string RemoteIp { get; private set; }

        public bool Connected => this.Client != null && this.Client.Connected;

        public virtual void Initialize(TcpClient client) {
            this.Client = client;
            this.RemoteIp = ((IPEndPoint) client.Client.RemoteEndPoint).Address.ToString();
            this._messageStream = new StringMessageStream(client.GetStream());

            XdScheduler.QueueAsyncTask(RunReadTask, true);
        }

        private void RunReadTask() {
            try {
                while (this.Client != null && this.Client.Connected) {
                    Packet packet = Packet.FromJson(_messageStream.ReadMessage());
                    XdScheduler.QueueSyncTask(() => { OnPacketReceived(packet); });
                }

                XdScheduler.QueueSyncTask(() => OnDisconnect(null));
            } catch (Exception e) {
                XdScheduler.QueueSyncTask(() => OnDisconnect(e));
            } finally {
                this.Client = Helper.DisposeAndNull(this.Client);
                this._messageStream = Helper.DisposeAndNull(this._messageStream);
            }
        }

        public void End() {
            XdScheduler.CheckIsMainThread();
            if (!this.Connected) return;

            this.Client = Helper.DisposeAndNull(this.Client);
        }


        protected abstract void OnPacketReceived([NotNull] Packet packet);

        protected abstract void OnDisconnect([CanBeNull] Exception ex);

        public void Send([NotNull] Packet packet) {
            XdScheduler.CheckIsMainThread();

            _messageStream?.WriteMessage(Packet.ToJson(packet));
        }
        
        // Format: hostname[:port] (e.g. 2.3.4.5, 1.2.3.4:1234)
        public static bool TryParseEndpoint([NotNull] string input, ushort defaultPort, out string host, out ushort port) {
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
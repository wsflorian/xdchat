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

        //private Thread readThread;
        
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
                    XdScheduler.Instance.RunSync(() => {
                        OnPacketReceived(Packet.FromJson(reader.ReadString()));
                    });
                }

                XdScheduler.Instance.RunSync(() => OnDisconnect(null));
            } catch (Exception e) {
                XdScheduler.Instance.RunSync(() => OnDisconnect(null));
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
    }
    
}

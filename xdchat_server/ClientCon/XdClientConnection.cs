using SimpleLogger;
using System;
using System.IO;
using System.Net.Sockets;
using System.Timers;
using xdchat_server.Commands;
using xdchat_server.EventsImpl;
using XdChatShared;
using XdChatShared.Packets;
using XdChatShared.Scheduler;

namespace xdchat_server {
    public class XdClientConnection : XdConnection, ICommandSender {
        private readonly XdServer server;
        private readonly Timer authTimeout;
        private long lastPingSent;
        public Authentication Auth { get; set; }

        public long Ping { get; private set; }

        public XdClientConnection(XdServer server, TcpClient client) {
            Initialize(client);
            
            this.server = server;
            this.authTimeout = XdScheduler.QueueSyncTaskScheduled(HandleTimeout, 2000);

            Logger.Log($"Client connected: {this.RemoteIp}");
        }

        private void HandleTimeout() {
            if (this.Connected) {
                XdScheduler.QueueSyncTask(() => this.Disconnect("Authentication timeout"));
            }
        }
        
        protected override void OnPacketReceived(Packet packet) {
            if (Auth == null && !packet.IsType(typeof(ClientPacketAuth))) {
                this.Disconnect("Authentication required");
                return;
            }
            authTimeout?.Stop();

            if (Auth != null && packet.IsType(typeof(ClientPacketAuth))) {
                this.Disconnect("Already authenticated");
                return;
            }
            
            server.EventEmitter.Emit(new PacketReceivedEvent(this, packet));
        }
        
        public void SendMessage(string text) {
            Send(new ServerPacketChatMessage() { Text = text });
        }

        public string GetName() {
            return this.Auth?.Nickname;
        }

        public void Disconnect(string message) {
            Logger.Log($"Disconnected: {message}");
            Send(new ServerPacketDisconnect() { Text = message });
            base.End();
        }

        protected override void OnDisconnect(Exception ex) {
            if (ex == null) {
                Logger.Log($"Client disconnected: {this.RemoteIp}");
            } else if (ex.GetType() == typeof(EndOfStreamException) || ex.GetType() == typeof(IOException)) {
                Logger.Log($"Client disconnected ({ex.GetType()}): {this.RemoteIp}");
            } else if (ex.GetType() == typeof(ProtocolException)) {
                Logger.Log($"Client disconnected (ProtocolException): {this.RemoteIp} - {ex.Message}");
            } else {
                Logger.Log($"Unknown exception in RunThread: {ex}");
            }

            server.Clients.Remove(this);
            server.SendUserListUpdate(this);
        }

        public void SendPing() {
            this.Send(new ServerPacketPing());
            this.lastPingSent = XdScheduler.CurrentTimeMillis();
        }

        public void ReceivePing() => this.Ping = XdScheduler.CurrentTimeMillis() - this.lastPingSent;
    }
    
}

using SimpleLogger;
using System;
using System.IO;
using System.Net.Sockets;
using XdChatShared;
using XdChatShared.Packets;
using XdChatShared.Scheduler;

namespace xdchat_server {
    class XdClientConnection : XdConnection, ICommandSender {
        private readonly XdServer server;
        private readonly Timeout authTimeout;
        public Authentication Auth { get; private set; }

        public XdClientConnection(XdServer server, TcpClient client) {
            Initialize(client);
            
            this.server = server;
            this.authTimeout = XdScheduler.RunTimeout(HandleTimeout, 2500);
            
            Logger.Log($"Client connected: {this.RemoteIp}");
        }

        private void HandleTimeout() {
            if (this.Connected) {
                XdScheduler.Instance.RunSync(() => this.Disconnect("Authentication timeout"));
            }
        }
        
        protected override void OnPacketReceived(Packet packet) {
            if (Auth == null && !packet.IsType(typeof(ClientPacketAuth))) {
                this.Disconnect("Authentication required");
                return;
            }

            if (Auth != null && packet.IsType(typeof(ClientPacketAuth))) {
                this.Disconnect("Already authenticated");
                return;
            }
            
            Packet.InvokeActionIfType<ClientPacketAuth>(packet, HandleAuthPacket);
            Packet.InvokeActionIfType<ClientPacketChatMessage>(packet, HandleChatPacket);
        }

        private void HandleAuthPacket(ClientPacketAuth packet) {
            this.authTimeout.Cancel();

            if (server.GetClientByNickname(packet.Nickname) != null) {
                this.Disconnect("This nickname is already used");
                return;
            }

            if (server.GetClientByUuid(packet.Uuid) != null) {
                this.Disconnect("You are already connected");
                return;
            }
            
            this.Auth = new Authentication(packet.Nickname, packet.Uuid);
            this.server.SendUserListUpdate(this);

            Logger.Log($"Client authenticated: {this.Auth.Nickname} ({this.Auth.Uuid})");
        }

        private void HandleChatPacket(ClientPacketChatMessage packet) {
            Logger.Log($"<{this.Auth?.Nickname}>: {packet.Text}");
            
            if (packet.Text.StartsWith("/")) {
                server.EmitCommand(this, packet.Text);
            } else {
                server.Broadcast(new ServerPacketChatMessage() {
                    HashedUuid = this.Auth?.HashedUuid,
                    Text = packet.Text
                }, con => con != this);
            }
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
        
    }
    
}

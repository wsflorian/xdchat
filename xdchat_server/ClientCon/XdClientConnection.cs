using System;
using System.IO;
using System.Net.Sockets;
using xdchat_server.Commands;
using xdchat_server.Db;
using xdchat_server.EventsImpl;
using xdchat_server.Server;
using xdchat_shared.Logger.Impl;
using XdChatShared.Connection;
using XdChatShared.Modules;
using XdChatShared.Packets;

namespace xdchat_server.ClientCon {
    public class XdClientConnection : XdConnection, ICommandSender, IExtendable<XdClientConnection> {
        private readonly ModuleHolder<XdClientConnection> _moduleHolder;
        public AuthModule Auth => this.Mod<AuthModule>();

        public XdClientConnection() {
            _moduleHolder = new ModuleHolder<XdClientConnection>(this);
        }

        public override void Initialize(TcpClient client) {
            base.Initialize(client);

            _moduleHolder.RegisterModule<PingModule>();
            _moduleHolder.RegisterModule<ChatModule>();
            _moduleHolder.RegisterModule<AuthModule>();
            
            XdLogger.Info($"Client connected: {this.RemoteIp}");
        }

        protected override void OnPacketReceived(Packet packet) {
            if (!this.Mod<AuthModule>().Authenticated && !packet.IsType(typeof(ClientPacketAuth))) {
                this.Disconnect("Authentication required");
                return;
            }

            if (this.Mod<AuthModule>().Authenticated && packet.IsType(typeof(ClientPacketAuth))) {
                this.Disconnect("Already authenticated");
                return;
            }
            
            XdServer.Instance.EventEmitter.Emit(new PacketReceivedEvent(this, packet));
        }
        
        public void SendMessage(string text) {
            Send(new ServerPacketChatMessage { Text = text });
        }

        public void SendOldMessage(string hashedUuid, DateTime ts, string text) {
            Send(new ServerPacketOldChatMessage {HashedUuid = hashedUuid, Timestamp = ts, Text = text});
        }
        
        public void ClearChat() {
            Send(new ServerPacketChatClear());
        }

        public string GetName() {
            return this.Mod<AuthModule>().Nickname;
        }

        public bool HasPermission(string permission) {
            return XdDatabase.CachedUserRank.Get(this.Auth.Uuid).HasPermission(permission);
        }
        
        public void Disconnect(string message) {
            XdLogger.Info($"Disconnected: {message}");
            Send(new ServerPacketDisconnect { Text = message });
            base.End();
        }

        protected override void OnDisconnect(Exception ex) {
            if (ex == null) {
                XdLogger.Info($"Client disconnected: {this.RemoteIp}");
            } else if (ex.GetType() == typeof(EndOfStreamException) || ex.GetType() == typeof(IOException)) {
                XdLogger.Info($"Client disconnected ({ex.GetType()}): {this.RemoteIp}");
            } else if (ex.GetType() == typeof(ProtocolException)) {
                XdLogger.Info($"Client disconnected (ProtocolException): {this.RemoteIp} - {ex.Message}");
            } else {
                XdLogger.Info($"Unknown exception in RunThread: {ex}");
            }
            
            XdServer.Instance.EventEmitter.Emit(new ClientDisconnectedEvent(this));

            _moduleHolder.UnregisterAll();
            XdServer.Instance.Clients.Remove(this);
            XdServer.Instance.SendUserListUpdate();
        }
        
        public TModule Mod<TModule>() where TModule : Module<XdClientConnection> {
            return _moduleHolder.Mod<TModule>();
        }
    }
    
}

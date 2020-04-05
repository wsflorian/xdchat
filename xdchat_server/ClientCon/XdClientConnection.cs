using System;
using System.IO;
using System.Net.Sockets;
using SimpleLogger;
using xdchat_server.Commands;
using xdchat_server.EventsImpl;
using xdchat_server.Server;
using XdChatShared.Connection;
using XdChatShared.Modules;
using XdChatShared.Packets;

namespace xdchat_server.ClientCon {
    public class XdClientConnection : XdConnection, ICommandSender, IExtendable<XdClientConnection> {
        private readonly ModuleHolder<XdClientConnection> _moduleHolder;

        public XdClientConnection() {
            _moduleHolder = new ModuleHolder<XdClientConnection>(this);
        }

        public override void Initialize(TcpClient client) {
            base.Initialize(client);
            
            _moduleHolder.RegisterModule<PingModule>();
            _moduleHolder.RegisterModule<ChatModule>();
            _moduleHolder.RegisterModule<AuthModule>();
            
            Logger.Log($"Client connected: {this.RemoteIp}");
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
            Send(new ServerPacketChatMessage() { Text = text });
        }

        public string GetName() {
            return this.Mod<AuthModule>().Nickname;
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

            _moduleHolder.UnregisterAll();
            XdServer.Instance.Clients.Remove(this);
            XdServer.Instance.SendUserListUpdate();
        }
        
        public TModule Mod<TModule>() where TModule : Module<XdClientConnection> {
            return _moduleHolder.Mod<TModule>();
        }
    }
    
}

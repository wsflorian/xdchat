using System.Linq;
using System.Security.Cryptography;
using System.Text;
using SimpleLogger;
using xdchat_server.EventsImpl;
using XdChatShared.Events;
using XdChatShared.Modules;
using XdChatShared.Packets;
using XdChatShared.Scheduler;
using Timer = System.Timers.Timer;

namespace xdchat_server.ClientCon {
    public class AuthModule : Module<XdClientConnection>, IEventListener {
        public bool Authenticated => Nickname != null;
        public string Nickname { get; private set; }
        public string Uuid { get; private set; }
        public string HashedUuid { get; private set; }
        
        private Timer _authTimeout;

        public AuthModule(XdClientConnection context) : base(context, XdServer.Instance.EventEmitter) {
        }

        public override void OnModuleEnable() {
            _authTimeout = XdScheduler.QueueSyncTaskScheduled(HandleTimeout, 2000);
        }

        public override void OnModuleDisable() {
            _authTimeout?.Stop();
        }

        [EventHandler(typeof(ClientPacketAuth), true)]
        public void HandleAuthPacket(PacketReceivedEvent ev) {
            ClientPacketAuth packet = (ClientPacketAuth) ev.Packet;
            
            if (XdServer.Instance.GetClientByNickname(packet.Nickname) != null) {
                ev.Client.Disconnect("This nickname is already used");
                return;
            }

            if (XdServer.Instance.GetClientByUuid(packet.Uuid) != null) {
                ev.Client.Disconnect("You are already connected");
                return;
            }

            this.Nickname = packet.Nickname;
            this.Uuid = packet.Uuid;
            this.HashedUuid = Sha256Hash(packet.Uuid);
            _authTimeout?.Stop();

            Logger.Log($"Client authenticated: {this.Nickname} ({this.Uuid})");
            XdServer.Instance.SendUserListUpdate(ev.Client);
        }

        private void HandleTimeout() {
            if (Context.Connected) {
                XdScheduler.QueueSyncTask(() => Context.Disconnect("Authentication timeout"));
            }
        }
        
        public ServerPacketClientList.User ToClientListUser() {
            return new ServerPacketClientList.User(Nickname, HashedUuid);
        }

        private static string Sha256Hash(string value) {
            using (SHA256 hash = SHA256.Create()) {
                return string.Concat(hash
                    .ComputeHash(Encoding.UTF8.GetBytes(value))
                    .Select(item => item.ToString("x2")));
            }
        } 
    }
}
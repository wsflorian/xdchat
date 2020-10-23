using System;
using System.Linq;
using xdchat_server.Db;
using xdchat_server.EventsImpl;
using xdchat_server.Server;
using xdchat_shared.Logger.Impl;
using XdChatShared.Misc;
using XdChatShared.Events;
using XdChatShared.Modules;
using XdChatShared.Packets;
using XdChatShared.Scheduler;
using Timer = System.Timers.Timer;

namespace xdchat_server.ClientCon {
    /* => Authentication <=
     *
     * General
     * => Every nickname can be connected once at a time
     * => Every uuid can be connected once at a time
     * => The first packet every client sends is an auth packet containing Nickname and Uuid
     * => If there's no auth packet within 2 seconds after the connection is established, the user will time out
     *
     * Nickname
     * => Can be freely changed
     *
     * Uuid
     * => Randomly generated for each client
     * => Always stays the same (saved on the client's computer)
     * => Used by the server to uniquely identify a user
     * => Shall not be shared to any other user
     *
     * HashedUuid
     * => A SHA256 hash of a users Uuid
     * => Is used by others to uniquely identify a user
     * => Can be shared to other users
     */
    public class AuthModule : Module<XdClientConnection>, IEventListener {
        public bool Authenticated => Nickname != null;
        public string Nickname { get; private set; }
        public string Uuid { get; private set; }
        public string HashedUuid { get; private set; }

        public int DbSessionId { get; private set; }
        
        private Timer _authTimeout;

        public AuthModule(XdClientConnection context) : base(context, XdServer.Instance) {
        }

        public override void OnModuleEnable() {
            _authTimeout = XdScheduler.QueueSyncTaskScheduled(HandleTimeout, 2000);
        }

        public override void OnModuleDisable() {
            _authTimeout?.Stop();
        }

        [XdEventHandler(typeof(ClientPacketAuth), true)]
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
            this.HashedUuid = Helper.Sha256Hash(packet.Uuid);
            _authTimeout?.Stop();
            
            using (XdDatabase db = XdServer.Instance.Db) {
                DbUser user = DbUser.GetByUuid(db, this.Uuid) ?? DbUser.Create(db, this.Uuid);
                DbUserSession session = DbUserSession.Create(db, new DbUserSession {
                    StartTs = DateTime.Now,
                    Nickname = this.Nickname,
                    User = user,
                    Room = db.Rooms.Single(room => room.IsDefault)
                });
                this.DbSessionId = session.Id;

                db.SaveChanges();
            }

            XdLogger.Info($"Client authenticated: {this.Nickname} ({this.Uuid})");
            XdServer.Instance.SendUserListUpdate();

            XdServer.Instance.EventEmitter.Emit(new ClientReadyEvent(ev.Client));
        }

        [XdEventHandler(null, true)]
        public void HandleDisconnected(ClientDisconnectedEvent ev) {
            if (!this.Authenticated) return;
            
            using (XdDatabase db = XdServer.Instance.Db) {
                DbUserSession.EndSession(db, this.GetDbSession(db));
                db.SaveChanges();
            }
        }

        private void HandleTimeout() {
            if (Context.Connected) {
                XdScheduler.QueueSyncTask(() => Context.Disconnect("Authentication timeout"));
            }
        }
        
        public ServerPacketClientList.User ToClientListUser() {
            return new ServerPacketClientList.User(Nickname, HashedUuid);
        }

        public DbUserSession GetDbSession(XdDatabase db) {
            return DbUserSession.GetById(db, this.DbSessionId);
        }

        public DbUser GetDbUser(XdDatabase db) {
            return DbUser.GetByUuid(db, this.Uuid);
        }
    }
}
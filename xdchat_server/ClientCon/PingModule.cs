using System;
using xdchat_server.EventsImpl;
using xdchat_server.Server;
using XdChatShared.Events;
using XdChatShared.Modules;
using XdChatShared.Packets;
using XdChatShared.Scheduler;
using Timer = System.Timers.Timer;

namespace xdchat_server.ClientCon {
    /* => Pinging <=
     *
     * => The server sends a ping packet every 10 seconds
     * => The client shall immediately respond with a pong packet upon receiving a ping packet
     * => The ping is the time between sending the packet and receiving the answer
     * => When trying to send a ping packet and there's no answer to the previous packet, the user will time out
     */
    public class PingModule : Module<XdClientConnection>, IEventListener {
        public long Ping { get; private set; } = -1;
        
        private Timer _pingTimer;

        private DateTime _lastPingSent, _lastPingReceived;

        public PingModule(XdClientConnection context) : base(context, XdServer.Instance) {}

        public override void OnModuleEnable() {
            _pingTimer = XdScheduler.QueueSyncTaskScheduled(RunPingTask, 10000, true);
            RunPingTask();
        }

        public override void OnModuleDisable() {
            _pingTimer.Stop();
        }
        
        private void RunPingTask() {
            if (_lastPingSent > _lastPingReceived) {
                Context.Disconnect("Timed out");
                return;
            }
            
            Context.Send(new ServerPacketPing());
            this._lastPingSent = DateTime.Now;
        }
        
        [XdEventHandler(typeof(ClientPacketPong), true)]
        public void HandlePongPacket(PacketReceivedEvent _) {
            this.Ping = (long) (DateTime.Now - this._lastPingSent).TotalMilliseconds;
            this._lastPingReceived = DateTime.Now;
        }
    }
}
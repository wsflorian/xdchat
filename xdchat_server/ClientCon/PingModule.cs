﻿using System;
using xdchat_server.EventsImpl;
using xdchat_server.Server;
using XdChatShared.Events;
using XdChatShared.Modules;
using XdChatShared.Packets;
using XdChatShared.Scheduler;
using Timer = System.Timers.Timer;

namespace xdchat_server.ClientCon {
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
        
        [XdChatShared.Events.EventHandler(typeof(ClientPacketPong), true)]
        public void HandlePongPacket(PacketReceivedEvent _) {
            this.Ping = (long) (DateTime.Now - this._lastPingSent).TotalMilliseconds;
            this._lastPingReceived = DateTime.Now;
        }
    }
}
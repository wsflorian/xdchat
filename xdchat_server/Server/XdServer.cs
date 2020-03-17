using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using SimpleLogger;
using xdchat_server.ClientCon;
using xdchat_server.Commands;
using xdchat_server.EventsImpl;
using XdChatShared;
using XdChatShared.Events;
using XdChatShared.Packets;
using XdChatShared.Scheduler;
using Timer = System.Timers.Timer;

namespace xdchat_server {
    public class XdServer {
        public static XdServer Instance { get; } = new XdServer();
        public List<XdClientConnection> Clients { get; } = new List<XdClientConnection>();
        public EventEmitter EventEmitter { get; } = new EventEmitter();
        public ConsoleCommandSender ConsoleCommandSender { get; } = new ConsoleCommandSender();

        private ConsoleHandler consoleHandler;

        private TcpListener serverSocket;

        private Timer pingTimer;

        private XdServer() {
            this.RegisterCommand(new KickCommand());
            this.RegisterCommand(new ListCommand());
            this.RegisterCommand(new WhisperCommand());
            this.RegisterCommand(new StopCommand());
            this.RegisterCommand(new SayCommand());
            
            this.EventEmitter.RegisterListener(new ChatPacketListener());
            this.EventEmitter.RegisterListener(new AuthPacketListener());
            this.EventEmitter.RegisterListener(new PongPacketListener());
        }

        private void RegisterCommand(Command command) {
            this.EventEmitter.RegisterListener(new CommandListener(command));
        }
        
        public void Start() {
            XdScheduler.CheckIsMainThread();
            this.consoleHandler = new ConsoleHandler(HandleConsoleInput);

            if (serverSocket != null)
                throw new InvalidOperationException("Server is already running");

            this.serverSocket = new TcpListener(IPAddress.Parse("127.0.0.1"), Constants.DefaultPort);

            Logger.Log("Starting...");
            try {
                this.serverSocket.Start();
            }
            catch (Exception e) {
                Logger.Log(Logger.Level.Error, $"Cannot start server :( {e}");
                return;
            }
            
            XdScheduler.QueueAsyncTask(RunAcceptTask, true);
            pingTimer = XdScheduler.QueueSyncTaskScheduled(RunPingTask, 15000, true);
        }

        private async Task RunAcceptTask() {
            Logger.Log("Started! :)");
            try {
                while (this.serverSocket != null) {
                    TcpClient tcpClient = await serverSocket.AcceptTcpClientAsync();
                    XdClientConnection client = new XdClientConnection(this, tcpClient);
                    this.Clients.Add(client);
                }
            } catch (SocketException) {
                Logger.Log("Server stopped");
            }
        }

        private void RunPingTask() {
            this.GetAuthenticatedClients().ForEach(client => client.SendPing());
        }

        public void Stop() {
            XdScheduler.CheckIsMainThread();
            
            Logger.Log("Stopping handlers...");
            consoleHandler.Stop();

            Logger.Log("Disconnecting clients...");
            this.Clients.ForEach(client => client.Disconnect("Server has been stopped"));
                
            Logger.Log("Stopping socket...");
            pingTimer.Stop();
            serverSocket.Stop();
            serverSocket = null;
        }
        
        private void HandleConsoleInput(string input) {
            XdScheduler.QueueSyncTask(() => EmitCommand(ConsoleCommandSender, input));
        }

        public void EmitCommand(ICommandSender sender, string commandText) {
            CommandEvent commandEvent = EventEmitter.Emit(new CommandEvent(sender, commandText));
            
            if (!commandEvent.Handled) {
                sender.SendMessage("Command not found");
            }
        }

        public XdClientConnection GetClientByNickname(string nickname) {
            return GetAuthenticatedClients().Find(con =>
                string.Compare(con.Auth.Nickname, nickname, StringComparison.OrdinalIgnoreCase) == 0);
        }

        public XdClientConnection GetClientByUuid(string uuid) {
            return GetAuthenticatedClients().Find(con => con.Auth.Uuid == uuid);
        }

        public List<XdClientConnection> GetAuthenticatedClients() {
            return Clients.FindAll(con => con.Auth != null);
        }

        public void SendUserListUpdate(XdClientConnection sender) {
            this.Broadcast(new ServerPacketClientList() {
                Users = GetAuthenticatedClients()
                    .FindAll(con => con != sender)
                    .ConvertAll(con => con.Auth.ToClientListUser())
            }, con => con.Auth != null);
        }

        public void Broadcast(Packet packet, Predicate<XdClientConnection> predicate) {
            XdScheduler.CheckIsMainThread();
            
            Clients.FindAll(predicate).ForEach(con => con.Send(packet));
        }

    }
}
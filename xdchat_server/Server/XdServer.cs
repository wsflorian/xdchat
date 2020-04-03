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

namespace xdchat_server {
    public class XdServer  {
        public static XdServer Instance { get; } = new XdServer();
        public List<XdClientConnection> Clients { get; } = new List<XdClientConnection>();
        public EventEmitter EventEmitter { get; } = new EventEmitter();
        public ConsoleCommandSender ConsoleCommandSender { get; } = new ConsoleCommandSender();

        private ConsoleHandler _consoleHandler;

        private TcpListener _serverSocket;
        
        private XdServer() {
            this.RegisterCommand(new KickCommand());
            this.RegisterCommand(new ListCommand());
            this.RegisterCommand(new WhisperCommand());
            this.RegisterCommand(new StopCommand());
            this.RegisterCommand(new SayCommand());
            this.RegisterCommand(new PingCommand());
        }

        private void RegisterCommand(Command command) {
            this.EventEmitter.RegisterListener(new CommandListener(command));
        }
        
        public void Start() {
            XdScheduler.CheckIsMainThread();
            this._consoleHandler = new ConsoleHandler(HandleConsoleInput);

            if (_serverSocket != null)
                throw new InvalidOperationException("Server is already running");

            this._serverSocket = new TcpListener(IPAddress.Parse("127.0.0.1"), Constants.DefaultPort);

            Logger.Log("Starting...");
            try {
                this._serverSocket.Start();
            }
            catch (Exception e) {
                Logger.Log(Logger.Level.Error, $"Cannot start server :( {e}");
                return;
            }
            
            XdScheduler.QueueAsyncTask(RunAcceptTask, true);
        }

        private async Task RunAcceptTask() {
            Logger.Log("Started! :)");
            try {
                while (this._serverSocket != null) {
                    TcpClient tcpClient = await _serverSocket.AcceptTcpClientAsync();
                    XdScheduler.QueueSyncTask(() => {
                        XdClientConnection client = new XdClientConnection();
                        client.Initialize(tcpClient);
                        this.Clients.Add(client);
                    });
                }
            } catch (SocketException) {
                Logger.Log("Server stopped");
            }
        }
        
        public void Stop() {
            XdScheduler.CheckIsMainThread();
            
            Logger.Log("Stopping handlers...");
            _consoleHandler.Stop();

            Logger.Log("Disconnecting clients...");
            this.Clients.ForEach(client => client.Disconnect("Server has been stopped"));
                
            Logger.Log("Stopping socket...");

            _serverSocket.Stop();
            _serverSocket = null;
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
                string.Compare(con.Mod<AuthModule>().Nickname, nickname, StringComparison.OrdinalIgnoreCase) == 0);
        }

        public XdClientConnection GetClientByUuid(string uuid) {
            return GetAuthenticatedClients().Find(con => con.Mod<AuthModule>().Uuid == uuid);
        }

        public List<XdClientConnection> GetAuthenticatedClients() {
            return Clients.FindAll(con => con.Mod<AuthModule>().Authenticated);
        }

        public void SendUserListUpdate(XdClientConnection sender) {
            this.Broadcast(new ServerPacketClientList() {
                Users = GetAuthenticatedClients()
                    .FindAll(con => con != sender)
                    .ConvertAll(con => con.Mod<AuthModule>().ToClientListUser())
            }, con => con.Mod<AuthModule>().Authenticated);
        }

        public void Broadcast(Packet packet, Predicate<XdClientConnection> predicate) {
            XdScheduler.CheckIsMainThread();
            
            Clients.FindAll(predicate).ForEach(con => con.Send(packet));
        }
    }
}
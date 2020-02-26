using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using SimpleLogger;
using xdchat_server.Commands;
using xdchat_server.Events;
using XdChatShared.Packets;
using XdChatShared.Scheduler;

namespace xdchat_server {
    public class XdServer {
        public static XdServer Instance { get; } = new XdServer();
        public List<XdClientConnection> Clients { get; } = new List<XdClientConnection>();
        public EventEmitter EventEmitter { get; } = new EventEmitter();
        public ConsoleCommandSender ConsoleCommandSender { get; } = new ConsoleCommandSender();

        private ConsoleHandler consoleHandler;

        private TcpListener serverSocket;

        private XdServer() {
            EventEmitter.RegisterListener(new KickCommand());
            EventEmitter.RegisterListener(new ListCommand());
            EventEmitter.RegisterListener(new WhisperCommand());
            EventEmitter.RegisterListener(new StopCommand());
        }

        public void Start() {
            XdScheduler.Instance.CheckIsSync();
            this.consoleHandler = new ConsoleHandler(HandleConsoleInput);

            if (serverSocket != null)
                throw new InvalidOperationException("Server is already running");

            this.serverSocket = new TcpListener(IPAddress.Parse("127.0.0.1"), 10000);

            Logger.Log("Starting...");
            try {
                this.serverSocket.Start();
            }
            catch (Exception e) {
                Logger.Log(Logger.Level.Error, $"Cannot start server :( {e}");
                return;
            }
            
            XdScheduler.Instance.RunAsync("Accept-Thread", () => {
                Logger.Log("Started! :)");
                try {
                    while (this.serverSocket != null) {
                        XdClientConnection client = new XdClientConnection(this, serverSocket.AcceptTcpClient());
                        this.Clients.Add(client);
                    }
                } catch (SocketException) {
                    Logger.Log("Server stopped");
                }
            });
        }
        
        public void Stop() {
            XdScheduler.Instance.CheckIsSync();
            
            Logger.Log("Stopping handlers...");
            consoleHandler.Stop();

            Logger.Log("Disconnecting clients...");
            this.Clients.ForEach(client => client.Disconnect("Server has been stopped"));
                
            Logger.Log("Stopping socket...");
            serverSocket.Stop();
            serverSocket = null;
        }
        
        private void HandleConsoleInput(string input) {
            XdScheduler.Instance.RunSync(() => EmitCommand(ConsoleCommandSender, input));
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
            XdScheduler.Instance.CheckIsSync();
            
            Clients.FindAll(predicate).ForEach(con => con.Send(packet));
        }

    }
}
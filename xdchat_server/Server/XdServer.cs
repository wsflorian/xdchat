using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using SimpleLogger;
using xdchat_server.ClientCon;
using XdChatShared;
using XdChatShared.Modules;
using XdChatShared.Packets;
using XdChatShared.Scheduler;

namespace xdchat_server {
    public class XdServer : XdService, IExtendable<XdServer> {
        public static XdServer Instance { get; } = new XdServer();

        private readonly ModuleHolder<XdServer> _moduleHolder;
        
        public List<XdClientConnection> Clients { get; } = new List<XdClientConnection>();
        
        private ConsoleHandler _consoleHandler;

        private TcpListener _serverSocket;
        
        private XdServer() {
            _moduleHolder = new ModuleHolder<XdServer>(this);
        }

        public override void Start() {
            XdScheduler.CheckIsMainThread();
            this._consoleHandler = new ConsoleHandler();
            this._moduleHolder.RegisterModule<CommandModule>();

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
        
        public override void Stop() {
            XdScheduler.CheckIsMainThread();

            Logger.Log("Stopping handlers...");
            _consoleHandler.Stop();

            Logger.Log("Disconnecting clients...");
            this.Clients.ForEach(client => client.Disconnect("Server has been stopped"));
                
            Logger.Log("Stopping socket...");
            
            _serverSocket.Stop();
            _serverSocket = null;
            
            this._moduleHolder.UnregisterAll();
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

        public TModule Mod<TModule>() where TModule : Module<XdServer> {
            return _moduleHolder.Mod<TModule>();
        }
    }
}
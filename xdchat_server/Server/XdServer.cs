using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using xdchat_server.ClientCon;
using xdchat_server.Db;
using xdchat_shared.Logger.Impl;
using XdChatShared;
using XdChatShared.Misc;
using XdChatShared.Modules;
using XdChatShared.Packets;
using XdChatShared.Scheduler;

namespace xdchat_server.Server {
    public class XdServer : XdService, IExtendable<XdServer> {
        public static XdServer Instance { get; } = new XdServer();

        private readonly ModuleHolder<XdServer> _moduleHolder;
        
        public List<XdClientConnection> Clients { get; } = new List<XdClientConnection>();
        
        public ServerConfig Config { get; private set; }
        
        private ConsoleHandler _consoleHandler;

        private TcpListener _serverSocket;
        
        public XdDatabase Db => new XdDatabase(new DbContextOptionsBuilder()
            .UseMySql(Config.SqlConnection)
            .Options);

        private XdServer() {
            _moduleHolder = new ModuleHolder<XdServer>(this);
        }

        public override void Start() {
            XdScheduler.CheckIsMainThread();
            
            this._consoleHandler = new ConsoleHandler();
            
            this.Config = ServerConfig.load();
            if (this.Config == null) {
                ServerConfig.create();
                XdLogger.Info("config.json was created. Please configure it and restart the server");
                Environment.Exit(1);
                return;
            }

            XdLogger.Info("Checking database...");
            using (XdDatabase db = this.Db) {
                db.Database.EnsureCreated();
            }
            
            this._moduleHolder.RegisterModule<CommandModule>();

            if (_serverSocket != null)
                throw new InvalidOperationException("Server is already running");

            this._serverSocket = new TcpListener(IPAddress.Parse("127.0.0.1"), Helper.DefaultPort);

            XdLogger.Info("Starting server...");
            try {
                this._serverSocket.Start();
            }
            catch (Exception e) {
                XdLogger.Error($"Cannot start server :( {e}");
                return;
            }
            
            XdScheduler.QueueAsyncTask(RunAcceptTask, true);
        }

        private async Task RunAcceptTask() {
            XdLogger.Info("Started! :)");
            try {
                while (this._serverSocket != null) {
                    TcpClient tcpClient;
                    try {
                        tcpClient = await _serverSocket.AcceptTcpClientAsync();
                    } catch (ObjectDisposedException) {
                        break; // Server stopped
                    }
                    XdScheduler.QueueSyncTask(() => {
                        XdClientConnection client = new XdClientConnection();
                        client.Initialize(tcpClient);
                        this.Clients.Add(client);
                    });
                }
            } catch (SocketException) {
                XdLogger.Info("Server stopped");
            }
        }
        
        public override void Stop() {
            XdScheduler.CheckIsMainThread();

            XdLogger.Info("Stopping handlers...");
            _consoleHandler.Stop();

            XdLogger.Info("Disconnecting clients...");
            this.Clients.ForEach(client => client.Disconnect("Server has been stopped"));
                
            XdLogger.Info("Stopping socket...");
            
            _serverSocket.Stop();
            _serverSocket = null;
            
            this._moduleHolder.UnregisterAll();
            
            XdLogger.Info("Exiting...");
            Environment.Exit(0);
        }

        public XdClientConnection GetClientByNickname(string nickname) {
            return GetAuthenticatedClients().FirstOrDefault(con =>
                string.Compare(con.Mod<AuthModule>().Nickname, nickname, StringComparison.OrdinalIgnoreCase) == 0);
        }

        public XdClientConnection GetClientByUuid(string uuid) {
            return GetAuthenticatedClients().FirstOrDefault(con => con.Mod<AuthModule>().Uuid == uuid);
        }

        public List<XdClientConnection> GetAuthenticatedClients() {
            return Clients.FindAll(con => con.Mod<AuthModule>().Authenticated);
        }

        public void SendUserListUpdate() {
            GetAuthenticatedClients().ForEach(client => {
                client.Send(new ServerPacketClientList() {
                    Users = GetAuthenticatedClients()
                        .FindAll(client2 => client != client2)
                        .ConvertAll(client2 => client2.Mod<AuthModule>().ToClientListUser())
                });
            });
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
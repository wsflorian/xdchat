using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
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

        private X509Certificate TlsCertificate;
        
        public XdDatabase Db => new XdDatabase(new DbContextOptionsBuilder()
            .UseMySql(Config.SqlConnection)
            .Options);

        private XdServer() {
            _moduleHolder = new ModuleHolder<XdServer>(this);
        }

        public override void Start() {
            XdScheduler.CheckIsMainThread();
            
            this._consoleHandler = new ConsoleHandler();
            
            this.Config = ServerConfig.Load();
            if (this.Config == null) {
                ServerConfig.Create();
                XdLogger.Info("config.json was created. Please configure it and restart the server");
                Environment.Exit(1);
                return;
            }

            XdLogger.Info("Checking database...");
            using (XdDatabase db = this.Db) {
                db.Database.EnsureCreated();
            }

            if (this.Config.TlsEnabled) {
                XdLogger.Info("Loading TLS certificate...");

                X509Certificate cert = new X509Certificate(this.Config.TlsCertFile, "", X509KeyStorageFlags.Exportable);
                TlsCertificate = new X509Certificate2(cert.Export(X509ContentType.Pkcs12));
            }
            
            this._moduleHolder.RegisterModule<CommandModule>();

            if (_serverSocket != null)
                throw new InvalidOperationException("Server is already running");

            this._serverSocket = new TcpListener(IPAddress.Parse(this.Config.ServerHost), this.Config.ServerPort);

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
                    
                    Stream stream = this.Config.TlsEnabled ? InitSsl(tcpClient) : tcpClient.GetStream();

                    if (stream == null) { // Failed to initialise stream
                        Helper.DisposeAndNull(tcpClient);
                        continue;
                    }

                    XdScheduler.QueueSyncTask(() => {
                        XdClientConnection client = new XdClientConnection();
                        client.Initialize(tcpClient, stream);
                        this.Clients.Add(client);
                    });
                }
            } catch (SocketException) {
                XdLogger.Info("Server stopped");
            }
        }

        private Stream InitSsl(TcpClient tcpClient) {
            SslStream sslStream = null;
            try {
                sslStream = new SslStream(tcpClient.GetStream(), false);
                sslStream.AuthenticateAsServer(TlsCertificate, false, true);

                return sslStream;
            } catch (Exception ex) {
                sslStream?.Close();
                XdLogger.Error($"Failed to initialize SSL connection. Connection will be closed. {ex}");
                return null;
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
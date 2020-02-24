using Microsoft.Win32;
using System.Linq;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using XdChatShared;
using XdChatShared.Packets;
using XdChatShared.Scheduler;

namespace xdchat_client {
    class XdServerConnection : XdConnection {
        private const string RegistryPath = @"HKEY_CURRENT_USER\Software\XdClient";
        private const string RegistryUuidValueName = "uuid";
        private const string RegistryNicknameValueName = "nickname";

        private string ServerAddress { get; }
        private int Port { get; }

        private string Uuid { get; set; }
        private string Nickname { get; set; }

        //private string mainThread;

        public XdServerConnection(string server, int port) {
            this.ServerAddress = server;
            this.Port = port;
        }
        //427
        public void Connect(){
            try {
                //Registry.SetValue(RegistryPath, RegistryNicknameKey, "test");
                Nickname = (string)Registry.GetValue(RegistryPath, RegistryNicknameValueName, null);
                
#if DEBUG
                Uuid = Guid.NewGuid().ToString();
#else
                Uuid = (string)Registry.GetValue(RegistryPath, RegistryUuidValueName, null);
#endif

                if(Nickname == null){
                    GetNickname();
                }else {
                    char selection = ' ';
                    while(selection != 'y' && selection != 'n'){
                        //Console.Clear();
                        Console.WriteLine($"Nickname found: {Nickname}");
                        Console.WriteLine("Do you want to use this one again? (y/n)");
                        selection = Char.ToLower(Console.ReadKey().KeyChar);
                    }
                    Console.WriteLine(selection);
                    if(selection == 'n'){
                        GetNickname();
                    }

                    //Console.Clear();
                }

                if(Uuid == null){
                    Uuid = Guid.NewGuid().ToString();
                    Registry.SetValue(RegistryPath, RegistryUuidValueName, Uuid);
                }

                base.Initialize(new TcpClient(ServerAddress, Port));
                XdScheduler.Instance.RunSync(() => {
                    base.Send(new ClientPacketAuth() {
                        Nickname = Nickname, Uuid = Uuid
                    });
                });

                while (base.Connected) {
                    Console.Write("Message: ");
                    string message = Console.ReadLine();
                    
                    XdScheduler.Instance.RunSync(() => SendMessage(message));
                }
            } catch (SocketException e) {
                Console.WriteLine($"Connect error: {e}");
            } catch (IOException e) {
                Console.WriteLine($"Connection-Error: {e}");
            } 
        }

        public void GetNickname(){
            Nickname = null;
            while(string.IsNullOrEmpty(Nickname)){
                //Console.Clear();
                Console.Write("Enter nickname: ");
                Nickname = Console.ReadLine();
                Nickname = Nickname.Trim();
                Nickname = new string(Nickname.Where(char.IsLetterOrDigit).ToArray());
            }

            if(Nickname.Length > Constants.MaxNickLength){
                Nickname = Nickname.Substring(0, 20);
            }

            Registry.SetValue(RegistryPath, RegistryNicknameValueName, Nickname);
        }

        public void SendMessage(string message){
            if (message.Length > 0) {
                base.Send(new ClientPacketChatMessage() {
                    Text = message
                });
            }
        }

        protected override void OnPacketReceived(Packet packet) {
            Console.WriteLine($"Data recieved... " + Packet.ToJson(packet));
            //throw new NotImplementedException();
        }

        protected override void OnDisconnect(Exception ex) {
            Console.WriteLine($"Disconnected: {ex}");
        }
    }
}
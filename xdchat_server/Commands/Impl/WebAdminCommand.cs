using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using xdchat_server.ClientCon;
using xdchat_server.Db;
using xdchat_server.Server;

namespace xdchat_server.Commands.Impl {
    public class WebAdminCommand : Command {
        public WebAdminCommand() : base("webadmin", "server.webadmin", 
            "Retrieve a token for the web administration panel", "wa") { }
        protected override void OnCommand(ICommandSender sender, List<string> args) {
            if (sender is ConsoleCommandSender) {
                sender.SendMessage("Only users can create web admin tokens");
                return;
            }

            XdClientConnection client = (XdClientConnection) sender;
            using (XdDatabase db = XdServer.Instance.Db) {
                DbUser dbUser = client.Auth.GetDbUser(db);
                DbWebToken token = DbWebToken.Create(db, new DbWebToken {
                    Token = GenerateToken(),
                    ExpiryTimeStamp = DateTime.Now.AddHours(1),
                    UserId = dbUser.Id
                });

                client.SendMessage($"Use the following token to login: {token.Token}", token.Token);
            }
        }

        private static string GenerateToken() {
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider()) {
                byte[] data = new byte[24];
                crypto.GetBytes(data);
                return BitConverter.ToString(data).Replace("-", "");
            }
        }
    }
}
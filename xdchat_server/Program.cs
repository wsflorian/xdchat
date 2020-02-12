using System;
using System.Text;

namespace xdchat_server {
    static class Program {
        static void Main(string[] args) {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;

            XdServer.Instance.Start(); // This is blocking
        }
    }
}

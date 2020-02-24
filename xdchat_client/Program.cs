using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleGui;
using XdChatShared;
using XdChatShared.ConsoleMouseListener;
using XdChatShared.Scheduler;

namespace xdchat_client {
    class Program {
        static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;
            Console.CursorVisible = false;
            // XdServerConnection client = new XdServerConnection("192.168.28.203", 10001);

            if (Environment.GetEnvironmentVariable("DBG_RUN_CLIENT") == "1") {
                XdServerConnection client = new XdServerConnection(ConsoleExtend.ReadLinePrefill("Enter server address: ", "localhost"), 10000);
            
                XdScheduler.Instance.RunAsync("Connection-Thread", () => {
                    client.Connect();
                });
                
                return;
            }

            // git test
            var frame = new Box()
            {
                Size = new ElemSize(50,90)
            };
            var ChatMessageBox = new Box()
            {
                Size = new ElemSize(45,75),
                Id = "ChtBox"
            };
            var ChatUsersBox = new Box()
            {
                Size = new ElemSize(45, 15),
                Position = new ElemPos(0,75),
                Id = "UsrBox"
            };
            var MessageBox = new Box()
            {
                Size = new ElemSize(5, 90),
                Position = new ElemPos(45, 0),
                Id = "MsgBox"
            };
            var TextInput = new TextInput()
            {
                Position = new ElemPos(2,2),
                Size = new ElemSize(1, 20),
                Prompt = "Msg: ",
                Value = "A"
            };

            frame.AddChild(ChatMessageBox);
            frame.AddChild(ChatUsersBox);
            frame.AddChild(MessageBox);
            MessageBox.AddChild(TextInput);
            
            frame.SetupAsFrame();
            frame.Render();
            
            // MouseListener.RestartListenThread();
            // MouseListener.RegisteredElements.Add(ChatMessageBox);
            // MouseListener.RegisteredElements.Add(ChatUsersBox);
            // MouseListener.RegisteredElements.Add(MessageBox);
            
            Console.ReadKey();
        }
    }
}

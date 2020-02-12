﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleGui;
using XdChatShared;
using XdChatShared.ConsoleMouseListener;

namespace xdchat_client {
    class Program {
        static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;
            Console.CursorVisible = false;
            MouseListener.RestartListenThread();
            // XdServerConnection client = new XdServerConnection("192.168.28.203", 10001);
            // XdServerConnection client = new XdServerConnection(ConsoleExtend.ReadLinePrefill("Enter server address: ", "localhost"), 10000);
            // client.Connect();
            // git test
            var frame = new Box()
            {
                Size = new ElemSize(50,90)
            };
            var ChatMessageBox = new Box()
            {
                Size = new ElemSize(43,75),
                Id = "ChtBox"
            };
            var ChatUsersBox = new Box()
            {
                Size = new ElemSize(43, 15),
                Position = new ElemPos(0,75),
                Id = "UsrBox"
            };
            var MessageBox = new Box()
            {
                Size = new ElemSize(7, 90),
                Position = new ElemPos(43, 0),
                Id = "MsgBox"
            };
            
            MouseListener.RegisteredElements.Add(ChatMessageBox);
            MouseListener.RegisteredElements.Add(ChatUsersBox);
            MouseListener.RegisteredElements.Add(MessageBox);

            frame.AddChild(ChatMessageBox);
            frame.AddChild(ChatUsersBox);
            frame.AddChild(MessageBox);
            
            frame.SetupAsFrame();
            frame.Render();
            Console.ReadKey();
        }
    }
}

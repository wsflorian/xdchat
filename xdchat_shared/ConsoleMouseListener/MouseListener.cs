using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

namespace XdChatShared.ConsoleMouseListener
{
    public class MouseListener
    {
        private static bool ConsoleLocked { get; set; } = false;
        private static Thread ListenThread { get; set; }
        private static bool Running { get; set; }
        
        private static NativeMethods.ConsoleHandle _handle;
        private static int _mode;
        
        public static void RestartListenThread()
        {
            Running = false;
            if (ListenThread != null)
            {
                ListenThread.Abort();
            }
            ListenThread = new Thread(RunThread);
            
            _handle = NativeMethods.GetStdHandle(NativeMethods.STD_INPUT_HANDLE);
            _mode = 0;
            
            if (!(NativeMethods.GetConsoleMode(_handle, ref _mode))) { throw new Win32Exception(); }

            _mode |= NativeMethods.ENABLE_MOUSE_INPUT;
            _mode &= ~NativeMethods.ENABLE_QUICK_EDIT_MODE;
            _mode |= NativeMethods.ENABLE_EXTENDED_FLAGS;

            if (!(NativeMethods.SetConsoleMode(_handle, _mode))) { throw new Win32Exception(); }
            
            ListenThread.Start();
        }

        private static void RunThread()
        {
            Running = true;
            var record = new NativeMethods.INPUT_RECORD();
            uint recordLen = 0;
            while (Running)
            {
                if (!(NativeMethods.ReadConsoleInput(_handle, ref record, 1, ref recordLen)))
                {
                    throw new Win32Exception();
                }
                
                switch (record.EventType)
                {
                    case NativeMethods.MOUSE_EVENT:
                        if (!ConsoleLocked)
                        {
                            Console.SetCursorPosition(0,0);
                            Console.Write($"X:{record.MouseEvent.dwMousePosition.X/2} Y:{record.MouseEvent.dwMousePosition.Y}          ");
                        }
                        // do something at mouse event
                        break;

                    case NativeMethods.KEY_EVENT: 
                        // do something at key event
                        break;
                }
            }
        }

        public static void LockConsole(bool locked)
        {
            ConsoleLocked = locked;
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using ConsoleGui;


namespace XdChatShared.ConsoleMouseListener
{
    public class MouseListener
    {
        public static List<Element> RegisteredElements { get; private set; } = new List<Element>();
        private static List<Element> PrevHovered { get; set; }
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
            
            RegisteredElements = new List<Element>();
            
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
            PrevHovered = new List<Element>();
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
                        int x = record.MouseEvent.dwMousePosition.X / 2;
                        int y = record.MouseEvent.dwMousePosition.Y;

                        var hoveredElems = RegisteredElements.Where(el => el.IsPointInside(2*x, y));
                        
                        ExecuteEvents(hoveredElems, x, y);

                        PrevHovered = hoveredElems.ToList();
                        // do something at mouse event
                        break;

                    case NativeMethods.KEY_EVENT: 
                        // do something at key event
                        break;
                }
            }
        }

        public static async void ExecuteEvents(IEnumerable<Element> hoveredElems, int x, int y)
        {
            foreach (var elem in hoveredElems.Where(el => !PrevHovered.Contains(el)))
            {
                elem.OnHover(x,y);
            }

            foreach (var elem in PrevHovered.Where(el => !hoveredElems.Contains(el)))
            {
                elem.Render();
            }
        }

        public static void LockConsole(bool locked)
        {
            ConsoleLocked = locked;
        }
    }
}
using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace XdChatShared.ConsoleMouseListener
{
    public class NativeMethods
    {

        public const int STD_INPUT_HANDLE = -10;

        public const int ENABLE_MOUSE_INPUT = 0x0010;
        public const int ENABLE_QUICK_EDIT_MODE = 0x0040;
        public const int ENABLE_EXTENDED_FLAGS = 0x0080;

        public const int KEY_EVENT = 1;
        public const int MOUSE_EVENT = 2;


        [DebuggerDisplay("EventType: {EventType}")]
        [StructLayout(LayoutKind.Explicit)]
        public struct INPUT_RECORD
        {
            [FieldOffset(0)] public short EventType;
            [FieldOffset(4)] public KEY_EVENT_RECORD KeyEvent;
            [FieldOffset(4)] public MOUSE_EVENT_RECORD MouseEvent;
        }

        [DebuggerDisplay("{dwMousePosition.X}, {dwMousePosition.Y}")]
        public struct MOUSE_EVENT_RECORD
        {
            public COORD dwMousePosition;
            public int dwButtonState;
            public int dwControlKeyState;
            public int dwEventFlags;
        }

        [DebuggerDisplay("{X}, {Y}")]
        public struct COORD
        {
            public ushort X;
            public ushort Y;
        }

        [DebuggerDisplay("KeyCode: {wVirtualKeyCode}")]
        [StructLayout(LayoutKind.Explicit)]
        public struct KEY_EVENT_RECORD
        {
            [FieldOffset(0)] [MarshalAsAttribute(UnmanagedType.Bool)]
            public bool bKeyDown;

            [FieldOffset(4)] public ushort wRepeatCount;
            [FieldOffset(6)] public ushort wVirtualKeyCode;
            [FieldOffset(8)] public ushort wVirtualScanCode;
            [FieldOffset(10)] public char UnicodeChar;
            [FieldOffset(10)] public byte AsciiChar;
            [FieldOffset(12)] public int dwControlKeyState;
        };


        public class ConsoleHandle : SafeHandleMinusOneIsInvalid
        {
            public ConsoleHandle() : base(false)
            {
            }

            protected override bool ReleaseHandle()
            {
                return true; //releasing console handle is not our business
            }
        }


        [DllImportAttribute("kernel32.dll", SetLastError = true)]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        public static extern Boolean GetConsoleMode(ConsoleHandle hConsoleHandle, ref Int32 lpMode);

        [DllImportAttribute("kernel32.dll", SetLastError = true)]
        public static extern ConsoleHandle GetStdHandle(Int32 nStdHandle);

        [DllImportAttribute("kernel32.dll", SetLastError = true)]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        public static extern Boolean ReadConsoleInput(ConsoleHandle hConsoleInput, ref INPUT_RECORD lpBuffer,
            UInt32 nLength, ref UInt32 lpNumberOfEventsRead);

        [DllImportAttribute("kernel32.dll", SetLastError = true)]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        public static extern Boolean SetConsoleMode(ConsoleHandle hConsoleHandle, Int32 dwMode);

    }
}
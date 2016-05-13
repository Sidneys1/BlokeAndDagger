using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local

namespace BlokeAndDagger.Helpers {

    public static class ExtendedConsole {
        #region Fields

        private static readonly SafeFileHandle ConsoleInputHandle;

        private static readonly SafeFileHandle ConsoleOutputHandle;

        private static readonly IntPtr WindowHandle;

        #endregion Fields

        #region Constructors
        
        static ExtendedConsole() {
            WindowHandle = GetConsoleWindow();

            if (WindowHandle == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            ConsoleOutputHandle = CreateFile("CONOUT$", 0x80000000 | 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
            if (ConsoleOutputHandle.IsInvalid)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            ConsoleInputHandle = CreateFile("CONIN$", 0x80000000 | 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
            if (ConsoleInputHandle.IsInvalid)
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        #endregion Constructors

        #region Methods

        public static void ClearConsoleArea(short x, short y, short w, short h) {
            var r = Enumerable.Repeat(new CharInfo {
                Char = new CharUnion { UnicodeChar = ' ' },
                Attributes = (short)((short)Console.ForegroundColor | (short)((short)Console.BackgroundColor << 8))
            }, w * h).ToArray();
            var rect = new SmallRect {
                Top = y,
                Left = x,
                Bottom = (short)(y + (h - 1)),
                Right = (short)(x + (w - 1))
            };
            var size = new Coord(w, h);
            var pos = Coord.Zero;
            if (!UpdateRegion(r, pos, size, ref rect))
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        public static void ClearConsoleLine(short line) {
            var r = Enumerable.Repeat(new CharInfo {
                Char = new CharUnion { UnicodeChar = ' ' },
                Attributes = (short)((short)Console.ForegroundColor | (short)((short)Console.BackgroundColor << 8))
            }, Console.BufferWidth).ToArray();
            var rect = new SmallRect { Top = line, Bottom = line, Left = 0, Right = (short)(r.Length - 1) };
            var size = new Coord((short)r.Length, 1);
            var pos = Coord.Zero;
            if (!UpdateRegion(r, pos, size, ref rect))
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        public static void ClearCurrentConsoleLine() =>
            ClearConsoleLine((short)Console.CursorTop);

        public static void EnableMouseInput() {
            uint mode;
            if (!GetConsoleMode(ConsoleInputHandle, out mode))
                throw new Win32Exception(Marshal.GetLastWin32Error());
            mode |= (uint)ConsoleModes.ENABLE_MOUSE_INPUT;
            mode &= ~(uint)ConsoleModes.ENABLE_QUICK_EDIT_MODE;
            mode |= (uint)ConsoleModes.ENABLE_EXTENDED_FLAGS;

            if (!SetConsoleMode(ConsoleInputHandle, mode))
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        public static void FixConsoleSize() =>
            SetWindowLong(WindowHandle, -16, GetWindowLong(WindowHandle, -16) ^ 0x00050000);

        public static void GetConsoleInput(ref InputRecord[] buf) {
            uint r;
            if (!ReadConsoleInput(ConsoleInputHandle, buf, buf.Length, out r))
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        public static bool UpdateRegion(CharInfo[] buffer, Coord point, Coord size, ref SmallRect smallRect) =>
            WriteConsoleOutput(ConsoleOutputHandle, buffer, size, point, ref smallRect);

        private static readonly Stack<ConsoleColor> ForegroundStack = new Stack<ConsoleColor>();
        private static readonly Stack<ConsoleColor> BackgroundStack = new Stack<ConsoleColor>();
        public static void PushConsoleColors() {
            ForegroundStack.Push(Console.ForegroundColor);
            BackgroundStack.Push(Console.BackgroundColor);
        }
        public static void PopConsoleColors() {
            if (ForegroundStack.Count == 0) return;
            Console.ForegroundColor = ForegroundStack.Pop();
            Console.BackgroundColor = BackgroundStack.Pop();
        }
        public static void PeekConsoleColors() {
            if (ForegroundStack.Count == 0) return;
            Console.ForegroundColor = ForegroundStack.Peek();
            Console.BackgroundColor = BackgroundStack.Peek();
        }

        public static bool GetConsoleInfo(ref ConsoleScreenBufferInfoEx info) =>
            GetConsoleScreenBufferInfoEx(ConsoleOutputHandle, ref info);

        public static bool SetConsoleInfo(ref ConsoleScreenBufferInfoEx info) =>
            SetConsoleScreenBufferInfoEx(ConsoleOutputHandle, ref info);

        #region Externs

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern SafeFileHandle CreateFile(
            string fileName,
            [MarshalAs(UnmanagedType.U4)] uint fileAccess,
            [MarshalAs(UnmanagedType.U4)] uint fileShare,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] int flags,
            IntPtr template);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetConsoleMode(SafeFileHandle hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("kernel32.dll", EntryPoint = "ReadConsoleInputW", CharSet = CharSet.Unicode)]
        private static extern bool ReadConsoleInput(
            SafeFileHandle hConsoleInput,
            [Out] InputRecord[] lpBuffer,
            int nLength,
            out uint lpNumberOfEventsRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleMode(SafeFileHandle hConsoleHandle, uint dwMode);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteConsoleOutput(
            SafeFileHandle hConsoleOutput,
            CharInfo[] lpBuffer,
            Coord dwBufferSize,
            Coord dwBufferCoord,
            ref SmallRect lpWriteRegion);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleScreenBufferInfoEx(
            SafeFileHandle ConsoleOutput,
            ref ConsoleScreenBufferInfoEx ConsoleScreenBufferInfoEx
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetConsoleScreenBufferInfoEx(
            SafeFileHandle hConsoleOutput,
            ref ConsoleScreenBufferInfoEx ConsoleScreenBufferInfo
        );
        
        #endregion

        #endregion Methods

        #region Enums

        [Flags]
        private enum ConsoleModes : uint {
            ENABLE_PROCESSED_INPUT = 0x0001,
            ENABLE_LINE_INPUT = 0x0002,
            ENABLE_ECHO_INPUT = 0x0004,

            ENABLE_WINDOW_INPUT = 0x0008,
            ENABLE_MOUSE_INPUT = 0x0010,
            ENABLE_INSERT_MODE = 0x0020,
            ENABLE_QUICK_EDIT_MODE = 0x0040,
            ENABLE_EXTENDED_FLAGS = 0x0080,
            ENABLE_AUTO_POSITION = 0x0100,
            ENABLE_PROCESSED_OUTPUT = 0x0001,
            ENABLE_WRAP_AT_EOL_OUTPUT = 0x0002,
            all = ENABLE_PROCESSED_INPUT | ENABLE_LINE_INPUT | ENABLE_ECHO_INPUT | ENABLE_WINDOW_INPUT | ENABLE_MOUSE_INPUT | ENABLE_INSERT_MODE | ENABLE_QUICK_EDIT_MODE | ENABLE_EXTENDED_FLAGS | ENABLE_AUTO_POSITION
        }

        #endregion Enums

        #region Structs

        [StructLayout(LayoutKind.Explicit)]
        public struct CharInfo
        {

            [FieldOffset(0)] public CharUnion Char;

            [FieldOffset(2)] public short Attributes;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct CharUnion
        {

            [FieldOffset(0)] public char UnicodeChar;

            [FieldOffset(0)] public byte AsciiChar;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Coord
        {
            public short X;
            public short Y;

            public Coord(short x, short y)
            {
                X = x;
                Y = y;
            }

            public static Coord Zero = new Coord(0, 0);
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct FocusEventRecord
        {
            public uint bSetFocus;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct InputRecord
        {

            [FieldOffset(0)] public ushort EventType;

            [FieldOffset(4)] public KeyEventRecord KeyEvent;

            [FieldOffset(4)] public MouseEventRecord MouseEvent;

            [FieldOffset(4)] public WindowBufferSizeRecord WindowBufferSizeEvent;

            [FieldOffset(4)] public MenuEventRecord MenuEvent;

            [FieldOffset(4)] public FocusEventRecord FocusEvent;
        };

        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
        public struct KeyEventRecord
        {

            [FieldOffset(0), MarshalAs(UnmanagedType.Bool)] public bool bKeyDown;

            [FieldOffset(4), MarshalAs(UnmanagedType.U2)] public ushort wRepeatCount;

            [FieldOffset(6), MarshalAs(UnmanagedType.U2)] public ushort wVirtualKeyCode;

            [FieldOffset(8), MarshalAs(UnmanagedType.U2)] public ushort wVirtualScanCode;

            [FieldOffset(10)] public char UnicodeChar;

            [FieldOffset(12), MarshalAs(UnmanagedType.U4)] public uint dwControlKeyState;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MenuEventRecord
        {
            public uint dwCommandId;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct MouseEventRecord
        {

            [FieldOffset(0)] public Coord dwMousePosition;

            [FieldOffset(4)] public uint dwButtonState;

            [FieldOffset(8)] public uint dwControlKeyState;

            [FieldOffset(12)] public uint dwEventFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SmallRect
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }

        public struct WindowBufferSizeRecord
        {
            public Coord dwSize;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct ColorRef {
            public ColorRef(byte r, byte g, byte b) {
                Value = 0;
                R = r;
                G = g;
                B = b;
            }

            public ColorRef(uint value) {
                R = 0;
                G = 0;
                B = 0;
                Value = value & 0x00FFFFFF;
            }

            [FieldOffset(0)]
            public byte R;
            [FieldOffset(1)]
            public byte G;
            [FieldOffset(2)]
            public byte B;

            [FieldOffset(0)]
            public uint Value;

            public void Set(byte r, byte g, byte b)
            {
                R = r;
                G = g;
                B = b;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ConsoleScreenBufferInfoEx {
            public uint cbSize;
            public Coord dwSize;
            public Coord dwCursorPosition;
            public short wAttributes;
            public SmallRect srWindow;
            public Coord dwMaximumWindowSize;
            public ushort wPopupAttributes;
            public bool bFullscreenSupported;

            internal ColorRef black;
            internal ColorRef darkBlue; 
            internal ColorRef darkGreen;
            internal ColorRef darkCyan;
            internal ColorRef darkRed;
            internal ColorRef darkMagenta;
            internal ColorRef darkYellow;
            internal ColorRef gray;
            internal ColorRef darkGray;
            internal ColorRef blue;
            internal ColorRef green;
            internal ColorRef cyan;
            internal ColorRef red;
            internal ColorRef magenta;
            internal ColorRef yellow;
            internal ColorRef white;

            public short Width
            {
                get { return dwSize.X; }
                set { dwSize.X = value; }
            }
            public short Height
            {
                get { return dwSize.Y; }
                set { dwSize.Y = value; }
            }
            
            public void SetColor(ConsoleColor color, byte r, byte g, byte b)
            {
                switch (color)
                {
                    case ConsoleColor.Black:
                        black.Set(r, g, b);
                        break;
                    case ConsoleColor.DarkBlue:
                        darkBlue.Set(r,g,b);
                        break;
                    case ConsoleColor.DarkGreen:
                        darkGreen.Set(r, g, b);
                        break;
                    case ConsoleColor.DarkCyan:
                        darkCyan.Set(r, g, b);
                        break;
                    case ConsoleColor.DarkRed:
                        darkRed.Set(r, g, b);
                        break;
                    case ConsoleColor.DarkMagenta:
                        darkMagenta.Set(r, g, b);
                        break;
                    case ConsoleColor.DarkYellow:
                        darkYellow.Set(r, g, b);
                        break;
                    case ConsoleColor.Gray:
                        gray.Set(r, g, b);
                        break;
                    case ConsoleColor.DarkGray:
                        darkGray.Set(r, g, b);
                        break;
                    case ConsoleColor.Blue:
                        blue.Set(r, g, b);
                        break;
                    case ConsoleColor.Green:
                        green.Set(r, g, b);
                        break;
                    case ConsoleColor.Cyan:
                        cyan.Set(r, g, b);
                        break;
                    case ConsoleColor.Red:
                        red.Set(r, g, b);
                        break;
                    case ConsoleColor.Magenta:
                        magenta.Set(r, g, b);
                        break;
                    case ConsoleColor.Yellow:
                        yellow.Set(r, g, b);
                        break;
                    case ConsoleColor.White:
                        white.Set(r, g, b);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(color), color, "Not a valid ConsoleColor");
                }
            }

            public bool Apply() => SetConsoleInfo(ref this);

            public bool Update() => GetConsoleInfo(ref this);

            public static ConsoleScreenBufferInfoEx GetNew()=>new ConsoleScreenBufferInfoEx {cbSize = 96};

            public static ConsoleScreenBufferInfoEx GetCurrent()
            {
                var ret = GetNew();
                GetConsoleInfo(ref ret);
                return ret;
            }

            public ColorRef[] GetColors() =>
                    new[]
                    {
                        black,      darkBlue,       darkGreen,  darkCyan,
                        darkRed,    darkMagenta,    darkYellow, gray,
                        darkGray,   blue,           green,      cyan,
                        red,        magenta,        yellow,     white
                    };

            public void SetColors(ColorRef[] colors)
            {
                if (colors.Length != 16)
                    throw new ArgumentException("Array must have 16 entries", nameof(colors));

                black = colors[0];
                darkBlue = colors[1];
                darkGreen = colors[2];
                darkCyan = colors[3];
                darkRed = colors[4];
                darkMagenta = colors[5];
                darkYellow = colors[6];
                gray = colors[7];
                darkGray = colors[8];
                blue = colors[9];
                green = colors[10];
                cyan = colors[11];
                red = colors[12];
                magenta = colors[13];
                yellow = colors[14];
                white = colors[15];
            }
                    
        }

        #endregion
    }
}
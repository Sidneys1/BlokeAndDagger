using System;
using System.Threading;
using BlokeAndDagger.Base;
using BlokeAndDagger.Helpers;
using Rant.Vocabulary;
using Rant;
using SimpleArgv;

namespace BlokeAndDagger {
    internal class Program {
        private const string StartMsg = "[Click to Start]";
        public static Random Rand = new Random();

        public static CommandLine CommandLine = new CommandLine(new[] {"--", "-"});

        private static readonly string[] TitleArt = {
            @" ▄▀▀█▄▄   ▄▀▀▀▀▄    ▄▀▀▀▀▄   ▄▀▀▄ █  ▄▀▀█▄▄▄▄      ▄▀▀█▄   ▄▀▀▄ ▀▄  ▄▀▀█▄▄ ",
            @"▐ ▄▀   █ █    █    █      █ █  █ ▄▀ ▐  ▄▀   ▐     ▐ ▄▀ ▀▄ █  █ █ █ █ ▄▀   █",
            @"  █▄▄▄▀  ▐    █    █      █ ▐  █▀▄    █▄▄▄▄▄        █▄▄▄█ ▐  █  ▀█ ▐ █    █",
            @"  █   █      █     ▀▄    ▄▀   █   █   █    ▌       ▄▀   █   █   █    █    █",
            @" ▄▀▄▄▄▀    ▄▀▄▄▄▄▄▄▀ ▀▀▀▀   ▄▀   █   ▄▀▄▄▄▄       █   ▄▀  ▄▀   █    ▄▀▄▄▄▄▀",
            @"█    ▐     █                █    ▐   █    ▐       ▐   ▐   █    ▐   █     ▐ ",
            @"▐          ▐                ▐        ▐                    ▐        ▐       ",
            @"         ▄▀▀█▄▄   ▄▀▀█▄   ▄▀▀▀▀▄    ▄▀▀▀▀▄   ▄▀▀█▄▄▄▄  ▄▀▀▄▀▀▀▄            ",
            @"        █ ▄▀   █ ▐ ▄▀ ▀▄ █         █        ▐  ▄▀   ▐ █   █   █            ",
            @"        ▐ █    █   █▄▄▄█ █    ▀▄▄  █    ▀▄▄   █▄▄▄▄▄  ▐  █▀▀█▀             ",
            @"          █    █  ▄▀   █ █     █ █ █     █ █  █    ▌   ▄▀    █             ",
            @"         ▄▀▄▄▄▄▀ █   ▄▀  ▐▀▄▄▄▄▀ ▐ ▐▀▄▄▄▄▀ ▐ ▄▀▄▄▄▄   █     █              ",
            @"        █     ▐  ▐   ▐   ▐         ▐         █    ▐   ▐     ▐              ",
            @"        ▐                                    ▐                             ",
        };

        private static void Main(string[] argv) {
            AddArgumentParsers();

            CommandLine.Parse(argv);
            
            Console.Title = "Bloke and Dagger";
            Console.BufferWidth = Console.WindowWidth = 201;
            Console.BufferHeight = Console.WindowHeight = 51;
            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            var info = ExtendedConsole.ConsoleScreenBufferInfoEx.GetCurrent();
            info.Width--;
            info.Height--;
            info.Apply();
            ExtendedConsole.FixConsoleSize();
            ExtendedConsole.EnableMouseInput();

            Console.Clear();

            if (!CommandLine.RawArguments.ContainsKey("--SkipIntro"))
                Intro(info);

            SetupMonokai(info);
            
            Console.ForegroundColor = ConsoleColor.White;

            //Game.Play();

            Console.WriteLine("Press ENTER to exit...");
            Console.ReadLine();
        }

        private static void AddArgumentParsers()
        {
            CommandLine.AddArgument(s=>null, string.Empty);
            CommandLine.AddArgument(s=>null, "--SkipIntro");

            CommandLine.AddArgument(s => Enum.Parse(typeof(Renown), s[0]), "--InitialRenown");
        }

        private static void Intro(ExtendedConsole.ConsoleScreenBufferInfoEx info) {
            var cache = info.GetColors();

            info.SetColor(ConsoleColor.DarkBlue, 0, 0, 0);
            info.SetColor(ConsoleColor.DarkGreen, 0, 0, 0);
            info.Apply();

            Console.Clear();
            var titleWidth = info.Width / 2 - TitleArt[0].Length / 2;
            Console.SetCursorPosition(titleWidth, info.Height / 2 - TitleArt.Length / 2 - 1);
            foreach (var t in TitleArt) {
                Console.WriteLine(t);
                Console.CursorLeft = titleWidth;
            }

            Console.SetCursorPosition(info.Width / 2 - StartMsg.Length / 2, Console.CursorTop + 1);
            
            var clickLine = Console.CursorTop;
            var clickLeft = Console.CursorLeft;
            foreach (var c in StartMsg) {
                if (c != ' ')
                    Console.ForegroundColor++;
                Console.Write(c);
                info.SetColor(Console.ForegroundColor, 0, 0, 0);
            }
            
            var clickRight = Console.CursorLeft;
            Console.ForegroundColor = ConsoleColor.DarkBlue;

            const int sleep = 2000 / 255;
            for (byte i = 0; i < 255; i++) {
                info.SetColor(ConsoleColor.Black, i, i, i);
                var c = ConsoleColor.DarkGreen;
                for (var j = 0; j < 14; j++)
                    info.SetColor(c++, i, i, i);
                info.Apply();
                Thread.Sleep(sleep);
            }

            Thread.Sleep(1000);

            for (var i = 1; i < 255 * 3; i += 8) {
                var v = (int)(255 - i);
                var c = ConsoleColor.DarkGreen;
                for (var j = 14; j > 0; j--) {
                    var y = Math.Min(255, v + (j * 32));
                    var x = (byte)Math.Max(0, y);
                    info.SetColor(c++, x, x, x);
                }
                info.Apply();
                Thread.Sleep(sleep);
            }

            var buf = new ExtendedConsole.InputRecord[1];
            var ms = false;
            var lx = 0;
            var ly = 0;
            var over = false;
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            do {
                ExtendedConsole.GetConsoleInput(ref buf);
                if ((buf[0].EventType & 0x0002) != 0x0002) continue;
                if ((buf[0].MouseEvent.dwEventFlags & 0x0001) == 0x0001) {
                    lx = buf[0].MouseEvent.dwMousePosition.X;
                    ly = buf[0].MouseEvent.dwMousePosition.Y;
                }
                if (!over && ly == clickLine && lx >= clickLeft && lx < clickRight) {
                    over = true;
                    var c = ConsoleColor.DarkGreen;
                    for (var j = 0; j < 14; j++)
                        info.SetColor(c++, 128, 128, 128);
                    info.Apply();
                } else if (over && !(ly == clickLine && lx >= clickLeft && lx < clickRight)) {
                    over = false;
                    var c = ConsoleColor.DarkGreen;
                    for (var j = 0; j < 14; j++)
                        info.SetColor(c++, 0, 0, 0);
                    info.Apply();
                }

                var mDown = (buf[0].MouseEvent.dwButtonState & 0x0001) == 0x0001;
                if (over) {
                    if (!ms && mDown)
                        ms = true;
                    else if (ms && !mDown)
                        break;
                } else if (ms) ms = false;
            } while (true);

            {
                var c = ConsoleColor.DarkGreen;
                for (var j = 0; j < 14; j++)
                    info.SetColor(c++, 0, 0, 0);
            }

            for (byte i = 255; i > 1; i -= 2) {
                info.SetColor(ConsoleColor.Black, i, i, i);
                info.Apply();
                Thread.Sleep(sleep / 2);
            }

            Thread.Sleep(500);
            Console.Clear();
            info.SetColors(cache);
            info.Apply();
        }

        private static void SetupMonokai(ExtendedConsole.ConsoleScreenBufferInfoEx info)
        {
            info.black.Value = 0x0;
            info.darkGray.Value = 0x00414746;

            info.darkBlue.Value = 0x00a64c1d;
            info.blue.Value = 0x00ef9566;

            info.darkGreen.Value = 0x0000995d;
            info.green.Value = 0x002ee2a6;

            info.darkCyan.Value = 0x00746a31;
            info.cyan.Value = 0x00efd966;

            info.darkRed.Value = 0x002900b0;
            info.red.Value = 0x007226f9;

            info.darkMagenta.Value = 0x00b63865;
            info.magenta.Value = 0x00ff81ae;

            info.darkYellow.Value = 0x001f97fd;
            info.yellow.Value = 0x0074dbe6;

            info.gray.Value = 0x008a908f;
            info.white.Value = 0x00f2f8f8;
            info.Apply();
        }
    }
}

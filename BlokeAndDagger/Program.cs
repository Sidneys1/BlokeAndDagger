﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using BlokeAndDagger.Base;
using SimpleArgv;
using ExtendedConsole;
using ExtendedConsole.Enums;
using ExtendedConsole.Structs;
using OoeyGui;
using EConsole = ExtendedConsole.ExtendedConsole;

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

        private const int BD_WIDTH = 201;
        private const int BD_HEIGHT = 51;

        private static void Main(string[] argv) {
            AddArgumentParsers();

            CommandLine.Parse(argv);

            Console.Title = "Bloke and Dagger";
            Console.BufferWidth = Console.WindowWidth = 100;
            Console.BufferHeight = Console.WindowHeight = 25;
            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            var info = ConsoleScreenBufferInfoEx.GetCurrent();
            info.Width--;
            info.Height--;
            info.Apply();
            EConsole.FixConsoleSize();
            EConsole.EnableMouseInput();

            Console.Clear();
            Console.ReadLine();
            if (!CommandLine.RawArguments.ContainsKey("--SkipIntro"))
                Intro(info);

            SetupMonokai(info);

            Console.ForegroundColor = ConsoleColor.White;

            OoeyGui.OoeyGui.Init();

            var text = "";
            var uiElement = new Label(50, 1, 35, 1, 0, "All the text you could ever want!") {BackgroundColor = ConsoleColor.DarkGray};
            var uiElement2 = new Label(3, 0, 35, 1, 1, text);
            OoeyGui.OoeyGui.AddChild(uiElement);
            OoeyGui.OoeyGui.AddChild(uiElement2);
            var w = new Stopwatch();
            var avg = new Stack<long>();
            var col = 1;
            OoeyGui.OoeyGui.Repaint();
            for (var i = 0; i < 100000; i++) {
                w.Restart();
                uiElement2.Text = new FormattedString(text.Select(c => new FormattedText(new string(c, 1), (ConsoleColor) (col++ %15 + 1))).ToArray());
                if(i % 256 == 0)
                    uiElement2.Y=(short)((uiElement2.Y + 1) % 25);
                uiElement.Y = (short) ((uiElement.Y + 1)%25);
                OoeyGui.OoeyGui.Repaint();
                w.Stop();
                if (avg.Count == 100) avg.Pop();
                avg.Push(w.ElapsedTicks);
                var avgd = avg.Average();
                text = $"{(TimeSpan.TicksPerSecond/avgd),6:n0} FPS ({TimeSpan.TicksPerMillisecond/avgd:n0}ms average draw time)";
            }

            //Game.Play();

            //Console.WriteLine("Press ENTER to exit...");
            Console.ReadLine();
        }

        private static void AddArgumentParsers() {
            CommandLine.AddArgument(s => null, string.Empty);
            CommandLine.AddArgument(s => null, "--SkipIntro");

            CommandLine.AddArgument(s => Enum.Parse(typeof(Renown), s[0]), "--InitialRenown");
        }

        private static void Intro(ConsoleScreenBufferInfoEx info) {
            var cache = info.GetColors();

            info.SetColor(ConsoleColor.DarkBlue, 0, 0, 0);
            info.SetColor(ConsoleColor.DarkGreen, 0, 0, 0);
            info.Apply();

            Console.Clear();
            var titleWidth = info.Width/2 - TitleArt[0].Length/2;
            Console.SetCursorPosition(titleWidth, info.Height/2 - TitleArt.Length/2 - 1);
            foreach (var t in TitleArt) {
                Console.WriteLine(t);
                Console.CursorLeft = titleWidth;
            }

            Console.SetCursorPosition(info.Width/2 - StartMsg.Length/2, Console.CursorTop + 1);

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

            const int sleep = 2000/255;
            for (byte i = 0; i < 255; i++) {
                info.SetColor(ConsoleColor.Black, i, i, i);
                var c = ConsoleColor.DarkGreen;
                for (var j = 0; j < 14; j++)
                    info.SetColor(c++, i, i, i);
                info.Apply();
                Thread.Sleep(sleep);
            }

            Thread.Sleep(1000);

            for (var i = 1; i < 255*3; i += 8) {
                var v = (int) (255 - i);
                var c = ConsoleColor.DarkGreen;
                for (var j = 14; j > 0; j--) {
                    var y = Math.Min(255, v + (j*32));
                    var x = (byte) Math.Max(0, y);
                    info.SetColor(c++, x, x, x);
                }
                info.Apply();
                Thread.Sleep(sleep);
            }

            var buf = new InputRecord[1];
            var ms = false;
            var lx = 0;
            var ly = 0;
            var over = false;
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            do {
                EConsole.GetConsoleInput(ref buf);
                if (!buf[0].EventType.HasFlag(EventType.MouseEvent)) continue;
                if (buf[0].MouseEvent.EventFlags.HasFlag(MouseEventFlags.MouseMoved)) {
                    lx = buf[0].MouseEvent.MousePosition.X;
                    ly = buf[0].MouseEvent.MousePosition.Y;
                }
                if (!over && ly == clickLine && lx >= clickLeft && lx < clickRight) {
                    over = true;
                    var c = ConsoleColor.DarkGreen;
                    for (var j = 0; j < 14; j++)
                        info.SetColor(c++, 128, 128, 128);
                    info.Apply();
                }
                else if (over && !(ly == clickLine && lx >= clickLeft && lx < clickRight)) {
                    over = false;
                    var c = ConsoleColor.DarkGreen;
                    for (var j = 0; j < 14; j++)
                        info.SetColor(c++, 0, 0, 0);
                    info.Apply();
                }

                var mDown = buf[0].MouseEvent.ButtonState.HasFlag(MouseButtonState.FirstButton);
                if (over) {
                    if (!ms && mDown)
                        ms = true;
                    else if (ms && !mDown)
                        break;
                }
                else if (ms) ms = false;
            } while (true);

            {
                var c = ConsoleColor.DarkGreen;
                for (var j = 0; j < 14; j++)
                    info.SetColor(c++, 0, 0, 0);
            }

            for (byte i = 255; i > 1; i -= 2) {
                info.SetColor(ConsoleColor.Black, i, i, i);
                info.Apply();
                Thread.Sleep(sleep/2);
            }

            Thread.Sleep(500);
            Console.Clear();
            info.SetColors(cache);
            info.Apply();
        }

        private static void SetupMonokai(ConsoleScreenBufferInfoEx info) {
            info.Black.Value = 0x0;
            info.DarkGray.Value = 0x00414746;

            info.DarkBlue.Value = 0x00a64c1d;
            info.Blue.Value = 0x00ef9566;

            info.DarkGreen.Value = 0x0000995d;
            info.Green.Value = 0x002ee2a6;

            info.DarkCyan.Value = 0x00746a31;
            info.Cyan.Value = 0x00efd966;

            info.DarkRed.Value = 0x002900b0;
            info.Red.Value = 0x007226f9;

            info.DarkMagenta.Value = 0x00b63865;
            info.Magenta.Value = 0x00ff81ae;

            info.DarkYellow.Value = 0x001f97fd;
            info.Yellow.Value = 0x0074dbe6;

            info.Gray.Value = 0x008a908f;
            info.White.Value = 0x00f2f8f8;
            info.Apply();
        }
    }
}
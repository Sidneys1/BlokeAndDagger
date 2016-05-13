using BlokeAndDagger.Races;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using BlokeAndDagger.Base;
using BlokeAndDagger.Helpers;
using Rant.Vocabulary;
using Rant;

namespace BlokeAndDagger {
    internal class Program {
        private const string StartMsg = "[Click to Start]";
        public static Random Rand = new Random();
        public static RantEngine Rant = new RantEngine(RantDictionary.FromDirectory(@"C:\Git\Rantionary"));

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
            @"",
            @"                               By  Sidneys1"
        };

        private static void Main() {
            Console.Title = "Bloke and Dagger";
            Console.BufferWidth = Console.WindowWidth = 140;
            Console.BufferHeight = Console.WindowHeight = 35;
            ExtendedConsole.FixConsoleSize();
            ExtendedConsole.EnableMouseInput();
            Console.CursorVisible = false;
            Intro();

            var player = GetRace();
            var playerController = new PlayerController(player);

            Console.Title = $"Bloke and Dagger - {player.RaceName} '{player.Name}'";
            Console.ForegroundColor = ConsoleColor.White;

            Console.Write("You are ");
            PlayerSummary(player);
            Console.WriteLine();
            Console.WriteLine();


            while (player.Health > 0) {
                Player opponent = new Robot();//Player.GetRandomRace();
                var ai = new AIController(opponent);

                Console.Write("Your opponent is ");
                PlayerSummary(opponent);
                Console.WriteLine();
                Console.WriteLine();

                var first = Rand.Next(2) == 1;
                Console.WriteLine("Rolling dice...");
                Console.WriteLine(first ? "You will make the first move!" : "Your opponent will make the first move!");
                Console.WriteLine();

                while (player.Health > 0 && opponent.Health > 0) {
                    if (first) {
                        Console.Write("Your turn: ");
                        playerController.Move(opponent);
                        Console.Write("Their turn: ");
                        ai.Move(player);
                    } else {
                        Console.Write("Their turn: ");
                        ai.Move(player);
                        Console.Write("Your turn: ");
                        playerController.Move(opponent);
                    }

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Round over!");

                    if (player.Health == 0) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("You died.");
                    } else if (opponent.Health == 0)
                        Console.WriteLine("Your opponent died.");

                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine($"Player health: {player.Health / player.MaxHealth:0.##%}");
                    Console.WriteLine($"    AI health: {opponent.Health / opponent.MaxHealth:0.##%}");
                    Console.WriteLine("Press enter");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.ReadLine();
                    Console.Clear();
                }
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Game Over. Press enter");
            Console.ReadLine();
        }

        private static void Intro() {
            Console.ForegroundColor = ConsoleColor.DarkBlue;

            var info = ExtendedConsole.ConsoleScreenBufferInfoEx.GetCurrent();

            //var cache = info.GetColors();

            info.SetColor(ConsoleColor.DarkBlue, 0, 0, 0);
            info.SetColor(ConsoleColor.DarkGreen, 0, 0, 0);
            info.Height--;
            info.Width--; // Bug? The Width/height of GET is always one larger than SET
            info.Apply();

            Console.Clear();
            var titleWidth = info.Width / 2 - TitleArt[0].Length / 2;
            Console.SetCursorPosition(titleWidth, info.Height / 2 - TitleArt.Length / 2 - 1);
            foreach (var t in TitleArt) {
                Console.WriteLine(t);
                Console.CursorLeft = titleWidth;
            }

            Console.SetCursorPosition(info.Width / 2 - StartMsg.Length / 2, Console.CursorTop + 1);
            //Console.ForegroundColor = ConsoleColor.DarkGreen;
            var clickLine = Console.CursorTop;
            var clickLeft = Console.CursorLeft;
            foreach (var c in StartMsg) {
                if (c != ' ')
                    Console.ForegroundColor++;
                Console.Write(c);
                info.SetColor(Console.ForegroundColor, 0, 0, 0);
            }
            //Console.Write(StartMsg);
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

            for (int i = 1; i < 255 * 3; i += 8) {
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
            //info.SetColors(cache);

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
            }
            info.Apply();
            Console.ForegroundColor = ConsoleColor.White;

            {
                var c = ConsoleColor.Black;
                for (var j = 0; j < 16; j++)
                {
                    Console.ForegroundColor = c;
                    Console.WriteLine(c++);
                }
            }
        }

        private static Player GetRace() {
            string line;
            int race;
            do {
                Console.WriteLine("Choose a race:");
                for (var i = 0; i < Player.PlayerTypes.Length; i++) {
                    var playerType = Player.PlayerTypes[i];
                    Console.WriteLine(
                        $"{i}: {playerType.GetCustomAttribute<DescriptionAttribute>()?.Description ?? playerType.Name}");
                }

                line = Console.ReadLine();
            } while (!int.TryParse(line, out race));
            var type = Player.PlayerTypes[race];

            Console.Clear();

            var player = (Player)Activator.CreateInstance(type);
            return player;
        }

        private static void PlayerSummary(Player player) {
            Console.Write($"{player.Name}, a {player.RaceName}, armed with ");
            player.Weapon.PrintHeader();

            var proc = player.GetProficiency(player.Weapon);
            Console.Write($"> {proc.GetDescription()} with {player.Weapon.BaseName}, having {player.Proficiency[player.Weapon.GetType()]:0.##}xp. ");
            if (proc != Proficiency.Master)
                Console.WriteLine($"(Next level at {player.GetNextProficiency(player.Weapon):0.##}xp)");
            else
                Console.WriteLine();
            double min, max;
            Player.GetProficiencyMultiplier(proc, out min, out max);
            Console.WriteLine($"> multiplier will be between {min:0.##%} and {max:0.##%}");
        }

        public static double GetRandomNumber(double minimum, double maximum) => Rand.NextDouble() * (maximum - minimum) + minimum;
    }
}

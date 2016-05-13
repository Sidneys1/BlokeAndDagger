using System;
using System.CodeDom;
using System.ComponentModel;
using System.Reflection;
using BlokeAndDagger.Base;
using BlokeAndDagger.Races;
using Rant;
using Rant.Vocabulary;

namespace BlokeAndDagger
{
    internal class Game
    {
        public static RantEngine Rant = new RantEngine(RantDictionary.FromDirectory(@"C:\Git\Rantionary"));

        public static void Play()
        {
            var player = ChooseRace();
            var playerController = new PlayerController(player);

            Console.Title = $"Bloke and Dagger - {player.RaceName} '{player.Name}'";
            Console.ForegroundColor = ConsoleColor.White;

            Console.Write("You are ");
            Player.PlayerSummary(player);
            Console.WriteLine();
            Console.WriteLine();


            while (player.Health > 0)
            {
                var opponent = Player.GetRandomRace();

                opponent.Weapon = (Weapon)Activator.CreateInstance(opponent.DefaultWeapon);
                opponent.Weapon.Setup(opponent, (Renown)Program.Rand.GetRandomEnumValue<Renown>());

                var ai = new AIController(opponent);

                Console.Write("Your opponent is ");
                Player.PlayerSummary(opponent);
                Console.WriteLine();
                Console.WriteLine();

                var first = Program.Rand.Next(2) == 1;
                Console.WriteLine("Rolling dice...");
                Console.WriteLine(first ? "You will make the first move!" : "Your opponent will make the first move!");
                Console.WriteLine();

                while (player.Health > 0 && opponent.Health > 0)
                {
                    if (first)
                    {
                        Console.Write("Your turn: ");
                        playerController.Move(opponent);
                        Console.Write("Their turn: ");
                        ai.Move(player);
                    }
                    else
                    {
                        Console.Write("Their turn: ");
                        ai.Move(player);
                        Console.Write("Your turn: ");
                        playerController.Move(opponent);
                    }

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Round over!");

                    if (player.Health == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("You died.");
                    }
                    else if (opponent.Health == 0)
                        Console.WriteLine("Your opponent died.");

                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine($"Player health: {player.Health/player.MaxHealth:0.##%}");
                    Console.WriteLine($"    AI health: {opponent.Health/opponent.MaxHealth:0.##%}");
                    Console.WriteLine("Press enter");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.ReadLine();
                    Console.Clear();
                }
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Game Over. Press enter");
        }

        private static Player ChooseRace() {
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
            } while (!Int32.TryParse(line, out race));
            var type = Player.PlayerTypes[race];

            Console.Clear();

            var player = (Player)Activator.CreateInstance(type);

            player.Weapon = (Weapon) Activator.CreateInstance(player.DefaultWeapon);
            var renown = (Renown)Program.CommandLine.GetValue("--InitialRenown", Program.Rand.GetRandomEnumValue<Renown>());
            player.Weapon.Setup(player, renown);
            return player;
        }
    }
}
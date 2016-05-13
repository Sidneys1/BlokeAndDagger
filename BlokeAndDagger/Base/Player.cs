using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using BlokeAndDagger.Races;

namespace BlokeAndDagger.Base
{
    public abstract class Player
    {
        public static Type[] PlayerTypes { get; }

        static Player()
        {
            PlayerTypes =
                Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => t.IsClass && t.IsSubclassOf(typeof(Player)) && !t.IsAbstract)
                    .ToArray();
        }

        #region Properties

        public double Health { get; protected set; }
        public abstract double MaxHealth { get; }
        public string RaceName => GetType().GetCustomAttribute<DescriptionAttribute>()?.Description ?? this.GetType().Name;

        public abstract string Name { get; }
        public Dictionary<Type, double> Proficiency { get; } = new Dictionary<Type, double>();
        public abstract Weapon Weapon { get; set; }

        public abstract Type DefaultWeapon { get; }

        #endregion Properties

        #region Methods

        public static void GetProficiencyMultiplier(Proficiency p, out double minDmg, out double maxDmg) {
            switch (p) {
                case Base.Proficiency.Idiot:
                    minDmg = 0.25 / 2;
                    maxDmg = 0.25;
                    break;
                case Base.Proficiency.Untrained:
                    minDmg = 0.25;
                    maxDmg = 0.5;
                    break;
                case Base.Proficiency.Average:
                    minDmg = 0.5;
                    maxDmg = 1;
                    break;
                case Base.Proficiency.Comfortable:
                    minDmg = 1;
                    maxDmg = 2;
                    break;
                case Base.Proficiency.Adept:
                    minDmg = 2;
                    maxDmg = 4;
                    break;
                default: // master
                    minDmg = 4;
                    maxDmg = 8;
                    break;
            }
        }

        public void AddProficiency(Weapon weapon, double damage) {
            var type = weapon.GetType();
            Proficiency[type] = Proficiency[type] + damage;
        }

        public double GetNextProficiency(Weapon weapon) {
            var mul = weapon.Renown;
            var bd = weapon.Damage;
            var type = weapon.GetType();
            if (!Proficiency.ContainsKey(type)) Proficiency.Add(type, 0);

            var p = Proficiency[type];
            var multi = Weapon.GetRenownMultiplier(mul);
            if (p < 3 * multi * bd) return 3 * multi * bd;
            if (p < 9 * multi * bd) return 9 * multi * bd;
            if (p < 27 * multi * bd) return 27 * multi * bd;
            if (p < 81 * multi * bd) return 81 * multi * bd;
            if (p < 243 * multi * bd) return 243 * multi * bd;
            return 0;
        }

        public Proficiency GetProficiency(Weapon weapon) {
            var mul = weapon.Renown;
            var bd = weapon.Damage;
            var type = weapon.GetType();
            if (!Proficiency.ContainsKey(type)) Proficiency.Add(type, 0);

            var p = Proficiency[type];
            var multi = Weapon.GetRenownMultiplier(mul);
            if (p < 3 * multi * bd) return Base.Proficiency.Idiot;
            if (p < 9 * multi * bd) return Base.Proficiency.Untrained;
            if (p < 27 * multi * bd) return Base.Proficiency.Average;
            if (p < 81 * multi * bd) return Base.Proficiency.Comfortable;
            if (p < 243 * multi * bd) return Base.Proficiency.Adept;
            return Base.Proficiency.Master;
        }

        public void TakeDamage(double amount) {
            amount = Math.Min(Health, amount);
            Health -= amount;
        }

        internal static Player GetRandomRace() => new Human();

        #endregion Methods

        public static void PlayerSummary(Player player) {
            Console.Write($"{player.Name}, a {player.RaceName}, armed with ");
            player.Weapon.PrintHeader();

            var proc = player.GetProficiency(player.Weapon);
            Console.Write($"> {proc.GetDescription()} with {player.Weapon.BaseName}, having {player.Proficiency[player.Weapon.GetType()]:0.##}xp. ");
            if (proc != Base.Proficiency.Master)
                Console.WriteLine($"(Next level at {player.GetNextProficiency(player.Weapon):0.##}xp)");
            else
                Console.WriteLine();
            double min, max;
            Player.GetProficiencyMultiplier(proc, out min, out max);
            Console.WriteLine($"> multiplier will be between {min:0.##%} and {max:0.##%}");
        }
    }
}
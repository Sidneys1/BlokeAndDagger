using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using BlokeAndDagger.Helpers;
using BlokeAndDagger.Races;

using EConsole = ExtendedConsole.ExtendedConsole;

namespace BlokeAndDagger.Base {
    public abstract class Weapon {
        public static Type[] WeaponTypes { get; }

        static Weapon() {
            WeaponTypes =
                Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => t.IsClass && t.IsSubclassOf(typeof(Weapon)) && !t.IsAbstract)
                    .ToArray();
        }

        #region Properties

        public virtual Attributes Attributes { get; } = Attributes.Indestructible;
        public abstract double BaseDamage { get; }
        public double Damage { get; protected set; }
        public abstract DamageTypes DamageType { get; }
        public double Durability { get; protected set; }
        public abstract Enchantments Enchantments { get; }
        public abstract Dictionary<Proficiency, string[]> FPAttackMessages { get; }
        public abstract double MaxDurability { get; }
        public abstract string BaseName { get; }
        public string Name { get; protected set; }
        public Renown Renown { get; protected set; }
        public abstract uint Tier { get; }
        public abstract Dictionary<Proficiency, string[]> TPAttackMessages { get; }

        #endregion Properties

        #region Methods

        public static double GetRenownMultiplier(Renown renown) {
            switch (renown) {
                case Renown.Junk:
                    return 0.25;

                case Renown.Crappy:
                    return 0.5;

                case Renown.Common:
                    return 1;

                case Renown.Renown:
                    return 2;

                case Renown.Legendary:
                    return 4;

                case Renown.Epic:
                    return 8;
            }
            return 16;
        }

        public double GetDamage(Proficiency p) {
            double minDmg;
            double maxDmg;
            Player.GetProficiencyMultiplier(p, out minDmg, out maxDmg);
            return Program.Rand.GetRandomNumber(minDmg, maxDmg) * Damage;
        }

        public void PrintHeader() {
            EConsole.PushConsoleColors();
            
            Renown.SetConsoleColors();
            Console.WriteLine(Name);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(" * ");
            Renown.PrintColor();
            Console.Write($" Tier {Tier} ");
            if (Attributes != Attributes.None) {
                Attributes.PrintFlagsColor();
                Console.Write(' ');
            }
            Console.WriteLine("Weapon");
            if (Enchantments != Enchantments.None) {
                Console.Write(" * Enchantments: ");
                Enchantments.PrintFlagsColor();
                Console.WriteLine();
            }
            Console.Write($" * Does {BaseDamage} {DamageType.FlagsString()} Damage");
            if (Renown != Renown.Common) {
                Console.Write(", ");
                Renown.SetConsoleColors();
                Console.WriteLine($"x{GetRenownMultiplier(Renown):0.##} {Renown} Multiplier");
            } else
                Console.WriteLine();
            if (!Attributes.HasFlag(Attributes.Indestructible))
                Console.WriteLine($" * {Durability / MaxDurability * 100:0.##}% Durability");
            EConsole.PopConsoleColors();
        }

        public virtual void Setup(Player player, Renown? renown = null) {
            Renown = renown ?? (Renown)Program.Rand.GetRandomEnumValue<Renown>();

            Damage = BaseDamage * GetRenownMultiplier(Renown);
            Durability = MaxDurability;

            var name = player?.Name;

            switch (Renown) {
                case Renown.Renown:
                    name = name != null ? name.Split(' ')[0] : Game.Rant.Do("<name>");
                    Name = $"{name}'s {BaseName}";
                    break;
                case Renown.Legendary:
                    name = name?.Split(' ')[0] ?? "<name>";
                    Name =
                        $"{Game.Rant.Do($@"{name}[case:lower]'s[case:title]{{|(2)\s{{famed|celebrated|prominent|notable}}}}")} {BaseName}";
                    break;
                case Renown.Epic:
                    name = name ?? "<name> <surname>";
                    Name =
                        $"{Game.Rant.Do($@"[case:title]{name}{{|\sthe <adj-cn1>|\sthe <adj-cn2>}}[case:lower]'s")} {BaseName}";
                    break;
                case Renown.Godlike:
                    name = name ?? "<name> <surname>";
                    Name =
                        $"{Game.Rant.Do($@"[case:title]{name}{{(5)|[numfmt:roman]\s[num:2;19][numfmt:normal]}}{{(3)|(2)\sthe <adj-cn1>|\sthe <adj-cn2>}}[case:lower]'s[case:title]{{|\s<verb-violent.ing>}}")} {BaseName} of {Game.Rant.Do(@"[case:title]<adj-cn2.ness>")}";
                    break;
                default:
                    Name = BaseName;
                    break;
            }
        }

        public override string ToString() => Name;

        internal void TakeDurability(double bd) {
            if (Attributes.HasFlag(Attributes.Indestructible)) return;

            if (Enchantments.HasFlag(Enchantments.Tempered))
                bd *= 0.75D;
            else if (Enchantments.HasFlag(Enchantments.Tempered2))
                bd /= 2;
            else if (Enchantments.HasFlag(Enchantments.Tempered3))
                bd /= 4;

            Durability -= bd;
        }
        
        #endregion Methods
    }
}
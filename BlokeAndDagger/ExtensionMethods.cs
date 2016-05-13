using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using BlokeAndDagger.Helpers;

namespace BlokeAndDagger {
    public static class ExtensionMethods {
        #region Enum

        public static double GetWeight(this Enum value) =>
            (Attribute.GetCustomAttribute(value.GetType().GetField(value.ToString()), typeof(WeightAttribute)) as WeightAttribute)?.Weight ?? 1;

        public static string GetDescription(this Enum value) {

            var attribute
                = Attribute.GetCustomAttribute(value.GetType().GetField(value.ToString()), typeof(DescriptionAttribute))
                    as DescriptionAttribute;

            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static string FlagsString(this Enum value) {
            var vals =
                Enum.GetValues(value.GetType())
                    .OfType<Enum>()
                    .Where(value.HasFlag)
                    .Select(v => v.GetDescription())
                    .ToArray();
            if (vals.Length == 0) return string.Empty;
            if (vals.Length == 1) return vals[0];
            var b = new StringBuilder();
            b.Append(string.Join(", ", vals.Take(vals.Length - 1)));
            b.Append(vals.Length > 2 ? ", and " : " and ");
            b.Append(vals[vals.Length - 1]);
            return b.ToString();
        }

        public static void PrintFlagsColor(this Enum value) {
            ExtendedConsole.PushConsoleColors();
            //value.SetConsoleColors();
            var vals = Enum.GetValues(value.GetType()).OfType<Enum>().Where(value.HasFlag).ToArray();
            if (vals.Length == 0) return;
            vals[0].PrintColor();
            if (vals.Length > 1) {
                for (var i = 1; i < vals.Length - 1; i++) {
                    ExtendedConsole.PeekConsoleColors();
                    Console.Write(", ");
                    vals[i].PrintColor();
                }
                ExtendedConsole.PeekConsoleColors();
                Console.Write(vals.Length > 2 ? ", and" : " and ");
                vals[vals.Length - 1].PrintColor();
            }
            ExtendedConsole.PopConsoleColors();
        }

        public static void PrintColor(this Enum value) {
            ExtendedConsole.PushConsoleColors();
            value.SetConsoleColors();
            Console.Write(value.GetDescription());
            ExtendedConsole.PopConsoleColors();
        }

        public static ConsoleColorAttribute ConsoleColor(this Enum value) {
            return
                Attribute.GetCustomAttribute(value.GetType().GetField(value.ToString()), typeof(ConsoleColorAttribute))
                    as ConsoleColorAttribute;
        }

        public static void SetConsoleColors(this Enum value) {
            (Attribute.GetCustomAttribute(value.GetType().GetField(value.ToString()), typeof(ConsoleColorAttribute))
                as ConsoleColorAttribute)?.SetColors();
        }

        #endregion

        #region String/FormattedString

        public static FormattedText Reset(this string str)
            => new FormattedText(str, reset: true);

        public static FormattedText Blue(this string str, bool dark = false)
            => new FormattedText(str, dark ? System.ConsoleColor.DarkBlue : System.ConsoleColor.Blue);

        public static FormattedText Blue(this FormattedText str, bool dark = false) {
            str.ForegroundColor = dark ? System.ConsoleColor.DarkBlue : System.ConsoleColor.Blue;
            return str;
        }

        public static FormattedText BlueBack(this string str, bool dark = false)
            => new FormattedText(str, bcolor: dark ? System.ConsoleColor.DarkBlue : System.ConsoleColor.Blue);

        public static FormattedText BlueBack(this FormattedText str, bool dark = false) {
            str.BackgroundColor = dark ? System.ConsoleColor.DarkBlue : System.ConsoleColor.Blue;
            return str;
        }

        public static FormattedText Cyan(this string str, bool dark = false)
            => new FormattedText(str, dark ? System.ConsoleColor.DarkCyan : System.ConsoleColor.Cyan);

        public static FormattedText Cyan(this FormattedText str, bool dark = false) {
            str.ForegroundColor = dark ? System.ConsoleColor.DarkCyan : System.ConsoleColor.Cyan;
            return str;
        }

        public static FormattedText CyanBack(this string str, bool dark = false)
            => new FormattedText(str, bcolor: dark ? System.ConsoleColor.DarkCyan : System.ConsoleColor.Cyan);

        public static FormattedText CyanBack(this FormattedText str, bool dark = false) {
            str.BackgroundColor = dark ? System.ConsoleColor.DarkCyan : System.ConsoleColor.Cyan;
            return str;
        }

        public static FormattedText DarkGray(this string str, bool dark = false)
            => new FormattedText(str, dark ? System.ConsoleColor.Black : System.ConsoleColor.DarkGray);

        public static FormattedText DarkGray(this FormattedText str, bool dark = false) {
            str.ForegroundColor = dark ? System.ConsoleColor.Black : System.ConsoleColor.DarkGray;
            return str;
        }

        public static FormattedText DarkGrayBack(this string str, bool dark = false)
            => new FormattedText(str, bcolor: dark ? System.ConsoleColor.Black : System.ConsoleColor.DarkGray);

        public static FormattedText DarkGrayBack(this FormattedText str, bool dark = false) {
            str.BackgroundColor = dark ? System.ConsoleColor.Black : System.ConsoleColor.DarkGray;
            return str;
        }

        public static FormattedText Green(this string str, bool dark = false)
            => new FormattedText(str, dark ? System.ConsoleColor.DarkGreen : System.ConsoleColor.Green);

        public static FormattedText Green(this FormattedText str, bool dark = false) {
            str.ForegroundColor = dark ? System.ConsoleColor.DarkGreen : System.ConsoleColor.Green;
            return str;
        }

        public static FormattedText GreenBack(this string str, bool dark = false)
            => new FormattedText(str, bcolor: dark ? System.ConsoleColor.DarkGreen : System.ConsoleColor.Green);

        public static FormattedText GreenBack(this FormattedText str, bool dark = false) {
            str.BackgroundColor = dark ? System.ConsoleColor.DarkGreen : System.ConsoleColor.Green;
            return str;
        }

        public static FormattedText Magenta(this string str, bool dark = false)
            => new FormattedText(str, dark ? System.ConsoleColor.DarkMagenta : System.ConsoleColor.Magenta);

        public static FormattedText Magenta(this FormattedText str, bool dark = false) {
            str.ForegroundColor = dark ? System.ConsoleColor.DarkMagenta : System.ConsoleColor.Magenta;
            return str;
        }

        public static FormattedText MagentaBack(this string str, bool dark = false)
            => new FormattedText(str, bcolor: dark ? System.ConsoleColor.DarkMagenta : System.ConsoleColor.Magenta);

        public static FormattedText MagentaBack(this FormattedText str, bool dark = false) {
            str.BackgroundColor = dark ? System.ConsoleColor.DarkMagenta : System.ConsoleColor.Magenta;
            return str;
        }

        public static FormattedText Red(this string str, bool dark = false)
            => new FormattedText(str, dark ? System.ConsoleColor.DarkRed : System.ConsoleColor.Red);

        public static FormattedText Red(this FormattedText str, bool dark = false) {
            str.ForegroundColor = dark ? System.ConsoleColor.DarkRed : System.ConsoleColor.Red;
            return str;
        }

        public static FormattedText RedBack(this string str, bool dark = false)
            => new FormattedText(str, bcolor: dark ? System.ConsoleColor.DarkRed : System.ConsoleColor.Red);

        public static FormattedText RedBack(this FormattedText str, bool dark = false) {
            str.BackgroundColor = dark ? System.ConsoleColor.DarkRed : System.ConsoleColor.Red;
            return str;
        }

        public static FormattedText White(this string str, bool dark = false)
            => new FormattedText(str, dark ? System.ConsoleColor.Gray : System.ConsoleColor.White);

        public static FormattedText White(this FormattedText str, bool dark = false) {
            str.ForegroundColor = dark ? System.ConsoleColor.Gray : System.ConsoleColor.White;
            return str;
        }

        public static FormattedText WhiteBack(this string str, bool dark = false)
            => new FormattedText(str, bcolor: dark ? System.ConsoleColor.Gray : System.ConsoleColor.White);

        public static FormattedText WhiteBack(this FormattedText str, bool dark = false) {
            str.BackgroundColor = dark ? System.ConsoleColor.Gray : System.ConsoleColor.White;
            return str;
        }

        public static FormattedText Yellow(this string str, bool dark = false)
            => new FormattedText(str, dark ? System.ConsoleColor.DarkYellow : System.ConsoleColor.Yellow);

        public static FormattedText Yellow(this FormattedText str, bool dark = false) {
            str.ForegroundColor = dark ? System.ConsoleColor.DarkYellow : System.ConsoleColor.Yellow;
            return str;
        }

        public static FormattedText YellowBack(this string str, bool dark = false)
            => new FormattedText(str, bcolor: dark ? System.ConsoleColor.DarkYellow : System.ConsoleColor.Yellow);

        public static FormattedText YellowBack(this FormattedText str, bool dark = false) {
            str.BackgroundColor = dark ? System.ConsoleColor.DarkYellow : System.ConsoleColor.Yellow;
            return str;
        }

        #endregion

        #region IEnumerable

        public static IEnumerable<T> WithoutLast<T>(this IEnumerable<T> source) {
            using (var e = source.GetEnumerator()) {
                if (!e.MoveNext()) yield break;
                for (var value = e.Current; e.MoveNext(); value = e.Current) {
                    yield return value;
                }
            }
        }

        #endregion

        #region Random
        
        public static T GetWeightedRandom<T>(this Random rand, Dictionary<T, double> weights) {
            var sum = 0.0;
            var master = weights.Select(w => new {Element = w.Key, Weight = w.Value, Sum = sum += w.Value}).ToArray();
            var randomPoint = rand.NextDouble() * sum;
            int lowGuess = 0, highGuess = weights.Count - 1;
            while (highGuess >= lowGuess) {
                var guess = (lowGuess + highGuess) / 2;
                if (master[guess].Sum < randomPoint)
                    lowGuess = guess + 1;
                else if (master[guess].Sum - master[guess].Weight > randomPoint)
                    highGuess = guess - 1;
                else
                    return master[guess].Element;
            }
            return default(T);
        }

        public static readonly Dictionary<Type, Dictionary<Enum, double>> WeightCache = new Dictionary<Type, Dictionary<Enum, double>>();
        public static Enum GetRandomEnumValue<T>(this Random rand) where T : struct, IConvertible {
            var type = typeof(T);
            if (!type.IsEnum) throw new ArgumentException("T must be an enumerated type", nameof(T));
            Dictionary<Enum, double> cache;
            if (WeightCache.ContainsKey(type))
                cache = WeightCache[type];
            else {
                cache = Enum.GetValues(typeof(T)).OfType<Enum>().ToDictionary(t => t, GetWeight);
                WeightCache.Add(type, cache);
            }
            return rand.GetWeightedRandom(cache);
        }

        public static double GetRandomNumber(this Random rand, double minimum, double maximum) => rand.NextDouble() * (maximum - minimum) + minimum;

        #endregion
    }
}

using System;
using System.ComponentModel;
using BlokeAndDagger.Helpers;

namespace BlokeAndDagger.Base {
    [Flags]
    public enum Attributes {
        None = 1,
        [ConsoleColor(ConsoleColor.Gray)]
        Indestructible = 2
    }

    [Flags]
    public enum Enchantments : long {
        None = 1,
        [ConsoleColor(ConsoleColor.Gray)]
        Tempered = 2,
        [ConsoleColor(ConsoleColor.Gray), Description("Tempered II")]
        Tempered2 = 4,
        [ConsoleColor(ConsoleColor.Gray), Description("Tempered III")]
        Tempered3 = 8,
        [ConsoleColor(ConsoleColor.Green)]
        Poisonous = 16
    }

    public enum Proficiency {
        [Description("an Idiot")]
        Idiot,
        Untrained,
        Average,
        Comfortable,
        Adept,
        [Description("a Master")]
        Master
    }

    public enum Renown {
        [Weight(32)]                                          Junk,
        [Weight(16)] [ConsoleColor(ConsoleColor.White)]       Crappy,
        [Weight(08)]                                          Common,
        [Weight(08)] [ConsoleColor(ConsoleColor.Green)]       Renown,
        [Weight(04)] [ConsoleColor(ConsoleColor.DarkMagenta)] Legendary,
        [Weight(02)] [ConsoleColor(ConsoleColor.DarkMagenta)] Epic,
        [Weight(01)] [ConsoleColor(ConsoleColor.Yellow)]      Godlike
    }

    [Flags]
    public enum DamageTypes {
        Peircing = 1,
        Crushing = 2,
        Slicing = 4,
        Magic = 8,
        Meta = 16
    }
}

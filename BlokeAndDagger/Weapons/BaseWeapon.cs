using System.Collections.Generic;

namespace BlokeAndDagger.Weapons {
    public enum Renown {
        Junk,
        Crappy,
        Common,
        Renown,
        Legendary,
        Epic,
        Godlike
    }

    public enum Proficiency {
        Idiot,
        Untrained,
        Average,
        Comfortable,
        Adept,
        Master
    }

    public class BaseWeapon {
        public string Name { get; } = "Fists";

        public DamageTypes DamageType { get; } = DamageTypes.Crushing;

        //public DamageDelivery DamageDelivery { get; } = DamageDelivery.Proximity;

        public Renown Renown { get; } = Renown.Junk;

        public uint Tier = 0;

        public uint BaseDamage { get; } = 1;

        public Dictionary<Proficiency, string[]> FPAttackMessages { get; } = new Dictionary<Proficiency, string[]> {
            { Proficiency.Idiot, new [] {
                "You lash out wildly with your fists. You even land a blow or two.",
                "You really suck at this. But I guess you got a hit in."
            } },
            { Proficiency.Untrained, new [] {
                "Swing away, Merrill. Swing away.",
                "Well, you didn't hurt yourself."
            } },
            { Proficiency.Average, new [] {
                "Right in the kisser!",
                "That's gotta hurt!"
            } },
            { Proficiency.Comfortable, new [] {
                "He's going to need a steak on that.",
                "You heard bone crunch!"
            } },
            { Proficiency.Adept, new [] {
                "They never saw you coming.",
                "He even passed out for a few seconds."
            } },
            { Proficiency.Master, new [] {
                "You are unparalleled. Recovery will be slow and arduous for your opponent!",
                "I really need to write more of these messages. Also, get a better weapon already. For God's sake."
            } }
        };

        public Dictionary<Proficiency, string[]> TPAttackMessages { get; } = new Dictionary<Proficiency, string[]> {
            { Proficiency.Idiot, new [] {
                "Enemy Idiot Message"
            } },
            { Proficiency.Untrained, new [] {
                "Enemy Untrained Message"
            } },
            { Proficiency.Average, new [] {
                "Enemy Average Message"
            } },
            { Proficiency.Comfortable, new [] {
                "Enemy Comfortable Message"
            } },
            { Proficiency.Adept, new [] {
                "Enemy Adept Message"
            } },
            { Proficiency.Master, new [] {
                "Enemy Master Message"
            } }
        };
    }
}

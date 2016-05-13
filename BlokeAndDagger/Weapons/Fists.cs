using System.Collections.Generic;
using BlokeAndDagger.Base;

namespace BlokeAndDagger.Weapons {
    public class Fists : Weapon
    {
        public override string BaseName { get; } = "Fists";

        public override double BaseDamage { get; } = 4;
        
        public override DamageTypes DamageType { get; } = DamageTypes.Crushing;
        
        public override Enchantments Enchantments { get; } = Enchantments.None;

        public override Dictionary<Proficiency, string[]> FPAttackMessages { get; } = new Dictionary<Proficiency, string[]>
        {
            {
                Proficiency.Idiot, new[]
                {
                    "You lash out wildly with your fists. You even land a blow or two.", "You really suck at this. But I guess you got a hit in."
                }
            },
            {
                Proficiency.Untrained, new[]
                {
                    "Swing away, Merrill. Swing away.", "Well, you didn't hurt yourself."
                }
            },
            {
                Proficiency.Average, new[]
                {
                    "Right in the kisser!", "That's gotta hurt!"
                }
            },
            {
                Proficiency.Comfortable, new[]
                {
                    "He's going to need a steak on that.", "You heard bone crunch!"
                }
            },
            {
                Proficiency.Adept, new[]
                {
                    "They never saw you coming.", "He even passed out for a few seconds."
                }
            },
            {
                Proficiency.Master, new[]
                {
                    "You are unparalleled. Recovery will be slow and arduous for your opponent!", "I really need to write more of these messages. Also, get a better weapon already. For God's sake."
                }
            }
        };

        public override Dictionary<Proficiency, string[]> TPAttackMessages { get; } = new Dictionary<Proficiency, string[]>
        {
            {
                Proficiency.Idiot, new[]
                {
                    "Enemy Idiot Message"
                }
            },
            {
                Proficiency.Untrained, new[]
                {
                    "Enemy Untrained Message"
                }
            },
            {
                Proficiency.Average, new[]
                {
                    "Enemy Average Message"
                }
            },
            {
                Proficiency.Comfortable, new[]
                {
                    "Enemy Comfortable Message"
                }
            },
            {
                Proficiency.Adept, new[]
                {
                    "Enemy Adept Message"
                }
            },
            {
                Proficiency.Master, new[]
                {
                    "Enemy Master Message"
                }
            }
        };

        public override double MaxDurability { get; } = 100;
        
        public override uint Tier { get; } = 0;
    }
}
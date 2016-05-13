using System;
using BlokeAndDagger.Base;
using BlokeAndDagger.Weapons;

namespace BlokeAndDagger.Races {
    public class Human : Player
    {
        public Human()
        {
            Health = MaxHealth;
        }
        
        public override string Name { get; } = Game.Rant.Do("<name> <surname>");
        public sealed override Weapon Weapon { get; set; } = new Fists();
        public override Type DefaultWeapon { get; } = typeof(Fists);

        public sealed override double MaxHealth { get; } = 100;
    }
}

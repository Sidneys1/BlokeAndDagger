using BlokeAndDagger.Base;
using BlokeAndDagger.Weapons;

namespace BlokeAndDagger.Races {
    public class Human : Player
    {
        public Human()
        {
            Weapon.Setup(player:this);
            Health = MaxHealth;
        }
        
        public override string Name { get; } = Program.Rant.Do("<name> <surname>");
        public sealed override Weapon Weapon { get; protected set; } = new Fists();

        public sealed override double MaxHealth { get; } = 100;
    }
}

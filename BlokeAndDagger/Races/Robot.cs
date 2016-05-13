using BlokeAndDagger.Base;
using BlokeAndDagger.Weapons;

namespace BlokeAndDagger.Races {
    class Robot:Player
    {
        public Robot()
        {
            Weapon.Setup(player:this);
            Health = MaxHealth;
        }

        public sealed override double MaxHealth { get; } = 100;
        public override string Name { get; } = Program.Rant.Do(@"0x\2,x {Alpha|Beta|Gamma|Delta|Epsilon|Zeta|Eta|Theta|Iota|Kappa|Lambda|Mu|Nu|Xi|Omicron|Pi|Rho|Sigma|Tau|Upsilon|Phi|Chi|Psi|Omega}");
        public new static string RaceName { get; } = "Robot";
        public sealed override Weapon Weapon { get; protected set; } = new Clamps();
    }
}

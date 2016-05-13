using System;
using BlokeAndDagger.Base;

namespace BlokeAndDagger {
    internal class AIController : IController {
        private readonly Player _player;
        
        public AIController(Player player) {
            _player = player;
        }

        public void Move(Player opponent) {
            var proc = _player.GetProficiency(_player.Weapon);
            var bd = _player.Weapon.GetDamage(proc);

            var msgs = _player.Weapon.TPAttackMessages[proc];
            var m = msgs[Program.Rand.Next(msgs.Length)];
            Console.Write(m);
            Console.WriteLine($" They do {bd:F2}pts of damage!");
            Console.WriteLine();
            _player.AddProficiency(_player.Weapon, bd);
            _player.Weapon.TakeDurability(bd);
            opponent.TakeDamage(bd);
        }
    }
}
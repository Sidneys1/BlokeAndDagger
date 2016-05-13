using System;
using BlokeAndDagger.Base;

namespace BlokeAndDagger {
    internal class PlayerController : IController {
        private readonly Player _player;

        public PlayerController(Player player) {
            _player = player;
        }

        public void Move(Player opponent) {
            var proc = _player.GetProficiency(_player.Weapon);
            var bd = _player.Weapon.GetDamage(proc);
            
            var msgs = _player.Weapon.FPAttackMessages[proc];
            var m = msgs[Program.Rand.Next(msgs.Length)];
            Console.Write(m);
            Console.WriteLine($" You do {bd:F2}pts of damage!");
            Console.WriteLine();
            _player.AddProficiency(_player.Weapon, bd);
            _player.Weapon.TakeDurability(bd);
            opponent.TakeDamage(bd);
        }
    }
}
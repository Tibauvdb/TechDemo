using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.InputSystem;

namespace Game
{
    public class InteractCommand : IImpulseCommand
    {
        private readonly PlayerBase _player;

        public InteractCommand(PlayerBase player)
        {
            _player = player;
        }

        public void Execute()
        {
            _player.Interact();
        }
    }
}

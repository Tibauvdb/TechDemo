using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.InputSystem;
using Game.Player;

namespace Game
{
    public class InteractXCommand : IImpulseCommand
    {
        private readonly PlayerController _player;

        public InteractXCommand(PlayerController player)
        {
            _player = player;
        }

        public void Execute()
        {
            _player.CurrentState.InteractX();
        }
    }
}
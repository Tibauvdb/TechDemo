using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.InputSystem;

namespace Game
{
    public class JumpCommand : IImpulseCommand
    {
        private readonly SmallPlayer _smallPlayer;

        public JumpCommand(SmallPlayer smallPlayer)
        {
            _smallPlayer = smallPlayer;
        }

        public void Execute()
        {
            _smallPlayer.Jump();
        }
    }
}

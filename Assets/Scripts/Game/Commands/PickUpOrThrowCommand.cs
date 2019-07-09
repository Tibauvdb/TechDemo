using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.InputSystem;

namespace Game
{
    public class PickUpOrThrowCommand : IImpulseCommand
    {
        private readonly BigPlayer _bigPlayer;

        public PickUpOrThrowCommand(BigPlayer bigPlayer)
        {
            _bigPlayer = bigPlayer;
        }

        public void Execute()
        {
            _bigPlayer.PickUpOrThrow();
        }
    }
}

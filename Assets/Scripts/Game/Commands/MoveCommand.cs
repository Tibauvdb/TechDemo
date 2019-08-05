
using Game.InputSystem;
using Game.Player;
using UnityEngine;

namespace Game
{
    public class MoveCommand : IDirectionCommand
    {
        private PlayerController _player;

        public MoveCommand(PlayerController player)
        {
            _player = player;
        }
        public void Execute(Vector2 direction)
        {
            _player.CurrentState.Move(direction);
        }
    }
}

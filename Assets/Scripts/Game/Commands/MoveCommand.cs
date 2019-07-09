
using Game.InputSystem;
using UnityEngine;

namespace Game
{
    public class MoveCommand : IDirectionCommand
    {
        private PlayerBase _player;

        public MoveCommand(PlayerBase player)
        {
            _player = player;
        }
        public void Execute(Vector2 direction)
        {
            _player.Move(direction);
        }
    }
}

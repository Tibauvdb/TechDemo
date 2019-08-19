using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.GamePlay;
using Game.Player;
using UnityEngine;

namespace Assets.Scripts.Game.Player.PlayerStates
{
    class DeathState : BaseState
    {
        private readonly PlayerController _playerController;
        private readonly PlayerMotor _playerMotor;
        private readonly AnimationsController _animController;

        public DeathState(PlayerMotor playerMotor, PlayerController playerController, 
            AnimationsController animController)
        {
            _playerMotor = playerMotor;
            _playerController = playerController;
            _animController = animController;
        }
        public override void OnStateEnter(IInteractable interactable)
        {
            _animController.StartDeathAnimation();
        }

        public override void OnStateExit()
        {
        }

        public override void Update()
        {

        }

        public override void Move(Vector2 direction)
        {
            //No Moving Since Player Is Dead
        }

        public override void InteractA()
        {
        }

        public override void InteractB()
        {
        }

        public override void InteractX()
        {
        }

        public override void InteractY()
        {
        }
    }
}

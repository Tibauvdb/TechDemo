using System.Collections;
using System.Collections.Generic;
using Game.Player;
using UnityEngine;

namespace Game.Player
{
    public class NormalState : BaseState
    {
        private readonly PlayerMotor _playerMotor;
        private readonly PlayerController _playerController;
        private readonly AnimationsController _animController;

        public NormalState(PlayerMotor playerMotor, PlayerController playerController,
            AnimationsController animController)
        {
            _playerMotor = playerMotor;
            _playerController = playerController;
            _animController = animController;
        }

        // Update is called once per frame
        public override void OnStateEnter()
        {

        }

        public override void OnStateExit()
        {

        }

        public override void Update()
        {

        }

        public override void Move(Vector2 direction)
        {
            _playerMotor.IsWalking = _playerMotor.CheckIfWalking(direction);

            if (_playerMotor.IsGrounded)
            {
                _playerMotor.IsWalking = _playerMotor.CheckIfWalking(direction);

                _playerMotor.Movement = new Vector3(direction.x, 0, direction.y);
            }
        }



        public override void InteractA()
        {
            //Go into Attack State
        }

        public override void InteractB()
        {
            //Jump
            //_playerMotor.CanJump = true;

        }

        public override void InteractX()
        {

        }

        public override void InteractY()
        {
        }
    }
}

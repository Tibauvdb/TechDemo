using System.Collections;
using System.Collections.Generic;
using Game.GamePlay;
using Game.GamePlay.Weapons;
using Game.Player;
using Game.Player.PlayerStates;
using UnityEngine;

namespace Game.Player
{
    public class NormalState : BaseState
    {
        private readonly PlayerMotor _playerMotor;
        private readonly PlayerController _playerController;
        private readonly AnimationsController _animController;

        private float _weaponDrawCooldownTimer;
        private float _weaponDrawCooldown = 1.75f;
        public NormalState(PlayerMotor playerMotor, PlayerController playerController,
            AnimationsController animController)
        {
            _playerMotor = playerMotor;
            _playerController = playerController;
            _animController = animController;
        }

        // Update is called once per frame
        public override void OnStateEnter(IInteractable interactable)
        {
            _weaponDrawCooldownTimer = _weaponDrawCooldown;
        }

        public override void OnStateExit()
        {

        }

        public override void Update()
        {
            _weaponDrawCooldownTimer -= Time.deltaTime;
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
            if(_weaponDrawCooldownTimer<=0)
                _playerController.SwitchState<DrawingWeaponState>(_playerController.Weapon.GetComponent<BaseWeapon>());
        }

        public override void InteractB()
        {
        }

        public override void InteractX()
        {
            _playerMotor.PerformDashAttack();
        }

        public override void InteractY()
        {
        }
    }
}

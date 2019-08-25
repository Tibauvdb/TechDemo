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

        private float _healStartTimer;
        private float _timeUntilPlayerStartsHealing = 2f;
        private float _healTimer;
        private float _healEveryXSec = 2f;
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
            _timeUntilPlayerStartsHealing = 0;
            _healTimer = 0;
        }

        public override void OnStateExit()
        {

        }

        public override void Update()
        {
            _weaponDrawCooldownTimer -= Time.deltaTime;

            _playerMotor.CheckIfFalling();

            HealPlayer();
        }

        private void HealPlayer()
        {
            _healStartTimer += Time.deltaTime;

            if (!(_healStartTimer >= _timeUntilPlayerStartsHealing)) return;


            _healTimer += Time.deltaTime;
            if (_healTimer >= _healEveryXSec)
            {
                _playerController.AddHealth(1);
                _healTimer = 0;
            }
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

            if(_weaponDrawCooldownTimer<=0 && !_playerMotor.IsFalling)
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

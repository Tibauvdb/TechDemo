using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.GamePlay;
using Game.GamePlay.Weapons;
using UnityEngine;

namespace Game.Player.PlayerStates
{
    public class AttackingState : BaseState
    {
        private readonly PlayerMotor _playerMotor;
        private readonly PlayerController _playerController;
        private readonly AnimationsController _animController;

        private BaseWeapon _weaponController;

        private int _amountOfAttacks = 0;
        public AttackingState(PlayerMotor playerMotor, PlayerController playerController,
            AnimationsController animController)
        {
            _playerMotor = playerMotor;
            _playerController = playerController;
            _animController = animController;

            _weaponController = _playerController.Weapon.GetComponent<BaseWeapon>();
        }

        public override void OnStateEnter(IInteractable interactable)
        {
            

            _animController.LightAttack();
            //_weaponController = (Sword)interactable;

            //_weaponController.SetAttacking(true);
        }

        public override void OnStateExit()
        {
            //_weaponController.SetAttacking(false);
            ((Sword) _weaponController).Attacking = false;
        }

        public override void Update()
        {
            ((Sword)_weaponController).Attacking = true;

            _playerMotor.StopMoving();

        }

        public override void Move(Vector2 direction)
        {
        }

        public override void InteractA()
        {
            _animController.LightAttack();
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

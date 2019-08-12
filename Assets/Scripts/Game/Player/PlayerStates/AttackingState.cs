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

        private Sword _swordScript;
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

            _playerMotor.IsWalking = false;
            _animController.LightAttack();
            //_weaponController = (Sword)interactable;

            //_weaponController.SetAttacking(true);
            _swordScript = (Sword) _playerController.Weapon.GetComponent<IInteractable>();
            _swordScript.PlayAttackParticle();
        }

        public override void OnStateExit()
        {
            //_weaponController.SetAttacking(false);
            ((Sword) _weaponController).SetAttacking(true);
            _swordScript.StopAttackParticle();
        }

        public override void Update()
        {
            ((Sword)_weaponController).Attacking = true;

            _playerMotor.StopMoving();
            _playerMotor.SetRotation();
        }

        public override void Move(Vector2 direction)
        {
            _playerMotor.Movement = new Vector3(direction.x,0,direction.y);
        }

        public override void InteractA()
        {
            _animController.LightAttack();
        }

        public override void InteractB()
        {
            _animController.HeavyAttack();
        }

        public override void InteractX()
        {
        }

        public override void InteractY()
        {
        }
    }
}

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

        private bool _attacking = false;
        private float _attackTimer = 0;

        private Transform _currentTarget;
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
            ((Sword) _weaponController).SetAttacking(false);
            _swordScript.StopAttackParticle();
        }

        public override void Update()
        {
            ((Sword)_weaponController).Attacking = true;

            //_playerMotor.StopMoving();
            //_playerMotor.SetRotation();

            //_playerController.gameObject.transform.Translate(Vector3.forward *( Time.deltaTime * 5));

            if (_attacking)
            {
                Debug.Log("currently attacking");
                _attackTimer += Time.deltaTime;
                _playerMotor.MoveTo(_currentTarget);
                //_playerMotor.KnockBackEntity(_currentTarget);
                _playerMotor.RotateTo(_currentTarget);
                if (_attackTimer >= 1f)
                {
                    _attacking = false;
                    _attackTimer = 0;
                }
            }
        }

        public override void Move(Vector2 direction)
        {
            //_playerMotor.Movement = _playerController.gameObject.transform.forward;
        }

        public override void InteractA()
        {
            
            _animController.LightAttack();
            _currentTarget = _playerController.FindClosestDamageable();

                //_playerMotor.MoveTo(_currentTarget);  
                //_playerMotor.KnockBackEntity(_currentTarget);
            _attacking = true;
            _attackTimer = 0;


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

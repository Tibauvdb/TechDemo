using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamePlay.Weapons;
using UnityEngine;

namespace Game.Player
{
    public class AttackState :BaseState
    {
        private readonly PlayerMotor _playerMotor;
        private readonly PlayerController _playerController;
        private readonly AnimationsController _animController;

        private GameObject _weapon;
        private Material _weaponMaterial;
        private IWeapon _weaponController;

        private int _targetDissolveValue;

        private bool _prepareToExit = false;

        private float _maxDissolve = 0.9f;


        public AttackState(PlayerMotor playerMotor, PlayerController playerController,
            AnimationsController animController)
        {
            _playerMotor = playerMotor;
            _playerController = playerController;
            _animController = animController;

            _weapon = _playerController.Weapon;
            _weaponMaterial = _weapon.GetComponent<MeshRenderer>().material;
            _weaponController = _weapon.GetComponent<IWeapon>();

            _targetDissolveValue = 0;
        }

        private void StartWeaponAppearing()
        {
                _weaponMaterial.SetFloat("_DissolveAmount",Mathf.Lerp(_weaponMaterial.GetFloat("_DissolveAmount"),_targetDissolveValue,Time.deltaTime));
        }

        public override void OnStateEnter()
        {
            _targetDissolveValue = 0;
            //Start Sword Summon Animation
            _animController.ChangeLayerWeight(1);
            _animController.DrawWeapon();
            //Attack
            //Play Attack Animation
        }

        public override void OnStateExit()
        {
            _prepareToExit = false;
            //_animController.SheathWeapon();

        }

        public override void Update()
        {
            StartWeaponAppearing();

            if (_prepareToExit && _weaponMaterial.GetFloat("_DissolveAmount") > _maxDissolve)
            {
                Debug.Log("switching state to normal state");
                _playerController.SwitchState<NormalState>();                
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
            //Light Attack
            _animController.LightAttack();
        }

        public override void InteractB()
        {
            _animController.HeavyAttack();
        }

        public override void InteractX()
        {
        }

        public override void InteractY() //Remove Sword
        {
            _targetDissolveValue = 1;
            _prepareToExit = true;
            _animController.SheathWeapon();
        }
    }
}

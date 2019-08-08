using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.GamePlay;
using Game.GamePlay.Weapons;
using Game.Player.PlayerStates;
using GamePlay.Weapons;
using UnityEngine;

namespace Game.Player
{
    public class HoldingWeaponState :BaseState
    {
        private readonly PlayerMotor _playerMotor;
        private readonly PlayerController _playerController;
        private readonly AnimationsController _animController;

        private GameObject _weapon;
        private Material _weaponMaterial;
        private BaseWeapon _weaponController;

        private int _targetDissolveValue;

        private bool _prepareToExit = false;

        private float _maxDissolve = 0.9f;

        private float _sheathTimer = 0;
        private float _timeUntilSheath = 5f;
        public HoldingWeaponState(PlayerMotor playerMotor, PlayerController playerController,
            AnimationsController animController)
        {
            _playerMotor = playerMotor;
            _playerController = playerController;
            _animController = animController;



            _targetDissolveValue = 0;
        }

        private void StartWeaponAppearing()
        {
                _weaponMaterial.SetFloat("_DissolveAmount",Mathf.Lerp(_weaponMaterial.GetFloat("_DissolveAmount"),_targetDissolveValue,Time.deltaTime));
        }

        public override void OnStateEnter(IInteractable interactable)
        {
            _targetDissolveValue = 0;
            //Start Sword Summon Animation
            if (!_animController.GetCurrentDominantLayer(1))
            {
                _animController.ChangeLayerWeight(1);
                _animController.DrawWeapon();
            }

            if (interactable != null)
            {
                Debug.Log("getting weapon");
                GetWeapon(interactable);
            }

            _animController.StopNextLightAttack();
        }

        private void GetWeapon(IInteractable interactable)
        {
            _weapon = ((BaseWeapon)interactable).gameObject;
            _weaponMaterial = _weapon.GetComponent<MeshRenderer>().material;
            _weaponController = _weapon.GetComponent<BaseWeapon>();
        }
        public override void OnStateExit()
        {
            _prepareToExit = false;
            _sheathTimer = 0;
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

            SheathTimer();
        }

        private void SheathTimer()
        {
            _sheathTimer += Time.deltaTime;
            if (_sheathTimer > _timeUntilSheath && !_prepareToExit)
            {
                Debug.Log("resdFGdhsas");
                SheatheWeapon();
                _sheathTimer = 0;
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
            //Switch To Attacking State
            _playerController.SwitchState<AttackingState>(_weaponController);
            //Light Attack
            //_animController.LightAttack();
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
            SheatheWeapon();
        }

        private void SheatheWeapon()
        {
            _targetDissolveValue = 1;
            _prepareToExit = true;
            _animController.SheathWeapon();
        }
    }
}

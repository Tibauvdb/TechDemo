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
    class SheathingWeapon : BaseState
    {
        private readonly PlayerMotor _playerMotor;
        private readonly PlayerController _playerController;
        private readonly AnimationsController _animController;

        private int _targetDissolveValue = 1;

        private GameObject _weapon;
        private Material _weaponMaterial;
        private BaseWeapon _weaponController;

        public SheathingWeapon(PlayerMotor playerMotor, PlayerController playerController,
            AnimationsController animController)
        {
            _playerMotor = playerMotor;
            _playerController = playerController;
            _animController = animController;

        }
        public override void OnStateEnter(IInteractable interactable)
        {

            if(interactable!=null)
                GetWeapon(interactable);
            else
            GetWeapon(_playerController.Weapon.GetComponent<IInteractable>());

            _animController.SheathWeapon();

            _playerMotor.IsWalking = false;
        }

        private void GetWeapon(IInteractable interactable)
        {
            _weapon = ((BaseWeapon)interactable).gameObject;
            _weaponMaterial = _weapon.GetComponent<MeshRenderer>().material;
            _weaponController = _weapon.GetComponent<BaseWeapon>();
        }

        public override void OnStateExit()
        {
            _animController.ChangeLayerWeight(0);
        }

        public override void Update()
        {
            MakeWeaponDisappear();

            if(_weaponMaterial.GetFloat("_DissolveAmount")>=0.9f)
                _playerController.SwitchState<NormalState>();

            _playerMotor.StopMoving();
        }

        private void MakeWeaponDisappear()
        {
            _weaponMaterial.SetFloat("_DissolveAmount", Mathf.Lerp(_weaponMaterial.GetFloat("_DissolveAmount"), _targetDissolveValue, Time.deltaTime * 1.5f));
        }
        public override void Move(Vector2 direction)
        {
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

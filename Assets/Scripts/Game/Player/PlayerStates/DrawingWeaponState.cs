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
    class DrawingWeaponState : BaseState
    {
        private readonly PlayerMotor _playerMotor;
        private readonly PlayerController _playerController;
        private readonly AnimationsController _animController;

        private int _targetDissolveValue = 0;

        private GameObject _weapon;
        private List<Material> _weaponMaterials = new List<Material>();
        private IInteractable _weaponController;



        public DrawingWeaponState(PlayerMotor playerMotor, PlayerController playerController,
            AnimationsController animController)
        {
            _playerMotor = playerMotor;
            _playerController = playerController;
            _animController = animController;
        }

        private void StartWeaponAppearing()
        {
            foreach (var weaponMaterial in _weaponMaterials)
            {
                weaponMaterial.SetFloat("_DissolveAmount", Mathf.Lerp(weaponMaterial.GetFloat("_DissolveAmount"), _targetDissolveValue, Time.deltaTime * 1.5f));                
            }
        }
        public override void OnStateEnter(IInteractable interactable)
        {
            
            if (!_animController.GetCurrentDominantLayer(1))
            {
                _animController.ChangeLayerWeight(1);
                _animController.DrawWeapon();
            }

            if(interactable!=null)
                GetWeapon(interactable);

            _playerMotor.IsWalking = false;
        }

        private void GetWeapon(IInteractable interactable)
        {
            _weapon = ((BaseWeapon)interactable).gameObject;
            Material[] materials = _weapon.GetComponent<MeshRenderer>().materials;
            _weaponMaterials = materials.ToList();
            _weaponController = _weapon.GetComponent<BaseWeapon>();
        }

        public override void OnStateExit()
        {
        }

        public override void Update()
        {
            StartWeaponAppearing();

            if(_weaponMaterials[0].GetFloat("_DissolveAmount")<=0.1f)
                _playerController.SwitchState<HoldingWeaponState>(_weaponController);

            _playerMotor.StopMoving();
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

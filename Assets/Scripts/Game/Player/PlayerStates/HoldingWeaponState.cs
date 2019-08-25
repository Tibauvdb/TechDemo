using Game.GamePlay;
using Game.GamePlay.Weapons;
using Game.Player.PlayerStates;
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

        private float _sheathTimer = 0;
        private float _timeUntilSheath = 10f;

        public HoldingWeaponState(PlayerMotor playerMotor, PlayerController playerController,

            AnimationsController animController)
        {
            _playerMotor = playerMotor;
            _playerController = playerController;
            _animController = animController;
        }

        public override void OnStateEnter(IInteractable interactable)
        {
        
            if (interactable != null)
            {
                GetWeapon(interactable);
            }

            _animController.StopNextLightAttack();

            _sheathTimer = 0;
        }

        private void GetWeapon(IInteractable interactable)
        {
            _weapon = ((BaseWeapon)interactable).gameObject;
            _weaponMaterial = _weapon.GetComponent<MeshRenderer>().material;
            _weaponController = _weapon.GetComponent<BaseWeapon>();

            ((Sword)_weaponController).SetAttacking(false);
        }

        public override void OnStateExit()
        {
        }

        public override void Update()
        {
            _playerMotor.CheckIfFalling();

            SheathTimer();
        }

        private void SheathTimer()
        {
            _sheathTimer += Time.deltaTime;
            if (_sheathTimer > _timeUntilSheath)
                _playerController.SwitchState<SheathingWeapon>(_weaponController);


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
            if(!_playerMotor.IsFalling)
                _playerController.SwitchState<AttackingState>(_weaponController);
        }

        public override void InteractB()
        {
        }

        public override void InteractX()
        {
            //Dash
            _playerMotor.PerformDashAttack();
            _sheathTimer = 0;
        }

        public override void InteractY() //Sheathe Sword
        {
            if(!_playerMotor.IsFalling)
                SheatheWeapon();
        }

        private void SheatheWeapon()
        {
            _playerController.SwitchState<SheathingWeapon>(_weaponController);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.Player.PlayerStates;
using Game.GamePlay;
using UnityEngine;


namespace Game.Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerMotor))]
    [RequireComponent(typeof(Animator))]
    public class PlayerController : MonoBehaviour, IDamageable
    {
        public BaseState CurrentState;

        private PlayerMotor _playerMotor;
        private Animator _anim;

        private AnimationsController _animCont;
        public AnimationsController AnimCont => _animCont;
        private StateMachineController _stateMachineController;

        [SerializeField] private GameObject _weapon;
        public GameObject Weapon => _weapon;

        private int _health = 10;
        public int Health => _health;
        // Start is called before the first frame update

        void Start()
        {
            _playerMotor = GetComponent<PlayerMotor>();
            _anim = GetComponent<Animator>();
            _animCont = new AnimationsController(_anim,this);
            CurrentState = new NormalState(_playerMotor, this, _animCont);

            _stateMachineController = new StateMachineController(_playerMotor,this,_animCont);
        }

        // Update is called once per frame
        void Update()
        {
            Debug.Log(CurrentState);
            UpdateAnimations();

            CurrentState.Update();
            _animCont.Update();
        }

        private void UpdateAnimations()
        {

            //Blend Idle - Walking - Running Animation 
            _animCont.SetForwardMomentum(GetBiggestValue(Mathf.Abs(_playerMotor.Movement.x),Mathf.Abs(_playerMotor.Movement.z)));

            /*if(_playerMotor.CanJump && _playerMotor.IsGrounded)
                _animCont.StartJumpingAnimation();*/
        }

        public void SwitchState<T>(IInteractable interactableObject = null) where T : BaseState
        {
            CurrentState?.OnStateExit();

            CurrentState = _stateMachineController.GetState<T>(interactableObject);
            CurrentState.OnStateEnter(interactableObject);
        }

        private static float GetBiggestValue(float value1, float value2)
        {
            float temp =  Mathf.Abs(value1) + Mathf.Abs(value2);
            return temp > 1 ? 1 : temp;
        }

        public void TakeDamage(int damage)
        {
            _health -= damage;
            if(_health<=0)
                Die();
        }

        public void Die()
        {
            SwitchState<DeathState>();
        }
    }
}

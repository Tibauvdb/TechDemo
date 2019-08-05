using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerMotor))]
    [RequireComponent(typeof(Animator))]
    public class PlayerController : MonoBehaviour
    {
        public BaseState CurrentState;

        private PlayerMotor _playerMotor;
        private Animator _anim;

        private AnimationsController _animCont;
        // Start is called before the first frame update
        private void Awake()
        {
            _playerMotor = GetComponent<PlayerMotor>();
            CurrentState = new NormalState(_playerMotor,this);
        }
        void Start()
        {
            _anim = GetComponent<Animator>();
            _animCont = new AnimationsController(_anim);
        }

        // Update is called once per frame
        void Update()
        {
            UpdateAnimations();
        }

        private void UpdateAnimations()
        {
            //Debug.Log(_playerMotor.Movement.x);
            _animCont.SetForwardMomentum(_playerMotor.Velocity.z);
        }
    }
}

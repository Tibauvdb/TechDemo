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
            _animCont.SetForwardMomentum(GetBiggestValue(Mathf.Abs(_playerMotor.Movement.x),Mathf.Abs(_playerMotor.Movement.z)));
        }

        private static float GetBiggestValue(float value1, float value2)
        {
            float temp =  Mathf.Abs(value1) + Mathf.Abs(value2);
            return temp > 1 ? 1 : temp;
            return value1 > value2 ? value1 : value2;

        }
    }
}

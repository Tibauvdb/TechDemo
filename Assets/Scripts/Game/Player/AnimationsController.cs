using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Player
{
    public class AnimationsController
    {
        private Animator _anim;

        private static readonly int _forwardMomentumParameter = Animator.StringToHash("ForwardVelocity");
        private static int _startJumpString = Animator.StringToHash("StartJump");

        public AnimationsController(Animator animator)
        {
            _anim = animator;
        }

        public void SetForwardMomentum(float movement)
        {
            movement = Mathf.Abs(movement);
            _anim.SetFloat(_forwardMomentumParameter,movement);
        }

        public void StartJumpingAnimation()
        {
            _anim.SetTrigger(_startJumpString);
        }
    }
}

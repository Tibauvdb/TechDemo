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
        private static int _forwardMomentumParameter = Animator.StringToHash("ForwardVelocity");

        public AnimationsController(Animator animator)
        {
            _anim = animator;
        }

        public void SetForwardMomentum(float movement)
        {
            Debug.Log("MOVEMEVEMT : " + movement);
            _anim.SetFloat("ForwardVelocity",movement);
            Debug.Log(_anim.GetFloat(_forwardMomentumParameter));
            Debug.Log(_anim.GetFloat("ForwardVelocity"));
        }
    }
}

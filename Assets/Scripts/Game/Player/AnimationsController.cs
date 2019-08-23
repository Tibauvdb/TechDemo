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

        public readonly SwordSheathToWalking SwordSheathToWalkingSB;
        public readonly AttackOverBehaviour AttackOverBehaviourSB;
        private static readonly int _forwardMomentumParameter = Animator.StringToHash("ForwardVelocity");
        private static int _startJumpString = Animator.StringToHash("StartJump");

        private readonly int _attackLocomotion = Animator.StringToHash("Attacking Locomotion");
        private int _totalAnimationLayers = 2;
        private int _currentLayer;

        private int _speed = 5;

        public AnimationsController(Animator animator)
        {
            _anim = animator;
        }

        public void Update()
        {
            UpdateLayerWeights();
        }

        public void SetForwardMomentum(float movement)
        {
            movement = Mathf.Abs(movement);
            _anim.SetFloat(_forwardMomentumParameter,movement);
        }

        private void UpdateLayerWeights()
        {
            for (int i = 0; i < _totalAnimationLayers; i++)
            {
                if (i == _currentLayer)
                    _anim.SetLayerWeight(_currentLayer, Mathf.Lerp(_anim.GetLayerWeight(i),1,Time.deltaTime * _speed));
                else
                    _anim.SetLayerWeight(i, Mathf.Lerp(_anim.GetLayerWeight(i), 0, Time.deltaTime * _speed));

            }
        }

        public void ChangeLayerWeight(int layer)
        {
            _currentLayer = layer;
        }

        public void DrawWeapon()
        {
            _anim.SetTrigger("DrawSword");
        }

        public void SheathWeapon()
        {
            _anim.SetTrigger("SheathSword");
        }

        public void LightAttack()
        {
            _anim.SetTrigger("LightAttack");
        }

        public void StopNextLightAttack()
        {
            _anim.ResetTrigger("LightAttack");
        }

        public void HeavyAttack()
        {
            _anim.SetTrigger("HeavyAttack");
        }

        public void StartDeathAnimation()
        {
            _anim.SetTrigger("IsDying");
            _anim.SetBool("Dead",true);
        }

        public bool GetCurrentDominantLayer(int layer)
        {
            if (_anim.GetLayerWeight(layer) >= 0.95f)
                return true;

            return false;
        }

        public void HitAnimation()
        {
            _anim.SetTrigger("Hit");
        }

        public void JumpDownAnimation()
        {
            _anim.SetTrigger("JumpDown");
        }

        public void Heal()
        {
            _anim.SetTrigger("Heal");
        }

        public void PrepareRangedAttack()
        {
            _anim.SetTrigger("StartRangedAttack");
        }

        public void ResetAnimation()
        {
            _anim.SetTrigger("Reset");
        }
    }
}

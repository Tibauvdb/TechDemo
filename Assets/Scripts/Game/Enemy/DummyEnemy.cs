using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.GamePlay;
using Game.Player;
using UnityEngine;

namespace Game.Enemy
{
    public class DummyEnemy : MonoBehaviour, IDamageable
    {
        private Animator _anim ;
        private int _health = 10;

        private bool _dead;
        private void Start()
        {
            _anim = GetComponent<Animator>();
        }

        public void TakeDamage(int damage)
        {
            if (_dead)
                return;
            _health -= damage;
            Debug.Log("Ouch!");
            if (_health > 0)
            {
                _anim.SetTrigger("Hit");
            }
            else
                Die();
        }

        public void Die()
        {
            Debug.Log("dyingsdfs");
            _anim.SetTrigger("IsDying");
            _anim.SetBool("Dead",true);
            _dead = true;
        }
    }
}

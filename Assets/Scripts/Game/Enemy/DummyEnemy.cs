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
        private List<Material> _dissolveMaterial = new List<Material>();
        private CharacterController _charCTRL;
        private void Start()
        {
            _anim = GetComponent<Animator>();
            SkinnedMeshRenderer[] smr = GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var mr in smr)
            {
                _dissolveMaterial.Add(mr.material);
            }

            _charCTRL = GetComponent<CharacterController>();
        }

        private void Update()
        {
            if (_dead)
            {
                foreach (var dissolve in _dissolveMaterial)
                {
                    dissolve.SetFloat("_DissolveAmount",Mathf.Lerp(dissolve.GetFloat("_DissolveAmount"),1,Time.deltaTime*0.5f));
                }
            }

            if(_dissolveMaterial[0].GetFloat("_DissolveAmount")>=0.9f)
                Destroy(this.gameObject);
                
        }
        public void TakeDamage(int damage)
        {
            if (_dead)
                return;
            _health -= damage;

            if (_health > 0)
            {
                _anim.SetTrigger("Hit");
            }
            else
                Die();

            _charCTRL.Move(Time.deltaTime * -transform.forward);
            
        }

        public void Die()
        {
            _anim.SetTrigger("IsDying");
            _anim.SetBool("Dead",true);
            _dead = true;
            GetComponent<CharacterController>().enabled = false;
        }

        public int GetHealth()
        {
            return _health;
        }
    }
}

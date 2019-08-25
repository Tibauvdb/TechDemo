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
        private float _health;
        private float _maxHealth = 3;

        private bool _dead;
        private List<Material> _dissolveMaterial = new List<Material>();
        private CharacterController _charCTRL;
        private Vector3 _velocity;
        public Vector3 Velocity => _velocity;

        private bool _isGrounded => _charCTRL.isGrounded;

        private Material _healthBarMaterial;
        private float _targetOpacity = 0;
        private void Start()
        {
            _anim = GetComponent<Animator>();
            SkinnedMeshRenderer[] smr = GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var mr in smr)
            {
                _dissolveMaterial.Add(mr.material);
            }

            _charCTRL = GetComponent<CharacterController>();

            _health = _maxHealth;
            _healthBarMaterial = transform.Find("HealthBar").GetComponent<MeshRenderer>().material;
        }

        private void FixedUpdate()
        {
            ApplyGround();

            _charCTRL.Move(_velocity * Time.deltaTime);

            _velocity = Vector3.zero;
        }
        private void Update()
        {
            if (_dead)
                DissolveOnDeath();

            _healthBarMaterial.SetFloat("_Opacity", Mathf.Lerp(_healthBarMaterial.GetFloat("_Opacity"), _targetOpacity, Time.deltaTime * 2));
            _healthBarMaterial.SetFloat("_HealthRemaining",_health/_maxHealth);
            Debug.Log(_targetOpacity);
        }

        public void TakeDamage(int damage)
        {

            if (_dead)
                return;
            _health -= damage;

            if (_health > 0)
            {
                _anim.SetTrigger("Hit");
                _targetOpacity = 1;
            }
            else
                Die();
          
        }

        public void Die()
        {
            _targetOpacity = 0;
            
            _anim.SetTrigger("IsDying");
            _anim.SetBool("Dead",true);
            _dead = true;
            GetComponent<CharacterController>().enabled = false;

        }

        public int GetHealth()
        {
            return (int)_health;
        }

        public void AddHealth(int amount)
        {
            
        }

        private void ApplyGround()
        {
            if (_isGrounded)
            {
                _velocity -= Vector3.Project(_velocity, Physics.gravity.normalized);
            }
        }

        private void DissolveOnDeath()
        {
            foreach (var dissolve in _dissolveMaterial)
            {
                dissolve.SetFloat("_DissolveAmount", Mathf.Lerp(dissolve.GetFloat("_DissolveAmount"), 1, Time.deltaTime * 0.5f));
            }

           
            if (_dissolveMaterial[0].GetFloat("_DissolveAmount") >= 0.9f)
                Destroy(this.gameObject);
        }
    }
}

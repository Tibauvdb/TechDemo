using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.GamePlay.Weapons
{
    class RangedProjectile : MonoBehaviour
    {
        private Vector3 _target = Vector3.zero;
        private int _damage = 5;

        private float _despawnTimer;
        private float _despawnTime = 5;

        private bool _fired = false;
        private float _projectileSpeed = 10;
        private void Start()
        {

        }

        private void Update()
        {
            if (_target != Vector3.zero)
            {
                transform.position = Vector3.MoveTowards(transform.position, _target, Time.deltaTime * _projectileSpeed);
                _despawnTimer += Time.deltaTime;

            }


            if(_despawnTimer>=_despawnTime)
                Destroy(this.gameObject);
        }

        public void SetTarget(Vector3 targetPos)
        {
            _target = targetPos;
            _fired = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_fired) return;
            if (other is SphereCollider) return;

            if (other.gameObject.tag == "Player")
            {
                if (other.GetComponent<IDamageable>() == null) return;
                IDamageable damageable = other.gameObject.GetComponent<IDamageable>();

                damageable.TakeDamage(_damage);
            
                Destroy(this.gameObject);
            }

            if(other.gameObject.tag!="Enemy")
                Destroy(this.gameObject);
        }

        private void OnCollisionEnter(Collision other)
        {
            if(!_fired) return;
            if (other.gameObject.tag == "Player")
            {
                Debug.Log("Entering Player collider");
                IDamageable damageable = other.gameObject.GetComponent<IDamageable>();

                damageable.TakeDamage(_damage);

                Destroy(this.gameObject);
            }
            
        }
        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Player;
using GamePlay.Weapons;
using UnityEngine;

namespace Game.GamePlay.Weapons
{
    public class Sword : MonoBehaviour,IWeapon
    {
        private int _swordDamage = 1;
        private AttackState _attackState;
        public AttackState AttackState => _attackState;

        public void SetStateScript()
        {

        }
        public void Hit()
        {
            
        }

        private void OnTriggerEnter(Collider other)
        {
            IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
            Debug.Log(other.gameObject.name);
            if (other.gameObject.GetComponent<IDamageable>() == null || other.gameObject.tag == "Player")
            {
                Debug.Log("Returning");
                return;
            }
            Debug.Log("Giving Damage");
            damageable.TakeDamage(_swordDamage);

            //play other Animations or something idk?
        }
    }
}

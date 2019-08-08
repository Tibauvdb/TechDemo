using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.GamePlay;
using UnityEngine;

namespace Game.Enemy
{
    public class DummyEnemy : MonoBehaviour, IDamageable
    {
        public void TakeDamage(int damage)
        {
            Debug.Log("Ouch!");
        }

        public void Die()
        {

        }
    }
}

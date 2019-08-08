using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.GamePlay;
using GamePlay.Weapons;
using UnityEngine;

namespace Game.GamePlay.Weapons
{
    public class BaseWeapon : MonoBehaviour,IWeapon,IInteractable
    {
        public virtual void Hit()
        {

        }

        public virtual void SetAttacking(bool state)
        {
        }
    }
}

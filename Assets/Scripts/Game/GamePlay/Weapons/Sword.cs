using Game.Player;
using UnityEngine;

namespace Game.GamePlay.Weapons
{
    public class Sword : BaseWeapon
    {
        private int _swordDamage = 1;
        private HoldingWeaponState _holdingWeaponState;
        public HoldingWeaponState HoldingWeaponState => _holdingWeaponState;


        public bool Attacking;

        public void SetStateScript()
        {

        }
        public void Hit()
        {
            
        }

        public override void SetAttacking(bool state)
        {
            Attacking = state;
        }

        private void OnTriggerEnter(Collider other)
        {
            IDamageable damageable = other.gameObject.GetComponent<IDamageable>();

            if (other.gameObject.GetComponent<IDamageable>() == null || other.gameObject.tag == "Player" || Attacking == false)
            {

                return;
            }

            damageable.TakeDamage(_swordDamage);

            //play other Animations or something idk?
        }
    }
}

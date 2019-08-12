using Game.Player;
using UnityEngine;

namespace Game.GamePlay.Weapons
{
    public class Sword : BaseWeapon
    {
        private int _swordDamage = 1;
        private HoldingWeaponState _holdingWeaponState;
        public HoldingWeaponState HoldingWeaponState => _holdingWeaponState;

        [SerializeField] private ParticleSystem _slashEffectPrefab;
        private ParticleSystem _slashEffect;
        [SerializeField] private TrailRenderer _trailRenderer;
        public bool Attacking;

        public void SetStateScript()
        {

        }
        public void Hit()
        {

        }

        public void PlayAttackParticle()
        {
            /*_slashEffect = Instantiate(_slashEffectPrefab,transform.position, Quaternion.identity, transform);
            _slashEffect.Play(true);*/
            _trailRenderer.emitting = true;

        }

        public void StopAttackParticle()
        {
            _trailRenderer.emitting = false;
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

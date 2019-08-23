using System.Collections;
using System.Collections.Generic;
using Game.BehaviourTree;
using Game.GamePlay.Weapons;
using Game.Player;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Enemy
{
    public class BaseEnemyBehaviour : MonoBehaviour
    {
        public INode BehaviourTree;
        public Coroutine TreeCoroutine;

        private Transform _transform;
        private EnemyMotor _enemyMotor;
        private Animator _anim;
        public AnimationsController AnimController;

        private Transform _playerTransform;
        private PlayerController _playerController;

        [Header("Enemy Stats")]
        [SerializeField] private float _maxHealth;
        private float _health;
        private bool _dead = false;
        [SerializeField] private float _fieldOfView;
        
        [Header("Attack Parameters")]
        public bool Ranged;
        public bool Attacking = false;
        [SerializeField] private int _attackRange;
        [SerializeField] private float _minAttackPrepTime;
        [SerializeField] private float _maxAttackPrepTime;
        private float _attackPrepTime;
        private float _attackPrepTimer;
        [Space]
        [SerializeField]private float _attackStunTime;
        private float _attackStunTimer;
        private bool _hasBeenAttacked;

        private float _targetOpacity = 0;

        private Material _healthBarMaterial;
        private List<Material> _dissolveMaterials = new List<Material>();

        [Header("Roaming Parameters")]
        private Vector3 _currentDestination;
        private float _roamCooldown;
        [SerializeField] private float _minRoamCooldown;
        [SerializeField] private float _maxRoamCooldown;
        private bool _inRoamCooldown = false;

        [SerializeField] private LayerMask _layerMask;

        public float BaseSpeed = 3.5f;

        public void Start()
        {
            #region InitVariables           

            _health = _maxHealth;
            _transform = transform;
            _enemyMotor = GetComponent<EnemyMotor>();
            _anim = GetComponent<Animator>();
            AnimController = new AnimationsController(_anim);

            SkinnedMeshRenderer[] smr = GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var mr in smr)
            {
                _dissolveMaterials.Add(mr.material);
            }

            _healthBarMaterial = _transform.Find("HealthBar").GetComponent<MeshRenderer>().material;

            _playerTransform = PlayerController.PlayerTransform;

            _playerController = _playerTransform.GetComponent<PlayerController>();

            #endregion
        }

        public void Update()
        {
            if (HasBeenAttacked())
                AttackStunTime();

            if (_dead)
                DissolveOnDeath();


            ShowHealthBar();
            UpdateAnimationController();
        }

        public virtual IEnumerator RunTree()
        {
            while (_health > 0)
            {
                yield return BehaviourTree.Tick();
            }
        }


        #region AttackedInteraction

        public IEnumerator<NodeResult> AttackReaction()
        {
            _enemyMotor.StopMoving(true);

            _enemyMotor.RotateTo(_playerTransform.position);

            if (_hasBeenAttacked)
                yield return NodeResult.Running;

            yield return NodeResult.Failure;
        }
        
        public void AttackStunTime()
        {
            _attackStunTimer += Time.deltaTime;

            if (_attackStunTimer >= _attackStunTime)
            {
                _attackStunTimer = 0;
                _hasBeenAttacked = false;
                _enemyMotor.StopMoving(false);
            }
        }
        
        public bool HasBeenAttacked()
        {
            return _hasBeenAttacked;
        }

        #endregion

        #region AttackingInteraction

        public IEnumerator<NodeResult> PrepareAttack()
        {
            _attackPrepTimer += Time.deltaTime;

            AnimController.ChangeLayerWeight(1);
            if (_attackPrepTimer >= _attackPrepTime)
            {
                _attackPrepTimer = 0;
                GenerateNewAttackPrepTime();
                yield return NodeResult.Succes;
            }

            yield return NodeResult.Failure;
        }
        
        public void GenerateNewAttackPrepTime()
        {
            _attackPrepTime = Random.Range(_minAttackPrepTime, _maxAttackPrepTime);
        }

        public IEnumerator<NodeResult> AttackPlayerRanged()
        {
            yield return NodeResult.Succes;
        }

        #endregion

        #region PlayerInteraction

        public float DistanceToPlayer()
        {
            return Vector3.Distance(_transform.position, _playerTransform.position);
        }
        
        public IEnumerator<NodeResult> SetPlayerAsTarget()
        {
            if (_playerController.GetHealth() > 0)
            {
                _enemyMotor.SetDestination(_playerTransform.position);
                _enemyMotor.RotateTo(_playerTransform.position);

                yield return NodeResult.Succes;
            }

            yield return NodeResult.Failure;
        }

        public bool CloseEnoughToPlayerToAttack()
        {
            //failure if not close enough
            return DistanceToPlayer() < _attackRange;
        }

        public bool CanSeePlayer()
        {
            Vector3 directionToPlayer = ((_playerTransform.position + Vector3.up) - (_transform.position + Vector3.up));
            if (Quaternion.Angle(_transform.rotation, Quaternion.LookRotation(directionToPlayer)) < _fieldOfView / 2)
            {
                RaycastHit hit;
                if (Physics.Raycast(_transform.position + Vector3.up, directionToPlayer, out hit, 100, _layerMask))
                {
                    Debug.Log("Can See Player");
                    if (hit.transform.gameObject.layer == 8)
                        return true;
                }
            }
            Debug.DrawRay(_transform.position,directionToPlayer,Color.red);
            return false;
        }
        #endregion

        #region RoamInteraction

        public bool IsRoamingOrWaitingToRoam()
        {
            if (_enemyMotor.HasNavMeshReachedDestination() && _roamCooldown <= 0 && _inRoamCooldown == false)
            {
                _roamCooldown = Random.Range(_minRoamCooldown, _maxRoamCooldown);
                _inRoamCooldown = true;
            }

            _roamCooldown -= Time.deltaTime;

            if (_roamCooldown <= 0 && _inRoamCooldown == true)
            {
                _inRoamCooldown = false;
                return true;
            }

            return false;
        }
        
        public IEnumerator<NodeResult> Roam()
        {
            AnimController.ChangeLayerWeight(0);

            _currentDestination = _enemyMotor.GetRandomDestination(_transform.position, 20, 1);

            _enemyMotor.SetDestination(_currentDestination);

            _enemyMotor.RotateTo(_currentDestination);
            yield return NodeResult.Succes;
        }

        #endregion

        #region IDamageable

        public void Die()
        {
            _enemyMotor.StopMoving(true);

            _targetOpacity = 0;

            _anim.SetTrigger("IsDying");
            _anim.SetBool("Dead", true);

            _dead = true;

            _playerController.RemoveFromList(this.gameObject);

            GetComponent<CapsuleCollider>().enabled = false;
        }
        
        public void TakeDamage(int damage)
        {
            if (_dead)
                return;

            if((int)_health ==(int)_maxHealth)
                Game.Instance.DamagedEnemies.Add(gameObject);

            _hasBeenAttacked = true;

            _attackStunTimer = 0;

            _health -= damage;
            AnimController.HitAnimation();

            if (_health <= 0)
                Die();
            else
                _targetOpacity = 1;
        }

        public int GetHealth()
        {
            return (int)_health;
        }

        #endregion

        public void UpdateAnimationController()
        {
            AnimController.SetForwardMomentum(GetBiggestValue(Mathf.Abs(_enemyMotor.GetNavMeshVelocity().x), Mathf.Abs(_enemyMotor.GetNavMeshVelocity().z)));

            AnimController.Update();
        }

        public void ShowHealthBar()
        {
            _healthBarMaterial.SetFloat("_Opacity", Mathf.Lerp(_healthBarMaterial.GetFloat("_Opacity"), _targetOpacity, Time.deltaTime * 2));
            _healthBarMaterial.SetFloat("_HealthRemaining", _health / _maxHealth);
        }

        public void DissolveOnDeath()
        {
            foreach (var dissolve in _dissolveMaterials)
            {
                dissolve.SetFloat("_DissolveAmount", Mathf.Lerp(dissolve.GetFloat("_DissolveAmount"), 1, Time.deltaTime * 0.5f));
            }

            if (_dissolveMaterials[0].GetFloat("_DissolveAmount") >= 0.9f)
                Destroy(this.gameObject);
        }

        public static float GetBiggestValue(float value1, float value2)
        {
            float temp = Mathf.Abs(value1) + Mathf.Abs(value2);
            return temp > 1 ? 1 : temp;
        }

        public void AddHealth(float amount)
        {
            _health += amount;
            _health = Mathf.Min(_maxHealth, _health);

            if ((int) _health == (int) _maxHealth)
                Game.Instance.DamagedEnemies.Remove(this.gameObject);
        }
    }
}



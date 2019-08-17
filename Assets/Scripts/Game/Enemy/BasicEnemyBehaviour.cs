using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.BehaviourTree;
using Game.GamePlay;
using Game.Player;
using Unity.Jobs;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Enemy
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(EnemyMotor))]
    class BasicEnemyBehaviour : MonoBehaviour,IDamageable
    {
        private INode _behaviourTree;
        private Coroutine _treeCoroutine;

        private Transform _transform;
        private EnemyMotor _enemyMotor;
        private Animator _anim;

        private AnimationsController _animationsController;
        private Transform _playerTransform;
        private PlayerController _playerController;

        private float _maxHealth = 3;
        private float _health;
        private bool _dead = false;
        [SerializeField] private float _fieldOfView;
        [SerializeField] private int _attackRange;
        [SerializeField] private float _attackStunTime;
        private float _attackStunTimer;
        private bool _hasBeenAttacked;
        private float _targetOpacity = 0;

        private Material _healthBarMaterial;
        private List<Material> _dissolveMaterials = new List<Material>();

        private Vector3 _currentDestination;
        private float _roamCooldown;
        private float _minRoamCooldown = 1f;
        private float _maxRoamCooldown = 5f;
        private bool _inRoamCooldown = false;
        private void Start()
        {
            _health = _maxHealth;
            _transform = transform;
            _enemyMotor = GetComponent<EnemyMotor>();
            _anim = GetComponent<Animator>();
            _animationsController = new AnimationsController(_anim);
            SkinnedMeshRenderer[] smr = GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var mr in smr)
            {
                _dissolveMaterials.Add(mr.material);
            }

            _healthBarMaterial = _transform.Find("HealthBar").GetComponent<MeshRenderer>().material;

            _playerTransform = PlayerController.PlayerTransform;
            _playerController = _playerTransform.GetComponent<PlayerController>();


            _behaviourTree = new SelectorNode(
            new SequenceNode(
                 new ConditionNode(HasBeenAttacked),
                            new ActionNode(AttackReaction)),
                            //Add Set Player As Target?
                        new SequenceNode(
                 new ConditionNode(CanSeePlayer),
                            new ActionNode(SetPlayerAsTarget),
                            new SequenceNode(
                     new ConditionNode(CloseEnoughToPlayerToAttack),
                                new ActionNode(PrepareAttack),
                                new ActionNode(AttackPlayer))),
                        new SequenceNode(
                 new ConditionNode(IsCoverClose),
                            new ActionNode(MoveToCover)),
                        new SequenceNode(
                 new ConditionNode(IsRoamingOrWaitingToRoam),
                            new ActionNode(Roam)));

            _treeCoroutine = StartCoroutine(RunTree());
        }

        private void Update()
        {
            if (HasBeenAttacked())
                AttackStunTime();

            if(_dead)
                DissolveOnDeath();

            Dissolve();

            _animationsController.SetForwardMomentum(GetBiggestValue(Mathf.Abs(_enemyMotor.GetNavMeshVelocity().x),Mathf.Abs(_enemyMotor.GetNavMeshVelocity().z)));
        }
        private static float GetBiggestValue(float value1, float value2)
        {
            float temp = Mathf.Abs(value1) + Mathf.Abs(value2);
            return temp > 1 ? 1 : temp;
        }
        IEnumerator RunTree()
        {
            while (_health > 0)
            {
                yield return _behaviourTree.Tick();
            }
        }
        private void AttackStunTime()
        {
            _attackStunTimer += Time.deltaTime;

            if (_attackStunTimer >= _attackStunTime)
                _hasBeenAttacked = false;
        }

        private float DistanceToPlayer()
        {
            return Vector3.Distance(_transform.position, _playerTransform.position);
        }

        private IEnumerator<NodeResult> PrepareAttack()
        {
            yield return NodeResult.Succes;
        }

        private IEnumerator<NodeResult> AttackPlayer()
        {
            yield return NodeResult.Succes;
        }


        private IEnumerator<NodeResult> SetPlayerAsTarget()
        {
            if (_playerController.GetHealth() > 0)
            {
                _enemyMotor.SetDestination(_playerTransform.position);
            }

            yield return NodeResult.Succes;
        }

        private IEnumerator<NodeResult> MoveToCover()
        {
            yield return NodeResult.Succes;
        }

        private IEnumerator<NodeResult> AttackReaction()
        {
            yield return NodeResult.Succes;
        }

        private bool CloseEnoughToPlayerToAttack()
        {
            return true;
        }

        private bool CanSeePlayer()
        {
            Vector3 directionToPlayer = _playerTransform.position - _transform.position;

            if (Quaternion.Angle(_transform.rotation, Quaternion.LookRotation(directionToPlayer)) < _fieldOfView / 2)
            {
                RaycastHit hit;
                if (Physics.Raycast(_transform.position, directionToPlayer, out hit, 100))
                {   
                    if (hit.transform.gameObject.layer == 8)
                        return true;
                }
            }
            return false;
        }

        private bool IsCoverClose()
        {
            return false;
        }

        private bool HasBeenAttacked()
        {
            return _hasBeenAttacked;
        }

        public void TakeDamage(int damage)
        {
            Debug.Log("OW");
            if (_dead)
                return;

            _hasBeenAttacked = true;

            _attackStunTimer = 0;

            _health -= damage;
            _animationsController.HitAnimation();

            if (_health <= 0)
                Die();
            else
                _targetOpacity = 1;
        }

        public void Die()
        {
            _enemyMotor.StopMoving(true);

            _targetOpacity = 0;

            _anim.SetTrigger("IsDying");
            _anim.SetBool("Dead", true);
            _dead = true;
            //GetComponent<CharacterController>().enabled = false;
        }

        public int GetHealth()
        {
            return (int) _health;
        }

        private void Dissolve()
        {
            _healthBarMaterial.SetFloat("_Opacity", Mathf.Lerp(_healthBarMaterial.GetFloat("_Opacity"), _targetOpacity, Time.deltaTime * 2));
            _healthBarMaterial.SetFloat("_HealthRemaining", _health / _maxHealth);
        }

        private void DissolveOnDeath()
        {
            foreach (var dissolve in _dissolveMaterials)
            {
                dissolve.SetFloat("_DissolveAmount", Mathf.Lerp(dissolve.GetFloat("_DissolveAmount"), 1, Time.deltaTime * 0.5f));
            }


            if (_dissolveMaterials[0].GetFloat("_DissolveAmount") >= 0.9f)
                Destroy(this.gameObject);
        }

        private bool IsRoamingOrWaitingToRoam()
        {
            if (_enemyMotor.HasNavMeshReachedDestination() && _roamCooldown <= 0 && _inRoamCooldown==false)
            {
                _roamCooldown = Random.Range(_minRoamCooldown, _maxRoamCooldown);
                _inRoamCooldown = true;
            }

            _roamCooldown -= Time.deltaTime;

            if (_roamCooldown <= 0 && _inRoamCooldown == true)
            {
                Debug.Log("End of roamcooldown");
                _inRoamCooldown = false;
                return true;
            }

            return false;
        }

        private IEnumerator<NodeResult> Roam()
        {
            _currentDestination =_enemyMotor.GetRandomDestination(_transform.position, 20, 1);
                                
            _enemyMotor.SetDestination(_currentDestination);

            _enemyMotor.RotateTo(_currentDestination);
            yield return NodeResult.Succes;
        }
    }
}


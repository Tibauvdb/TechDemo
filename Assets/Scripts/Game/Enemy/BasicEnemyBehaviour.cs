using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.BehaviourTree;
using Game.GamePlay;
using Game.Player;
using Unity.Jobs;
using UnityEngine;

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

        [SerializeField] private float _maxHealth;
        private float _health;

        [SerializeField] private float _fieldOfView;
        [SerializeField] private int _attackRange;
        [SerializeField] private float _attackStunTime;
        private float _attackStunTimer;
        private bool _hasBeenAttacked;

        private void Start()
        {
            _health = _maxHealth;
            _transform = transform;
            _enemyMotor = GetComponent<EnemyMotor>();
            _anim = GetComponent<Animator>();
            _animationsController = new AnimationsController(_anim);

            _playerTransform = PlayerController.PlayerTransform;
            _playerController = _playerTransform.GetComponent<PlayerController>();


            _behaviourTree = new SequenceNode(
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
                            new ActionNode(MovetoCover)),
                        new ActionNode(Roam));
        }

        private void Update()
        {
            if (HasBeenAttacked())
                AttackStunTime();
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
        private IEnumerator<NodeResult> Roam()
        {
            _enemyMotor.Walk();
            _enemyMotor.SetDestination(_enemyMotor.GetRandomDestination(_transform.position,3,-1));
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

        private IEnumerator<NodeResult> MovetoCover()
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
            return true;
        }

        private bool HasBeenAttacked()
        {
            return _hasBeenAttacked;
        }

        public void TakeDamage(int damage)
        {
            _hasBeenAttacked = true;

            _attackStunTimer = 0;

            _health -= damage;
            _animationsController.HitAnimation();
            
            if(_health<=0)
                Die();
        }

        public void Die()
        {
            _enemyMotor.StopMoving(true);
        }

        public int GetHealth()
        {
            return (int) _health;
        }
    }
}

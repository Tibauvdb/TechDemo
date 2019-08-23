using System.Collections.Generic;
using Game.BehaviourTree;
using Game.GamePlay;
using Game.GamePlay.Weapons;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Enemy
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(EnemyMotor))]
    class RangedEnemyBehaviour : BaseEnemyBehaviour, IDamageable
    {
        [Header("Ranged Attack Parameters")]
        [SerializeField] private float _baseMana;
        [SerializeField] private float _maxMana;
        [SerializeField] private float _manaGainOnAttack;
        private float _currentMana;
        [SerializeField] private GameObject _attackPrefab;
        [SerializeField] private Transform _rightHand;

        [Header("Healing Parameters")]
        [SerializeField] private float _healCooldown;
        [SerializeField] private float _healManaCost;
        [SerializeField] private float _healAmount;
        private float _healCooldownTimer;
        private GameObject _allyToHeal;

        public bool PreparingAttack;
        private bool _readyToAttack;
        private bool _readyToHeal;
        
        private GameObject _attackGO;
        private List<GameObject> _attacks = new List<GameObject>();
        private void Start()
        {
            base.Start();

            _currentMana = _baseMana;
            //PreparingAttack = false;

            BehaviourTree = new SelectorNode(
     /*new SequenceNode(
         new ConditionNode(HasBeenAttacked),
                    new ActionNode(AttackReaction)),*/
                new SequenceNode(
         new ConditionNode(CanHealAlly),
                    new ActionNode(ChooseAllyToHeal),
                    new AlwaysSuccessNode(
                        new SequenceNode(
                 new ActionNode(PrepareHeal),
                            new ActionNode(HealAlly)))),
                new SequenceNode(
         new ConditionNode(CanSeePlayer),
                    new AlwaysSuccessNode(
                        new SequenceNode(
                 new ConditionNode(CloseEnoughToPlayerToAttack),
                            new ActionNode(PrepareRangedAttack),
                            new ActionNode(RangedAttack)))),
                new SequenceNode(
         new ConditionNode(IsRoamingOrWaitingToRoam),
                    new ActionNode(Roam)));

            TreeCoroutine = StartCoroutine(RunTree());
        }

        private void Update()
        {
            base.Update();

            UpdateCooldowns();
        }

        private void UpdateCooldowns()
        {
            if(_healCooldownTimer>=0)
                _healCooldownTimer -= Time.deltaTime;
        }

        #region HealingAbilityInteraction

        public bool CanHealAlly()
        {
            if (_healCooldownTimer<=0 && _currentMana>=_healManaCost)
                return true;

            return false;
        }

        public IEnumerator<NodeResult> ChooseAllyToHeal()
        {
            //Get Random Ally From List Of Damaged Allies To Heal
            if (Game.Instance.DamagedEnemies.Count == 0)
                yield return NodeResult.Failure;

            if (_allyToHeal != null)
                yield return NodeResult.Succes;

            _allyToHeal = Game.Instance.DamagedEnemies[Random.Range(0, Game.Instance.DamagedEnemies.Count)];
            yield return NodeResult.Succes;
        }

        public IEnumerator<NodeResult> PrepareHeal()
        {
            if (_readyToHeal)
                yield return NodeResult.Succes;

            yield return NodeResult.Failure;
        }

        public IEnumerator<NodeResult> HealAlly()
        {
            AnimController.Heal();

            _allyToHeal.GetComponent<BaseEnemyBehaviour>().AddHealth(_healAmount);

            _healCooldownTimer = _healCooldown;
            _currentMana -= _healManaCost;

            _allyToHeal = null;
            yield return NodeResult.Succes;
        }
        #endregion

        private IEnumerator<NodeResult> PrepareRangedAttack()
        {
            Debug.Log("preparing attack "+_readyToAttack + " | "+PreparingAttack);
            GetComponent<NavMeshAgent>().speed = 0f;

            if (!PreparingAttack && _attackGO==null)
            {
                _attackGO = Instantiate(_attackPrefab, _rightHand);
                PreparingAttack = true;
                AnimController.PrepareRangedAttack();
            }

            if (_readyToAttack)
                yield return NodeResult.Succes;

            //_enemyMotor.StopMoving(true);
            yield return NodeResult.Failure;
        }

        private IEnumerator<NodeResult> RangedAttack()
        {
            Debug.Log("PerformingAttack " + PreparingAttack);
            _attackGO.transform.parent = null;
            _attackGO.GetComponent<RangedProjectile>().SetTarget(PlayerTransform.position+ Vector3.up);
            _attackGO = null;

            //PreparingAttack = false;
            _readyToAttack = false;

            _currentMana += _manaGainOnAttack;

            //_enemyMotor.StopMoving(false);
            yield return NodeResult.Succes;
        }

        public void DonePreparingAttack()
        {
            _readyToAttack = true;
            Debug.Log("donePreparingAttack + : "+_readyToAttack);
        }

        public void DonePreparingHeal()
        {
            _readyToHeal = true;
        }

        private bool CanSeePlayer()
        {
            bool canSeePlayer = base.CanSeePlayer();

            if (PreparingAttack)
                return true;
            return canSeePlayer;
        }

        private bool CloseEnoughToPlayerToAttack()
        {
            bool distance = base.CloseEnoughToPlayerToAttack();

            if (PreparingAttack)
                return true;

            return distance;
        }
    }

}


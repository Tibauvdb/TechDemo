using System.Collections.Generic;
using Game.BehaviourTree;
using Game.GamePlay;
using UnityEngine;

namespace Game.Enemy
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(EnemyMotor))]
    class RangedEnemyBehaviour : BaseEnemyBehaviour, IDamageable
    {
        [Header("RangedEnemyExtraStats")]
        [SerializeField] private float _baseMana;
        [SerializeField] private float _maxMana;
        [SerializeField] private float _manaGainOnAttack;
        private float _currentMana;

        [Header("Healing Parameters")]
        [SerializeField] private float _healPreperationTime;
        private float _healPreperationTimer;
        [SerializeField] private float _healCooldown;
        [SerializeField] private float _healManaCost;
        [SerializeField] private float _healAmount;
        private float _healCooldownTimer;
        private GameObject _allyToHeal;

        private void Start()
        {
            base.Start();

            _currentMana = _baseMana;

            BehaviourTree = new SelectorNode(
     new SequenceNode(
         new ConditionNode(HasBeenAttacked),
                    new ActionNode(AttackReaction)),
                new SequenceNode(
         new ConditionNode(CanHealAlly),
                    new ActionNode(ChooseAllyToHeal),
                    new AlwaysSuccessNode(
                        new SequenceNode(
                 new ActionNode(PrepareHeal),
                            new ActionNode(HealAlly)))),
                new SequenceNode(
         new ConditionNode(CanSeePlayer),
                    new ActionNode(SetPlayerAsTarget),
                    new AlwaysSuccessNode(
                        new SequenceNode(
                 new ConditionNode(CloseEnoughToPlayerToAttack),
                            new ActionNode(PrepareAttack),
                            new ActionNode(AttackPlayerRanged)))),
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
            _healPreperationTimer += Time.deltaTime;
            if (_healPreperationTimer >= _healPreperationTime)
                yield return NodeResult.Succes;

            yield return NodeResult.Failure;
        }

        public IEnumerator<NodeResult> HealAlly()
        {
            Debug.Log("Healed Ally");
            _allyToHeal.GetComponent<BaseEnemyBehaviour>().AddHealth(_healAmount);

            _healCooldownTimer = _healCooldown;
            _currentMana -= _healManaCost;

            _allyToHeal = null;
            yield return NodeResult.Succes;
        }
        #endregion

        private IEnumerator<NodeResult> RangedAttack()
        {
            //Set To Attacking -> in animationUpdate
            //Play Animation ->here
            //Instantiate Object in Hand->here
            //Release Object in AnimationEvent ->animEvent
            //Gain Mana

            yield return NodeResult.Succes;
        }
    }

}


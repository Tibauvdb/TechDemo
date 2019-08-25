using System.Collections.Generic;
using Game.BehaviourTree;
using Game.GamePlay;
using Game.GamePlay.Weapons;
using UnityEngine;

namespace Game.Enemy
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(EnemyMotor))]
    class MeleeEnemyBehaviour : BaseEnemyBehaviour,IDamageable
    {

        [Header("Weapon Parameters")]
        [SerializeField] private GameObject _sword;
        private Sword _swordScript;
        private Material[] _swordMaterials;
        private float _swordDissolveTarget = 1;

        private void Start()
        {            
            base.Start();

            _swordScript = _sword.GetComponent<Sword>();
            _swordMaterials = _sword.GetComponent<MeshRenderer>().materials;

            BehaviourTree = new SelectorNode(
             new SequenceNode(
                 new ConditionNode(HasBeenAttacked),
                            new ActionNode(AttackReaction)),
                        new SequenceNode(
                 new ConditionNode(CanSeePlayer),
                            new ActionNode(SetPlayerAsTarget),
                            new AlwaysSuccessNode(
                                new SequenceNode(
                         new ConditionNode(CloseEnoughToPlayerToAttack),
                                    new ActionNode(PrepareAttack),
                                    new ActionNode(AttackPlayerWithSword)))),
                        new SequenceNode(
                 new ConditionNode(IsRoamingOrWaitingToRoam),
                            new ActionNode(Roam)));


            TreeCoroutine = StartCoroutine(RunTree());
        }

        private void Update()
        {
            base.Update();

            StartWeaponAppearing();

            _swordScript.Attacking = Attacking;
        }

        public IEnumerator<NodeResult> PrepareAttack()
        {
            _swordDissolveTarget = 0;
            IEnumerator<NodeResult> prepareAttack = base.PrepareAttack();

            return prepareAttack;
        }

        public IEnumerator<NodeResult> AttackPlayerWithSword()
        {
            if (!Attacking)
            {
                AnimController.LightAttack();
                _swordScript.Attacking = true;
            }
            yield return NodeResult.Failure;
        }

        public void StartWeaponAppearing()
        {
            if (GetHealth() <= 0) _swordDissolveTarget = 1;
            foreach (var weaponMaterial in _swordMaterials)
            {
                weaponMaterial.SetFloat("_DissolveAmount", Mathf.Lerp(weaponMaterial.GetFloat("_DissolveAmount"), _swordDissolveTarget, Time.deltaTime * 1.5f));
            }
        }

        public IEnumerator<NodeResult> Roam()
        {
            _swordDissolveTarget = 1;
            IEnumerator<NodeResult> roam = base.Roam();

            return roam;
        }
    }
}


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.BehaviourTree;
using Game.GamePlay;
using Game.GamePlay.Weapons;
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
        private float _attackStunTime =5f;
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

        [SerializeField] private float _attackPrepTime;
        private float _attackPrepTimer;

        public bool Attacking = false;

        [SerializeField] private GameObject _sword;
        private Sword _swordScript;
        private Material[] _swordMaterials;
        private float _swordDissolveTarget = 0;

        [SerializeField] private LayerMask _layerMask;
        private void Start()
        {
            #region InitVariables           
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

            _swordScript = _sword.GetComponent<Sword>();
            _swordMaterials = _sword.GetComponent<MeshRenderer>().materials;
            #endregion


            _behaviourTree = new SelectorNode(
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
                                    new ActionNode(AttackPlayer)))),
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

            _animationsController.Update();

            StartWeaponAppearing();

            _swordScript.Attacking = Attacking;
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
            {
                _attackStunTimer = 0;
                _hasBeenAttacked = false;
                _enemyMotor.StopMoving(false);
            }
        }

        private float DistanceToPlayer()
        {
            return Vector3.Distance(_transform.position, _playerTransform.position);
        }

        private IEnumerator<NodeResult> PrepareAttack()
        {
            Debug.Log("Preparing Attack");
            _attackPrepTimer += Time.deltaTime;

            _swordDissolveTarget = 0;
            _animationsController.ChangeLayerWeight(1);
            if (_attackPrepTimer >= _attackPrepTime)
            {
                _attackPrepTimer = 0;

                yield return NodeResult.Succes;
            }
            
            yield return NodeResult.Failure;
        }

        private IEnumerator<NodeResult> AttackPlayer()
        {          
            if (!Attacking)
            {
                Debug.Log("Starting Light Attack");


                _animationsController.LightAttack();
                _swordScript.Attacking = true;
            }

           /* if (Attacking)
            {
                Debug.Log("Currently Attacking");
                yield return NodeResult.Failure;
            }

            if (!Attacking && _animationsController.GetCurrentDominantLayer(1))
            {
                Debug.Log("Done with Attack");
                _swordScript.Attacking = false;
                yield return NodeResult.Failure;
            }*/
           yield return NodeResult.Failure;
        }

        private void StartWeaponAppearing()
        {
            foreach (var weaponMaterial in _swordMaterials)
            {
                weaponMaterial.SetFloat("_DissolveAmount", Mathf.Lerp(weaponMaterial.GetFloat("_DissolveAmount"), _swordDissolveTarget, Time.deltaTime * 1.5f));
            }
        }

        private IEnumerator<NodeResult> SetPlayerAsTarget()
        {
            if (_playerController.GetHealth() > 0)
            {
                foreach (Material dm in _dissolveMaterials)
                {
                    dm.SetColor("_Color0",Color.blue);
                }
                _enemyMotor.SetDestination(_playerTransform.position);
                _enemyMotor.RotateTo(_playerTransform.position);
            }

            yield return NodeResult.Succes;
        }

        private IEnumerator<NodeResult> MoveToCover()
        {
            yield return NodeResult.Succes;
        }

        private IEnumerator<NodeResult> AttackReaction()
        {
            _enemyMotor.StopMoving(true);
            
            _enemyMotor.RotateTo(_playerTransform.position);

            if(_hasBeenAttacked)
                yield return NodeResult.Running;

            yield return NodeResult.Failure;
        }

        private bool CloseEnoughToPlayerToAttack()
        {
            //failure if not close enough
            return DistanceToPlayer()<_attackRange;
        }

        private bool CanSeePlayer()
        {
            Vector3 directionToPlayer = ((_playerTransform.position+Vector3.up) - (_transform.position + Vector3.up));
            if (Quaternion.Angle(_transform.rotation, Quaternion.LookRotation(directionToPlayer)) < _fieldOfView / 2)
            {
                RaycastHit hit;
                if (Physics.Raycast(_transform.position + Vector3.up, directionToPlayer, out hit, 100,_layerMask))
                {
                    if (hit.transform.gameObject.layer == 8)
                    {
                        Debug.Log("Sees Player");
                         return true;
                    }
                }
                Debug.DrawRay(_transform.position + Vector3.up,directionToPlayer,Color.red);
            }
            Debug.Log("Doesnt See Player");
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

            _playerController.RemoveFromList(this.gameObject);

            GetComponent<CapsuleCollider>().enabled = false;

            _swordDissolveTarget = 1;
            
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
                _inRoamCooldown = false;
                return true;
            }

            return false;
        }

        private IEnumerator<NodeResult> Roam()
        {
            _animationsController.ChangeLayerWeight(0);
            _swordDissolveTarget = 1;
            foreach (Material dm in _dissolveMaterials)
            {
                dm.SetColor("_Color0", Color.red);
            }
            _currentDestination =_enemyMotor.GetRandomDestination(_transform.position, 20, 1);
                                
            _enemyMotor.SetDestination(_currentDestination);

            _enemyMotor.RotateTo(_currentDestination);
            yield return NodeResult.Succes;
        }
    }
}


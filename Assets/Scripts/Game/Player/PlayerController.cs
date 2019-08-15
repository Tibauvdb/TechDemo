using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.Player.PlayerStates;
using Cinemachine;
using Game.GamePlay;
using UnityEngine;


namespace Game.Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerMotor))]
    [RequireComponent(typeof(Animator))]
    public class PlayerController : MonoBehaviour, IDamageable
    {
        public static Transform PlayerTransform { get; private set; }

        public BaseState CurrentState;

        private PlayerMotor _playerMotor;
        private Animator _anim;
        [SerializeField] private CinemachineFreeLook _cameraBrain;
        [SerializeField] private float _dashFOV;
        [SerializeField] private ParticleSystem _dustParticleSystem;
        private float _originalFOV;
        private AnimationsController _animCont;
        public AnimationsController AnimCont => _animCont;
        private StateMachineController _stateMachineController;

        [SerializeField] private GameObject _weapon;
        public GameObject Weapon => _weapon;

        private int _health = 10;
        public int Health => _health;

        private SkinnedMeshRenderer[] _skinnedMeshRenderers;
        
        //[SerializeField]private List<IDamageable> _damageables = new List<IDamageable>();
        [SerializeField] private List<GameObject> _damageables = new List<GameObject>();
        void Start()
        {
            PlayerTransform = transform;
            _playerMotor = GetComponent<PlayerMotor>();
            _anim = GetComponent<Animator>();
            _animCont = new AnimationsController(_anim,this);
            CurrentState = new NormalState(_playerMotor, this, _animCont);

            _stateMachineController = new StateMachineController(_playerMotor,this,_animCont);

            _skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

            _originalFOV = _cameraBrain.m_Lens.FieldOfView;
        }

        // Update is called once per frame
        void Update()
        {
            UpdateAnimations();

            CurrentState.Update();
            _animCont.Update();

            DashVisuals(!_playerMotor.IsDashing);


            for (int i = 0; i < _damageables.Count; i++)
            {
                if (_damageables[i] == null)
                    _damageables.Remove(_damageables[i]);
            }

        }

        private void UpdateAnimations()
        {
            //Blend Idle - Walking - Running Animation 
            _animCont.SetForwardMomentum(GetBiggestValue(Mathf.Abs(_playerMotor.Movement.x),Mathf.Abs(_playerMotor.Movement.z)));
        }

        public void SwitchState<T>(IInteractable interactableObject = null) where T : BaseState
        {
            CurrentState?.OnStateExit();

            CurrentState = _stateMachineController.GetState<T>(interactableObject);
            CurrentState.OnStateEnter(interactableObject);
        }

        private static float GetBiggestValue(float value1, float value2)
        {
            float temp =  Mathf.Abs(value1) + Mathf.Abs(value2);
            return temp > 1 ? 1 : temp;
        }

        public void TakeDamage(int damage)
        {
            _health -= damage;
            if(_health<=0)
                Die();
        }

        public void Die()
        {
            SwitchState<DeathState>();
        }

        public int GetHealth()
        {
            return _health;
        }

        public void DashVisuals(bool value)
        {
            _dustParticleSystem.gameObject.SetActive(value);
        }

        public Transform FindClosestDamageable()
        {
                Transform bestTarget = null;
                float closestDistanceSqr = Mathf.Infinity;
                Vector3 currentPosition = transform.position;

                foreach (var potTarget in _damageables)
                {
                    Vector3 directionToTarget = potTarget.transform.position - currentPosition;
                    float dSqrToTarget = directionToTarget.sqrMagnitude;

                    if (dSqrToTarget < closestDistanceSqr)
                    {
                        closestDistanceSqr = dSqrToTarget;
                        bestTarget = potTarget.gameObject.transform;
                    }
                }
                return bestTarget;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<IDamageable>() == null || other.gameObject.layer == 8)
                return;

            
            _damageables.Add(other.gameObject);
        }
      
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.GetComponent<IDamageable>() == null)
                return;

            for (int i = _damageables.Count-1; i >=0; i--)
            {
                if (other.gameObject.Equals(_damageables[i]))
                    _damageables.Remove(_damageables[i]);
            }
        }
    }
}

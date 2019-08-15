using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Player;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

namespace Game.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(CharacterController))]
    public class EnemyMotor : MonoBehaviour
    {
        private NavMeshAgent _navMeshAgent;
        public CharacterController CharController { get; private set; }
        private Transform _transform;
        private Transform _playerTransform;
        private float _runningSpeed;
        private float _walkingSpeed;

        private float _rotationSpeed;
        private void Start()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            CharController = GetComponent<CharacterController>();

            _transform = transform;
        }

        public void Walk()
        {
            _navMeshAgent.speed = _walkingSpeed;
        }

        public void Run()
        {
            _navMeshAgent.speed = _runningSpeed;
        }

        private bool IsStandingStill()
        {
            return !_navMeshAgent.updatePosition || (_navMeshAgent.isOnNavMesh && _navMeshAgent.isStopped);
        }

        public void RotateToPlayer()
        {
            _transform.rotation = Quaternion.RotateTowards(_transform.rotation,
                Quaternion.LookRotation(Vector3.Scale(_playerTransform.position - _transform.position,
                    new Vector3(1, 0, 1))), Time.deltaTime * _rotationSpeed);
        }

        public bool HasNavMeshReachedDestination()
        {
            if (_navMeshAgent.pathPending) return false;

            if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
            {
                if (!_navMeshAgent.hasPath || _navMeshAgent.velocity.sqrMagnitude <= 0f)
                    return true;
            }
            return false;
        }

        public void SetDestination(Vector3 position)
        {
            _navMeshAgent.destination = position;
        }

        public bool IsOnNavMesh()
        {
            return _navMeshAgent.isOnNavMesh;
        }

        public void StopMoving(bool value)
        {
            _navMeshAgent.isStopped = value;
        }

        public Vector3 GetRandomDestination(Vector3 origin, float range, int layerMask)
        {
            Vector3 randomPos = UnityEngine.Random.insideUnitSphere * range;

            randomPos += origin;

            NavMeshHit navHit;
            NavMesh.SamplePosition(randomPos, out navHit, range, layerMask);

            return navHit.position;
        }
    }
}

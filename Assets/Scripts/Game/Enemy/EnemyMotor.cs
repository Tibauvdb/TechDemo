using System;
using System.Collections;
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
    [RequireComponent(typeof(Rigidbody))]
    public class EnemyMotor : MonoBehaviour
    {
        
        private NavMeshAgent _navMeshAgent;
        private Rigidbody _rb;
        private Transform _transform;
        private Transform _playerTransform;
        
        private float _runningSpeed=5f;
        private float _walkingSpeed=3.5f;

        private float _rotationSpeed = 360;

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _rb = GetComponent<Rigidbody>();
            
            _transform = transform;
        }

        IEnumerator Start()
        {
            while (true)
            {
                if (IsOnNavMeshLink())
                {
                    yield return StartCoroutine(JumpOffLink(1f, .8f));
                    _navMeshAgent.CompleteOffMeshLink();
                }

            yield return null;
            }
        }

        private void Update()
        {
        }

        public Vector3 GetNavMeshVelocity()
        {
            return _navMeshAgent.velocity;
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

        public void RotateTo(Vector3 pos)
        {
            _transform.rotation = Quaternion.RotateTowards(_transform.rotation,
                Quaternion.LookRotation(Vector3.Scale(pos - _transform.position,
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

            NavMesh.SamplePosition(randomPos, out NavMeshHit navHit, range, layerMask);

            return navHit.position;
        }

        public bool IsOnNavMeshLink()
        {
            return _navMeshAgent.isOnOffMeshLink;
        }

        public void RemoveAgentDestination()
        {
            _navMeshAgent.isStopped = true;
            _navMeshAgent.ResetPath();
        }

        private IEnumerator JumpOffLink(float height, float duration)
        {
            /*Vector3 lookDir = data.endPos - data.startPos;
            _transform.rotation = Quaternion.RotateTowards(_transform.rotation,
                Quaternion.LookRotation(lookDir), Time.deltaTime*50);*/



            _navMeshAgent.speed = 0;
            OffMeshLinkData data = _navMeshAgent.currentOffMeshLinkData;
            GetComponent<Animator>().SetTrigger("JumpDown");

            Vector3 startPos = _navMeshAgent.transform.position;
            Vector3 endPos = data.endPos + Vector3.up * _navMeshAgent.baseOffset;
            float normalizedTime = 0.0f;
            
            while (normalizedTime < 1.0f)
            {
                //if (Quaternion.Dot(_transform.rotation,Quaternion.LookRotation(lookDir)) >=0.95f)
                //{
                    float yOffset = height * 4.0f * (normalizedTime - normalizedTime * normalizedTime);
                    _navMeshAgent.transform.position =
                    Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
                    normalizedTime += Time.deltaTime / duration;
                //}
                    yield return null;
            }
        }
    }
}

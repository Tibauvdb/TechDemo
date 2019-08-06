using System;
using UnityEngine;

namespace Game.Player
{
    public class PlayerMotor : MonoBehaviour
    {
        public float MaxRunningSpeed = (30.0f * 1000) / (60 * 60);
        [SerializeField] private float _acceleration = 3;  //[m/s^2]
        [SerializeField] private float _jumpHeight = 1; //[m/s^2]
        [SerializeField] private float _dragOnGround = 1;
        [SerializeField] private float _dragOnMovementStop = 10;
        [SerializeField] private float _dragInAir = 1;

        [SerializeField] private float _walkingSpeedMultiplier;

        private CharacterController _charCont;
        private Transform _playerTransform;
        private Transform _cameraTransform;
        private Vector3 _movement;

        public bool IsWalking { get; set; }
        public bool CanJump { get; set; }
        public bool IsGrounded => _charCont.isGrounded;
        public bool HasGravity { get; set; } = true;

        public Vector3 Movement
        {
            get => _movement;
            set => _movement = value;
        }

        [SerializeField] private Vector3 _velocity = Vector3.zero;
        public Vector3 Velocity => _velocity;

        [SerializeField] private float _horizontalRotationSpeed;
        public float HorizontalRotationSpeed => _horizontalRotationSpeed;

        [SerializeField] private float _verticalRotationSpeed;
        public float VerticalRotationSpeed => _verticalRotationSpeed;

        [SerializeField] private LayerMask _mapLayerMask;

        [SerializeField] private IsGroundedChecker _isGroundedCheck;
        public IsGroundedChecker IsGroundedCheck => _isGroundedCheck;

        private Quaternion _lastRotation;
        private Vector2 _lastMovement;

        private void Start()
        {
            _charCont = GetComponent<CharacterController>();
            _playerTransform = transform;
            _cameraTransform = Camera.main.GetComponent<Transform>();
        }

        void FixedUpdate()
        {
            ApplyGround();
            ApplyGravity();

            ApplyMovement();
            ApplyGroundDrag();

            ApplyAirDrag();
            ApplyJump();

            LimitMaximumRunningSpeed();

            _charCont.Move(_velocity * Time.deltaTime);
        }

        private void ApplyMovement()
        {
            if (IsGrounded)
            {
                Debug.Log(Movement);
                Vector3 relativeMovement = RelativeDirection(Movement);
                SetPlayerRotation(relativeMovement);
                _velocity = relativeMovement * _acceleration; // F(= m.a) [m/s^2] * t [s]


                
            }
        }

        //move player according to camera forward
        private Vector3 RelativeDirection(Vector3 direction)
        {
            Vector3 moveDir = Vector3.Scale(_cameraTransform.TransformDirection(direction),new Vector3(1,0,1));
            return moveDir;
        }

        private void SetPlayerRotation(Vector3 relativeMovement)
        {
            if (IsWalking)
            {

                //transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward,relativeMovement,Time.deltaTime * 15,1.0f));
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(relativeMovement), Time.deltaTime * 10);
                //_lastRotation = transform.rotation;
            }
            /*else
                transform.rotation = Quaternion.LookRotation(_lastRotation);*/
        }
        private void ApplyGround()
        {
            if (IsGrounded)
            {
                _velocity -= Vector3.Project(_velocity, Physics.gravity.normalized);
            }
        }

        private void ApplyGravity()
        {
            if (HasGravity && !IsGrounded)
            {
                _velocity += Physics.gravity * Time.deltaTime; // g[m/s^2] * t[s]
            }

        }

        private void ApplyJump()
        {
            if (CanJump && IsGrounded)
            {
                _velocity += -Physics.gravity.normalized * Mathf.Sqrt(2 * Physics.gravity.magnitude * _jumpHeight);
                CanJump = false;
            }

        }

        private void ApplyAirDrag()
        {
            if (!IsGrounded)
            {
                Vector3 xzVelocity = Vector3.Scale(_velocity, new Vector3(1, 0, 1));
                xzVelocity = xzVelocity * (1 - Time.deltaTime * _dragInAir);

                xzVelocity.y = _velocity.y;
                _velocity = xzVelocity;
            }
        }

        private void ApplyGroundDrag()
        {
            if (IsGrounded)
            {
                _velocity *= (1 - Time.deltaTime * _dragOnGround);
                if(!IsWalking)
                    _velocity *= (1 - Time.deltaTime * (_dragOnGround * _dragOnMovementStop));
            }
        }

        private void LimitMaximumRunningSpeed()
        {
            Vector3 yVelocity = Vector3.Scale(_velocity, new Vector3(0, 1, 0));

            Vector3 xzVelocity = Vector3.Scale(_velocity, new Vector3(1, 0, 1));
            Vector3 clampedXzVelocity = Vector3.ClampMagnitude(xzVelocity, MaxRunningSpeed);

            _velocity = yVelocity + clampedXzVelocity;
        }

        public void StopMoving()
        {
            Movement = Vector3.zero;
            _velocity = Vector3.zero;
        }

        public float GetDistanceFromGround()
        {
            RaycastHit hit;
            if (Physics.Raycast(_playerTransform.position, Vector3.down, out hit, 1000, _mapLayerMask))
            {
                return (hit.point - _playerTransform.position).magnitude;
            }

            return 1000;
        }

        public void SetPosition(Vector3 position)
        {
            _playerTransform.position = position;
        }
    }
}

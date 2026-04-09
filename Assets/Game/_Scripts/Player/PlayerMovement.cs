using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] private InputManager _input;
    [SerializeField] private CameraManager _cameraManager;
    [SerializeField] private PlayerAudioManager _playerAudioManager;

    [Header("Move Configs")]
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _sprintSpeed;
    [SerializeField] private float _crouchSpeed;
    [SerializeField] private float _walkSprintTransition;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _climbSpeed;
    private float _speed;

    [Header("Rotate Configs")]
    [SerializeField] private float _rotationSmoothTime = 0.1f;
    private float _rotationSmoothVelocity;

    [Header("Step Configs")]
    [SerializeField] private Vector3 _upperStepOffset;
    [SerializeField] private float _stepCheckerDistance;
    [SerializeField] private float _stepForce;

    [Header("Climb Configs")]
    [SerializeField] private Transform _climbDetector;
    [SerializeField] private float _climbCheckDistance;
    [SerializeField] private Vector3 _climbOffset;
    [SerializeField] private LayerMask _climbableLayer;

    [Header("Glide Configs")]
    [SerializeField] private float _glideSpeed;
    [SerializeField] private float _airDrag;
    [SerializeField] private Vector3 _glideRotationSpeed;
    [SerializeField] private float _minGlideRotationX;
    [SerializeField] private float _maxGlideRotationX;
    private float _glideRotationX;

    [Header("Ground Check")]
    [SerializeField] private Transform _groundDetector;
    [SerializeField] private float _groundDetectorRadius;
    [SerializeField] private LayerMask _groundLayer;
    private bool _isGrounded;

    [Header("Punch Configs")]
    [SerializeField] private float _resetComboInterval;
    private Coroutine _resetComboRoutine;
    private bool _isPunching;
    private int _combo = 0;

    [Header("Hit Check")]
    [SerializeField] private Transform _hitDetector;
    [SerializeField] private float _hitDetectorRadius;
    [SerializeField] private LayerMask _hitLayer;

    [Header("Camera Configs")]
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float defaultFOV = 40f;
    [SerializeField] private float climbFOV = 70f;

    [Header("Collider Configs")]
    [SerializeField] private float _defaultColliderHeight = 1.8f;
    [SerializeField] private float _defaultColliderCenter = .9f;
    [SerializeField] private float _crouchColliderHeight = 1.3f;
    [SerializeField] private float _crouchColliderCenter = .66f;
    [SerializeField] private float _climbColliderCenter = 1.3f;

    private Rigidbody _rigidbody;
    private CapsuleCollider _collider;
    private Animator _animator;
    private PlayerStance _playerStance;

    private void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
        _animator = GetComponent<Animator>();

        _speed = _walkSpeed;
        _playerStance = PlayerStance.Stand;
    }

    private void Start() {
        _input.OnMoveInput += Move;
        _input.OnSprintInput += Sprint;
        _input.OnJumpInput += Jump;
        _input.OnClimbInput += StartClimb;
        _input.OnCancelClimbInput += CancelClimb;
        _input.OnCrouchInput += Crouch;
        _input.OnGlideInput += StartGlide;
        _input.OnCancelGlide += CancelGlide;
        _input.OnPunchInput += Punch;

        _cameraManager.OnChangePerspective += ChangePerspective;
    }

    private void OnDestroy() {
        _input.OnMoveInput -= Move;
        _input.OnSprintInput -= Sprint;
        _input.OnJumpInput -= Jump;
        _input.OnClimbInput -= StartClimb;
        _input.OnCancelClimbInput -= CancelClimb;
        _input.OnCrouchInput -= Crouch;
        _input.OnGlideInput -= StartGlide;
        _input.OnCancelGlide -= CancelGlide;
        _input.OnPunchInput -= Punch;

        _cameraManager.OnChangePerspective -= ChangePerspective;
    }

    private void Update() {
        Glide();
        CheckIsGrounded();
        CheckStep();
    }

    private void ChangePerspective() {
        _animator.SetTrigger("ChangePerspective");
    }

    #region Attack
    private void Punch() {
        if (!_isPunching && _playerStance == PlayerStance.Stand) {
            _isPunching = true;
            if (_combo < 3) {
                _combo++;
            }
            else {
                _combo = 1;
            }

            _animator.SetInteger("Combo", _combo);
            _animator.SetTrigger("Punch");
        }
    }

    private void EndPunch() {
        _isPunching = false;
    }

    private IEnumerator ResetCombo() {
        yield return new WaitForSeconds(_resetComboInterval);
        _combo = 0;
    }

    private void Hit() {
        Collider[] hitObjects = Physics.OverlapSphere(
            _hitDetector.position,
            _hitDetectorRadius,
            _hitLayer);

        for (int i = 0; i < hitObjects.Length; i++) {
            if (hitObjects[i].gameObject != null) {
                Destroy(hitObjects[i].gameObject);
            }
        }
    }
    #endregion

    #region Movement
    private void Move(Vector2 moveInput) {
        moveInput = moveInput.normalized;

        Vector3 movementDir = Vector3.zero;
        float movementAmount = 0f;

        bool isPlayerStanding = _playerStance == PlayerStance.Stand;
        bool isPlayerClimbing = _playerStance == PlayerStance.Climb;
        bool isPlayerCrouching = _playerStance == PlayerStance.Crouch;
        bool isPlayerGliding = _playerStance == PlayerStance.Glide;

        Vector3 velocity = Vector3.zero;

        if ((isPlayerStanding || isPlayerCrouching) && !_isPunching) {
            switch (_cameraManager.CameraState) {
                case CameraState.ThirdPerson:
                    // only move if input vector is not zero
                    if (moveInput.magnitude > 0f) {
                        // calculate rotation angle
                        float rotationAngle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg + _cameraTransform.eulerAngles.y;
                        float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, rotationAngle, ref _rotationSmoothVelocity, _rotationSmoothTime);

                        // apply rotation
                        transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

                        // move the character
                        movementDir = Quaternion.Euler(0f, rotationAngle, 0f) * Vector3.forward;
                        movementAmount = _speed * Time.deltaTime;
                        _rigidbody.AddForce(movementDir * movementAmount);
                    }
                    break;
                case CameraState.FirstPerson:
                    Vector3 rotationAmount = new Vector3(0f, _cameraTransform.eulerAngles.y, 0f);
                    transform.rotation = Quaternion.Euler(rotationAmount);

                    Vector3 verticalDir = moveInput.y * transform.forward;
                    Vector3 horizontalDir = moveInput.x * transform.right;
                    movementDir = verticalDir + horizontalDir;
                    movementAmount = _speed * Time.deltaTime;
                    _rigidbody.AddForce(movementDir * movementAmount);
                    break;
                default:
                    break;
            }

            // set animation parameter
            velocity = new Vector3(_rigidbody.linearVelocity.x, 0, _rigidbody.linearVelocity.z);
            _animator.SetFloat("Velocity", moveInput.magnitude * velocity.magnitude);
            _animator.SetFloat("VelocityX", moveInput.x * velocity.magnitude);
            _animator.SetFloat("VelocityZ", moveInput.y * velocity.magnitude);
        }

        else if (isPlayerClimbing) {
            Vector3 horizontalDir = moveInput.x * transform.right;
            Vector3 verticalDir = moveInput.y * transform.up;

            movementDir = horizontalDir + verticalDir;
            movementAmount = _climbSpeed * Time.deltaTime;
            _rigidbody.AddForce(movementDir * movementAmount);

            velocity = new Vector3(_rigidbody.linearVelocity.x, _rigidbody.linearVelocity.y, 0);
            _animator.SetFloat("ClimbVelocityX", moveInput.x * velocity.magnitude);
            _animator.SetFloat("ClimbVelocityY", moveInput.y * velocity.magnitude);
        }


        else if (isPlayerGliding) {
            Vector3 rotationDegree = transform.rotation.eulerAngles;

            // calculate and clamp player rotation on x axis while gliding
            _glideRotationX += _glideRotationSpeed.x * moveInput.y * Time.deltaTime;
            _glideRotationX = Mathf.Clamp(_glideRotationX, _minGlideRotationX, _maxGlideRotationX);
            rotationDegree.x = _glideRotationX;

            // calculate player rotation on other axis while gliding
            rotationDegree.z += _glideRotationSpeed.z * moveInput.x * Time.deltaTime;
            rotationDegree.y += _glideRotationSpeed.y * moveInput.x * Time.deltaTime;

            transform.rotation = Quaternion.Euler(rotationDegree);
        }
    }

    private void Sprint(bool isSprinting) {
        if (isSprinting) {
            if (_speed < _sprintSpeed) {
                _speed += _walkSprintTransition * Time.deltaTime;
            }
        }
        else {
            if (_speed > _walkSpeed) {
                _speed -= _walkSprintTransition * Time.deltaTime;
            }
        }
    }

    private void Crouch() {
        if (_playerStance == PlayerStance.Stand) {
            _playerStance = PlayerStance.Crouch;
            _animator.SetBool("IsCrouch", true);

            _speed = _crouchSpeed;

            _collider.height = _crouchColliderHeight;
            _collider.center = Vector3.up * _crouchColliderCenter;
        }
        else if (_playerStance == PlayerStance.Crouch) {
            _playerStance = PlayerStance.Stand;
            _animator.SetBool("IsCrouch", false);

            _speed = _walkSpeed;


            _collider.height = _defaultColliderHeight;
            _collider.center = Vector3.up * _defaultColliderCenter;
        }
    }

    private void Jump() {
        if (_isGrounded) {
            Vector3 jumpDir = Vector3.up;
            _rigidbody.AddForce(jumpDir * _jumpForce, ForceMode.Impulse);

            // trigger jump animation
            _animator.SetTrigger("Jump");
        }
    }

    private void StartClimb() {
        bool isInFrontOfClimbableWall = Physics.Raycast(
            _climbDetector.position,
            transform.forward,
            out RaycastHit hit,
            _climbCheckDistance,
            _climbableLayer
            );

        bool isNotClimbing = _playerStance != PlayerStance.Climb;

        if (isInFrontOfClimbableWall && _isGrounded && isNotClimbing) {
            Vector3 offset = (transform.forward * _climbOffset.z) + (Vector3.up * _climbOffset.y);
            transform.position = hit.point - offset;

            _playerStance = PlayerStance.Climb;
            _rigidbody.useGravity = false;

            // set fps camera to clamped
            _cameraManager.SetFPSClampedCamera(true, transform.rotation.eulerAngles);
            _cameraManager.SetTPSFieldOfView(climbFOV);

            // set collider center
            _collider.center = Vector3.up * _climbColliderCenter;

            // set animation
            _animator.SetBool("IsClimbing", true);
        }
    }

    private void CancelClimb() {
        if (_playerStance == PlayerStance.Climb) {
            _playerStance = PlayerStance.Stand;
            _rigidbody.useGravity = true;
            transform.position -= transform.forward * _climbOffset.z;

            // set fps camera to unclamped
            _cameraManager.SetFPSClampedCamera(false, transform.rotation.eulerAngles);
            _cameraManager.SetTPSFieldOfView(defaultFOV);

            // set collider center
            _collider.center = Vector3.up * _defaultColliderCenter;

            // set animation
            _animator.SetBool("IsClimbing", false);
        }
    }

    public void Glide() {
        if (_playerStance == PlayerStance.Glide) {
            Vector3 playerRotation = transform.rotation.eulerAngles;
            float lift = playerRotation.x;

            Vector3 upForce = transform.up * (lift + _airDrag);
            Vector3 forwardForce = transform.forward * _glideSpeed;
            Vector3 totalForce = upForce + forwardForce;

            _rigidbody.AddForce(totalForce * Time.deltaTime);
        }
    }

    private void StartGlide() {
        if (_playerStance != PlayerStance.Glide && !_isGrounded) {
            _playerStance = PlayerStance.Glide;

            _cameraManager.SetFPSClampedCamera(true, transform.rotation.eulerAngles);

            _animator.SetBool("IsGliding", true);

            _playerAudioManager.PlayGlideSFX();
        }
    }

    private void CancelGlide() {
        if (_playerStance == PlayerStance.Glide) {
            _playerStance = PlayerStance.Stand;

            _cameraManager.SetFPSClampedCamera(false, transform.rotation.eulerAngles);

            _animator.SetBool("IsGliding", false);

            _playerAudioManager.StopGlideSFX();
        }
    }
    #endregion

    #region State Check
    private void CheckIsGrounded() {
        _isGrounded = Physics.CheckSphere(_groundDetector.position, _groundDetectorRadius, _groundLayer);

        if (_isGrounded) {
            CancelGlide();
        } 

        // flag animation
        _animator.SetBool("IsGrounded", _isGrounded);
    }

    private void CheckStep() {
        bool isHitLowerStep = Physics.Raycast(
            _groundDetector.position,
            transform.forward,
            _stepCheckerDistance);

        bool isHitUpperStep = Physics.Raycast(
            _groundDetector.position + _upperStepOffset,
            transform.forward,
            _stepCheckerDistance);

        if (isHitLowerStep && !isHitUpperStep) {
            Vector3 stepForce = Vector3.up * _stepForce * Time.deltaTime;
            _rigidbody.AddForce(stepForce);
        }
    }
    #endregion
}

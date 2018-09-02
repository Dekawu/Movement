using UnityEngine;

public class Player : MonoBehaviour {
    public float MovementSpeed;
    public float JumpHeight;
    public float DashDistance;
    public float DashCooldown;
    public float DashLength;
    public float GravityStrength;
    public Transform Camera;

    private Vector3 _movement;
    private bool _dashing;
    private float _dashTimeRemaining;
    private float _dashCooldownRemaining;
    private Vector3 _dashDirection;
    private Vector3 _gravityVelocity;
    private CharacterController _characterController;
    private PlayerCameraController _cameraController;

    void Start() {
        _dashCooldownRemaining = 0;
        _gravityVelocity = Vector3.zero;
        _characterController = GetComponent<CharacterController>();
        _cameraController = Camera.GetComponent<PlayerCameraController>();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            if (_cameraController.TargetLocked) {
                _cameraController.TargetUnlock();
            } else {
                Ray ray = new Ray(transform.position, Camera.forward);
                RaycastHit[] hits = Physics.RaycastAll(ray);
                foreach (var hit in hits) {
                    if (hit.transform.GetComponent<Targetable>()) {
                        _cameraController.TargetLockOn(hit.transform);
                    }
                }
            }
        }
    }

    void FixedUpdate() {
        // Input movement
        _movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        // Move toward camera's facing direction
        _movement = Quaternion.Euler(0, Camera.eulerAngles.y, 0) * _movement;

        // Character facing direction
        if (_movement != Vector3.zero)
            transform.forward = _movement;

        // Character gravity
        if (!_dashing) {
            _gravityVelocity.y += GravityStrength * Time.fixedDeltaTime;
            _characterController.Move(_gravityVelocity * Time.fixedDeltaTime);
        }
        if ((_characterController.isGrounded && _gravityVelocity.y < 0) || _dashing) {
            _gravityVelocity = Vector3.zero;
        }

        // Character Jump
        if (Input.GetButtonDown("Jump") && _characterController.isGrounded) {
            _gravityVelocity.y += Mathf.Sqrt(JumpHeight * -2f * GravityStrength);
        }

        // Character Dash
        if (Input.GetKey(KeyCode.LeftShift) && _dashCooldownRemaining <= 0) {
            _dashing = true;
            _dashCooldownRemaining = DashCooldown;
            _dashTimeRemaining = DashLength;
            _dashDirection = transform.forward;
        } else if (_dashTimeRemaining <= 0 && _dashing) {
            _dashing = false;
        } else if (_dashing) {
            _movement = DashDistance * _dashDirection;
        }

        _dashCooldownRemaining = Mathf.Clamp(_dashCooldownRemaining - Time.fixedDeltaTime, 0, DashCooldown);
        _dashTimeRemaining = Mathf.Clamp(_dashTimeRemaining - Time.fixedDeltaTime, 0, DashLength);

        _characterController.Move(_movement * Time.fixedDeltaTime * MovementSpeed);
    }
}

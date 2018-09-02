using UnityEngine;

public class Player : MonoBehaviour {
    public float _speed;
    public float _gravityStrength;
    public float _jumpHeight;
    public float _dashDistance;
    public float _dashCooldown;
    public float _dashTimeLength;
    public GameObject _camera;

    private Vector3 _dashDirection;
    private bool _dashing;
    private float _dashTimeRemaining;
    private float _dashCooldownRemaining = 0;
    private Vector3 _gravityVelocity = Vector3.zero;
    private CharacterController _characterController;

    private Vector3 _movement;

    void Start() {
        _characterController = GetComponent<CharacterController>();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            if (_camera.GetComponent<CameraControl>().TargetLocked) {
                _camera.GetComponent<CameraControl>().TargetUnlock();
            } else {
                Ray ray = new Ray(transform.position, _camera.transform.forward);
                RaycastHit[] hits = Physics.RaycastAll(ray);
                foreach (var hit in hits) {
                    if (hit.transform.parent != null) {
                        if (hit.transform.parent.transform.gameObject.tag == "Enemy") {
                            _camera.GetComponent<CameraControl>().TargetLockOn(hit.transform);
                        }
                    }
                }
            }
        }
    }

    void FixedUpdate() {
        // Input movement
        _movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        // Move toward camera's facing direction
        _movement = Quaternion.Euler(0, _camera.transform.eulerAngles.y, 0) * _movement;

        // Character facing direction
        if (_movement != Vector3.zero)
            transform.forward = _movement;

        // Character gravity
        if (!_dashing) {
            _gravityVelocity.y += _gravityStrength * Time.fixedDeltaTime;
            _characterController.Move(_gravityVelocity * Time.fixedDeltaTime);
        }
        if ((_characterController.isGrounded && _gravityVelocity.y < 0) || _dashing) {
            _gravityVelocity = Vector3.zero;
        }

        // Character Jump
        if (Input.GetButtonDown("Jump") && _characterController.isGrounded) {
            _gravityVelocity.y += Mathf.Sqrt(_jumpHeight * -2f * _gravityStrength);
        }

        // Character Dash
        if (Input.GetKey(KeyCode.LeftShift) && _dashCooldownRemaining <= 0) {
            _dashing = true;
            _dashCooldownRemaining = _dashCooldown;
            _dashTimeRemaining = _dashTimeLength;
            _dashDirection = transform.forward;
        } else if (_dashTimeRemaining <= 0 && _dashing) {
            _dashing = false;
        } else if (_dashing) {
            _movement = _dashDistance * _dashDirection;
        }

        _dashCooldownRemaining = Mathf.Clamp(_dashCooldownRemaining - Time.fixedDeltaTime, 0, _dashCooldown);
        _dashTimeRemaining = Mathf.Clamp(_dashTimeRemaining - Time.fixedDeltaTime, 0, _dashTimeLength);

        _characterController.Move(_movement * Time.fixedDeltaTime * _speed);
    }


    void LateUpdate() {

    }


}

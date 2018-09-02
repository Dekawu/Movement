using UnityEngine;

public class PlayerCameraController : MonoBehaviour {
    public float RotationSpeed;
    public float CameraTargetHeight;
    public float CameraTargetDistance;

    private float _horizontal;
    private float _vertical;
    private float _cameraHeight;
    private float _cameraDistance;
    private Transform _player;
    private MeshRenderer _playerMeshRenderer;
    private Transform _target;

    public bool TargetLocked { get; private set; }

    void Start() {
        _horizontal = 0f;
        _vertical = 0f;
        _cameraHeight = CameraTargetHeight; 
        _cameraDistance = CameraTargetDistance;
        _player = transform.parent;
        _playerMeshRenderer = _player.GetComponent<MeshRenderer>();

        TargetLocked = false;
    }

    void Update() {
        if (Input.GetKey(KeyCode.LeftAlt)) {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        } else {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (TargetLocked) {
            Vector3 dir = _target.position - transform.position;
            CameraTargetHeight = _player.position.y;

            Quaternion rotation = Quaternion.LookRotation(dir);
            transform.position = _player.position - (rotation * new Vector3(0, 0, _cameraDistance));
            transform.position = new Vector3(transform.position.x, CameraTargetHeight, transform.position.z);

            transform.rotation = rotation;
        } else {
            _cameraHeight = CameraTargetHeight;

            _horizontal += Input.GetAxis("Mouse X") * RotationSpeed;
            _vertical += -Input.GetAxis("Mouse Y") * RotationSpeed;
            _vertical = Mathf.Clamp(_vertical, -10, 65);

            Quaternion rotation = Quaternion.Euler(_vertical, _horizontal, 0);
            transform.position = _player.position - (rotation * new Vector3(0, 0, _cameraDistance));

            transform.LookAt(_player);
            transform.Translate(new Vector3(0, _cameraHeight, 0));
        }

        Debug.DrawRay(transform.position, transform.forward * 500, Color.red);
    }

    // Should these somehow be a part of the targetable component?
    public void TargetLockOn(Transform target) {
        _target = target;
        TargetLocked = true;
    }

    public void TargetUnlock() {
        _target = null;
        TargetLocked = false;
    }
}


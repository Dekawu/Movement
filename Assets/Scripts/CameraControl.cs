using UnityEngine;

public class CameraControl : MonoBehaviour {
    public float _damping = 5f;
    public float _rotationSpeed = 3f;
    public float _cameraDistance = 10f;
    public float _cameraHeight = 1.75f;
    public float _cameraTargetHeight = 1.75f;
    public float _cameraDistanceTime = 1f;
    public float _cameraTargetDistance = 10f;

    private Transform _subject;
    private float horizontal = 0f;
    private float vertical = 0f;
    private float _cameraTransitionSpeed;
    private MeshRenderer _subjectMeshRenderer;
    private Transform _target;

    private bool _targetLocked = false;
    public bool TargetLocked {
        get {
            return _targetLocked;
        }
    }

    void Start() {
        _subject = transform.parent;
        _subjectMeshRenderer = _subject.GetComponent<MeshRenderer>();
    }

    void Update() {
        if (Input.GetKey(KeyCode.LeftAlt)) {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        } else {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        //var bounds = _subjectMeshRenderer.bounds.center;
        //var screenSpace = Camera.main.WorldToScreenPoint(bounds);

        //if (screenSpace.y < 0) {
        //    Debug.Log("OOB Bottom");
        //} else {
        //_cameraDistance = Mathf.SmoothDamp(_cameraDistance, _cameraTargetDistance, ref _cameraTransitionSpeed, _cameraDistanceTime);
        //_cameraHeight = Mathf.SmoothDamp(_cameraHeight, _cameraTargetHeight, ref _cameraTransitionSpeed, _cameraDistanceTime);
        //}

        if (TargetLocked) {
            //_cameraDistance = 

            Vector3 dir = _target.position - transform.position;
            _cameraTargetHeight = _subject.position.y;

            Quaternion rotation = Quaternion.LookRotation(dir);
            transform.position = _subject.position - (rotation * new Vector3(0, 0, _cameraDistance));
            transform.position = new Vector3(transform.position.x, _cameraTargetHeight, transform.position.z);

            transform.rotation = rotation;
        } else {
            _cameraTargetHeight = 1.75f;

            horizontal += Input.GetAxis("Mouse X") * _rotationSpeed;
            vertical += -Input.GetAxis("Mouse Y") * _rotationSpeed;
            vertical = Mathf.Clamp(vertical, -10, 65);

            Quaternion rotation = Quaternion.Euler(vertical, horizontal, 0);
            transform.position = _subject.position - (rotation * new Vector3(0, 0, _cameraDistance));

            transform.LookAt(_subject);
            transform.Translate(new Vector3(0, _cameraHeight, 0));
        }

        //if (_cameraHeight < _subject.position.y - _subject.localScale.y / 2) {
        //    _cameraHeight = _subject.position.y - _subject.localScale.y / 2;
        //}
        //_cameraDistance = Mathf.Clamp(_cameraDistance, 5, 30);

        Debug.DrawRay(transform.position, transform.forward * 500, Color.red);
    }

    public void TargetLockOn(Transform target) {
        _target = target;
        _targetLocked = true;
    }

    public void TargetUnlock() {
        _target = null;
        _targetLocked = false;
    }
}


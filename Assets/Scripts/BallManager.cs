using System;
using UnityEngine;

//This should be attached to the ball
[RequireComponent(typeof(Rigidbody))]
public class BallManager : Singleton<BallManager>
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private float _hitForceMultiplier = 1;
    
    // indicators should NOT be children of the ball
    [SerializeField] private RectTransform _hitIndicator;
    [SerializeField] private RectTransform _hitIndicatorPivot;
    [SerializeField] private Transform _hitEndedIndicator;

    [SerializeField] private float _velocityThreshold = 0.01f;

    private bool _updateSwing = false;
    private Rigidbody _rb;

    private Vector3 _hitVector;
    
    private bool _isMoving;
    private float _hitEndedTimer;

    private Vector3 _lastSafeLocation;
    
    void Start()
    {
        Systems.InputSystem.ActionDefinitions.SwingStart.Subscribe(OnSwingStart);
        Systems.InputSystem.ActionDefinitions.SwingEnd.Subscribe(OnSwingEnd);

        _rb = GetComponent<Rigidbody>();
        _lastSafeLocation = transform.position;
    }

    private void Update()
    {
        if (_updateSwing)
        {
            _hitIndicatorPivot.position = this.transform.position;
            Plane plane = new Plane(Vector3.down, this.transform.position);
            Ray ray = Camera.main.ScreenPointToRay(Systems.InputSystem.PointerPosition);
            plane.Raycast(ray, out var enter);
            var pointerPositionOnPlane = ray.GetPoint(enter);

            _hitVector = pointerPositionOnPlane - transform.position;
            float angle = Vector3.SignedAngle(Vector3.forward, _hitVector, Vector3.up);

            _hitIndicatorPivot.localRotation = Quaternion.Euler(0, 0, angle);

            float distToBall = _hitVector.magnitude;

            _hitIndicator.sizeDelta = new Vector2(_hitIndicator.sizeDelta.x, distToBall);
        }

        if (_isMoving && _rb.linearVelocity.sqrMagnitude < _velocityThreshold * _velocityThreshold)
        {
            _hitEndedTimer += Time.deltaTime;
            if (_hitEndedTimer > 0.5f)
            {
                _rb.linearVelocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
                _isMoving = false;
                _hitEndedIndicator.transform.position = this.transform.position;
                _hitEndedIndicator.gameObject.SetActive(false);
                _hitEndedIndicator.gameObject.SetActive(true);
                _lastSafeLocation = transform.position;
            }
        }
    }

    private void OnSwingStart()
    {
        if (!_isMoving)
        {
            _updateSwing = true;
            _hitIndicatorPivot.gameObject.SetActive(true);
            Debug.Log("Swing Started");
        }
    }
    
    private void OnSwingEnd()
    {
        if (_updateSwing)
        {
            _updateSwing = false;
            _hitIndicatorPivot.gameObject.SetActive(false);
            
            _rb.AddForce(_hitVector * -_hitForceMultiplier, ForceMode.Impulse);
            Debug.Log("Swing Ended");
            _isMoving = true;
            _hitEndedTimer = 0;
        }
    }

    public void ReturnToLastSafePosition()
    {
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        transform.position = _lastSafeLocation + Vector3.up;
    }

    public void SetPosition(Vector3 position)
    {
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        transform.position = position;
        _lastSafeLocation = position;
        _isMoving = false;
    }
}

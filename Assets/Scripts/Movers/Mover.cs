using System;
using UnityEditor;
using UnityEngine;
using MathUtils = Utils.MathUtils;

public class Mover : MonoBehaviour
{
    public enum Axis
    {
        X,
        Y,
        Z,
    }

    [SerializeField] private Axis _axis;
    [Tooltip("Min offset along axis from current position")]
    [SerializeField] private float _min; 
    [Tooltip("Max offset along axis from current position")]
    [SerializeField] private float _max;
    [SerializeField] private float _snappingSpeed = 10;
    
    [Tooltip("Enable if the ball could rest on this mover")]
    [SerializeField] private bool _useRigidBody;

    private Vector3 _initialPosition;
    private bool _isMoving;
    private Vector3 _pointerStartVector;
    private Vector3 _startPositionVector;
    private Plane _plane;
    private Camera _cam;
    private Rigidbody _rb;

    public float CurrentPositionFraction { get; private set; }
    
    void Start()
    {
        _initialPosition = transform.position;

        _plane = GetAxisPlane(_axis);
        _cam = Camera.main;
        if (_useRigidBody)
        {
            _rb = GetComponent<Rigidbody>();
        }
    }
    
    // Fixed update so that physics plays nice when the ball is atop this mover
    void FixedUpdate()
    {
        if (_useRigidBody)
        {
            Tick();
        }
    }

    private void Update()
    {
        if (!_useRigidBody)
        {
            Tick();
        }
    }

    private void Tick()
    {
        if (_isMoving)
        {
            Ray ray = _cam.ScreenPointToRay(Systems.InputSystem.PointerPosition);
            _plane.Raycast(ray, out var enter);
            Vector3 newPointerVector = ray.GetPoint(enter);
        
            Vector3 delta = Vector3.Scale(newPointerVector - _pointerStartVector, MoverUtils.GetAxisVector(_axis));
            Vector3 newPos = _startPositionVector;
            newPos += delta;
        
            SetPosition(GetClampedNewPos(newPos));
        }

        else
        {
            // Smooth motion to the nearest whole number
            // If i need other snapping sizes I'll deal with it later
            // Todo: Would prefer to accelerate towards nearest point then stop abruptly, 
            Vector3Int targetPosition = Vector3Int.RoundToInt(transform.position);
            SetPosition(MathUtils.ExpDecay(transform.position, targetPosition, _snappingSpeed, Time.deltaTime));
        }

        float positionDelta = (transform.position - _initialPosition + (MoverUtils.GetAxisVector(_axis) * _min)).magnitude;
        CurrentPositionFraction = positionDelta / (_max - _min);
    }

    private void SetPosition(Vector3 position)
    {
        if (_useRigidBody)
        {
            _rb.MovePosition(position);
        }
        else
        {
            transform.position = position;
        }
    }

    public void OnDragStart()
    {
        _isMoving = true;
        Ray ray = _cam.ScreenPointToRay(Systems.InputSystem.PointerPosition);
        _plane.Raycast(ray, out var enter);
        _pointerStartVector = ray.GetPoint(enter);
        _startPositionVector = transform.position;
    }

    public void OnDragEnd()
    {
        if (_isMoving)
        {
            _isMoving = false;
        }
    }
    
    // private Vector3 GetAxisVector(Axis axis)
    // {
    //     switch (axis)
    //     {
    //         case Axis.X:
    //             return Vector3.right;
    //         case Axis.Y: 
    //             return Vector3.up;
    //         case Axis.Z:
    //             return Vector3.forward;
    //         default: 
    //             return Vector3.zero;
    //     }
    // }

    private Plane GetAxisPlane(Axis axis)
    {
        switch (axis)
        {
            case Axis.X:
                return new Plane(Vector3.forward, transform.position);
            case Axis.Y: 
                return new Plane(new Vector3(0.5f, 0, 0.5f), transform.position);
            case Axis.Z:
                return new Plane(Vector3.up, transform.position);
            default: 
                return new Plane();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="newPosition"></param>
    /// <returns>offset from initial position clamped between min/max along the axis</returns>
    private Vector3 GetClampedNewPos(Vector3 newPosition)
    {
        Vector3 clampedDelta = newPosition - _initialPosition;

        switch (_axis)
        {
            case Axis.X:
                clampedDelta.x = Mathf.Clamp(clampedDelta.x, _min, _max);
                break;
            case Axis.Y:
                clampedDelta.y = Mathf.Clamp(clampedDelta.y, _min, _max);
                break;
            case Axis.Z:
                clampedDelta.z = Mathf.Clamp(clampedDelta.z, _min, _max);
                break;
        }
        return _initialPosition + clampedDelta;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position + (MoverUtils.GetAxisVector(_axis) * _min), transform.position + MoverUtils.GetAxisVector(_axis) * _max);
    }
}

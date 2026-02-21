using System;
using UnityEngine;

public class GearedMover : MonoBehaviour
{
    [SerializeField] private Mover _parentMover;
    [SerializeField] private Mover.Axis _axis;

    [Tooltip("Min offset along axis from current position")]
    [SerializeField] private float _min;
    [Tooltip("Max offset along axis from current position")]
    [SerializeField] private float _max;

    private Vector3 _initialPosition;
    private Rigidbody _rb;

    private void Start()
    {
        _initialPosition = transform.position;
        _rb = GetComponent<Rigidbody>();
    }

    //Fixed update so that physics works nice if the ball is on the object when its moving
    private void FixedUpdate()
    {
        float delta = ((_max - _min) * _parentMover.CurrentPositionFraction) + _min;

        var newPosition = _initialPosition + (MoverUtils.GetAxisVector(_axis) * delta);
        
        _rb.MovePosition(newPosition);
    }
}

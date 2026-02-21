using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MoverHandle : MonoBehaviour
{
    [SerializeField] private Mover _mover;
    
    private void Start()
    {
        Systems.InputSystem.ActionDefinitions.MoverDragStart.Subscribe(OnMoverDragStart);
        Systems.InputSystem.ActionDefinitions.MoverDragEnd.Subscribe(OnMoverDragEnd);
    }

    private void OnMoverDragStart(MoverHandle handle)
    {
        if (handle == this)
        {
            _mover.OnDragStart();
        }
    }

    private void OnMoverDragEnd()
    {
        _mover.OnDragEnd();
    }
}

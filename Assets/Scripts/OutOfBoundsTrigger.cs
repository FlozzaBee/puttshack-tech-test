using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class OutOfBoundsTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out BallManager _))
        {
            Debug.Log("Out of bounds");
            BallManager.Instance.ReturnToLastSafePosition();
        }
    }
}

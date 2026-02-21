using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HoleTrigger : MonoBehaviour
{
    [SerializeField] private ParticleSystem _confettiParticles;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out BallManager _))
        {
            _confettiParticles.Play();
            GameplayManager.Instance.MoveToNextLevel();
        }
    }
}

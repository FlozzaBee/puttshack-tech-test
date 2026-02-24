using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HoleTrigger : MonoBehaviour
{
    [SerializeField] private ParticleSystem _confettiParticles;
    [SerializeField] private Flag _flag;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out BallManager _))
        {
            StartCoroutine(OnEnterHoleRoutine());
        }
    }

    private IEnumerator OnEnterHoleRoutine()
    {
        _confettiParticles.Play();
        _flag.OnBallPotted();
        // Wait a short time before moving on.
        yield return new WaitForSeconds(0.33f);
        GameplayManager.Instance.MoveToNextLevel();
    }
}

using System;
using UnityEngine;

public class Flag : MonoBehaviour
{
    private static readonly int AlphaID = Shader.PropertyToID("_Alpha");
    [SerializeField] private float _ballDistToRaise = 4;
    [SerializeField] private float _maxHeight;

    private Vector3 BallPosition => BallManager.Instance.transform.position;

    private Vector3 _initialPosition;
    private bool _doUpdatePosition = true;
    private Renderer _renderer;
    private MaterialPropertyBlock _propertyBlock;
    
    private void Start()
    {
        _initialPosition = transform.position;
        _renderer = GetComponent<Renderer>();
        _propertyBlock = new MaterialPropertyBlock();
    }

    void Update()
    {
        if (_doUpdatePosition && (BallPosition - _initialPosition).sqrMagnitude > _maxHeight * _maxHeight)
        {
            float distToBall = (BallPosition - _initialPosition).magnitude;
            float distToBallFraction = Mathf.Clamp01(distToBall / _ballDistToRaise);
            float height = (1 - distToBallFraction) * _maxHeight;

            transform.position = _initialPosition + (Vector3.up * height);
            
            _propertyBlock.SetFloat(AlphaID, distToBallFraction);
            _renderer.SetPropertyBlock(_propertyBlock);
            
            // _renderer.material.SetFloat(AlphaID, 0.5f);
        }
    }

    public void OnBallPotted()
    {
        _doUpdatePosition = false;
    }
}

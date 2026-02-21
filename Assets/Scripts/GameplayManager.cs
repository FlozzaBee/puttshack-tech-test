using System;
using System.Collections;
using UnityEngine;

public class GameplayManager : Singleton<GameplayManager>
{
    [Serializable]
    public struct LevelData
    {
        public Transform CameraTarget;
        public Transform BallSpawnPosition;
    }

    [Serializable]
    public struct CameraTransitionSettings
    {
        public AnimationCurve Curve;
        public float Duration;
    }
    
    [SerializeField] private LevelData[] Levels;
    [SerializeField] private CameraTransitionSettings cameraTransitionSettings;

    private Camera _cam;
    private int _currentLevel = 0;

#if UNITY_EDITOR
    public EditorButton moveToNextLevel = new EditorButton(nameof(MoveToNextLevel), true);
#endif
    
    private void Start()
    {
        _cam = Camera.main;
        _cam.transform.position = Levels[0].CameraTarget.position;
        
        // BallManager.Instance.SetPosition(Levels[0].BallSpawnPosition.position);
    }

    public void MoveToNextLevel()
    {
        _currentLevel += 1;
        MoveToLevel(_currentLevel);
    }

    private void MoveToLevel(int index)
    {
        //todo: lerp camera
        if (cameraTransitionSettings.Duration == 0)
        {
            _cam.transform.position = Levels[index].CameraTarget.position;
        }
        else
        {
            StartCoroutine(MoveCameraRoutine(Levels[index].CameraTarget.position));
        }
        BallManager.Instance.SetPosition(Levels[index].BallSpawnPosition.position);
    }

    private IEnumerator MoveCameraRoutine(Vector3 newCameraPosition)
    {
        Vector3 cameraStartPosition = _cam.transform.position;
        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime / cameraTransitionSettings.Duration;
            float t = cameraTransitionSettings.Curve.Evaluate(time);
            _cam.transform.position = Vector3.Lerp(cameraStartPosition, newCameraPosition, t);
            yield return null;
        }
    }
}

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

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
    
    [FormerlySerializedAs("Levels")] [SerializeField] private LevelData[] _levels;
    [FormerlySerializedAs("cameraTransitionSettings")] [SerializeField] private CameraTransitionSettings _cameraTransitionSettings;

    private Camera _cam;
    private int _currentLevel = 0;

#if UNITY_EDITOR
    public EditorButton moveToNextLevel = new EditorButton(nameof(MoveToNextLevel), true);
#endif
    
    private void Start()
    {
        _cam = Camera.main;
        _cam.transform.position = _levels[0].CameraTarget.position;
    }

    public void MoveToNextLevel()
    {
        _currentLevel += 1;
        if (_currentLevel > _levels.Length - 1)
        {
            OnGameEnd();
        }
        else
        {
            MoveToLevel(_currentLevel);
        }
    }

    public void MoveToLevel(int index)
    {
        // So that the current level stays consistent if the level is set out of sequence (i.e. by debug tools)
        _currentLevel = index;
        if (index >= _levels.Length)
        {
            return;
        }
        if (_cameraTransitionSettings.Duration == 0)
        {
            _cam.transform.position = _levels[index].CameraTarget.position;
        }
        else
        {
            StartCoroutine(MoveCameraRoutine(_levels[index].CameraTarget.position));
        }
        BallManager.Instance.SetPosition(_levels[index].BallSpawnPosition.position);
    }

    private IEnumerator MoveCameraRoutine(Vector3 newCameraPosition)
    {
        Vector3 cameraStartPosition = _cam.transform.position;
        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime / _cameraTransitionSettings.Duration;
            float t = _cameraTransitionSettings.Curve.Evaluate(time);
            _cam.transform.position = Vector3.Lerp(cameraStartPosition, newCameraPosition, t);
            yield return null;
        }
    }

    private void OnGameEnd()
    {
        Debug.Log($"Hits: {BallManager.Instance.Hits}");
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

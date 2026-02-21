using UnityEngine;

public class CameraTarget : MonoBehaviour
{
#if UNITY_EDITOR
    public EditorButton bringCameraHere = new EditorButton(nameof(BringCameraHere));

    public void BringCameraHere()
    {
        Camera.main.transform.position = this.transform.position;
    }
#endif
}

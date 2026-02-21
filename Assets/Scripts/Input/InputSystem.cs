using UnityEngine;

public class InputSystem
{
    public ActionDefinitions ActionDefinitions = new();

    public Vector2 PointerPosition { get; private set; }
    
    private InputHandler _inputHandler = new();
    
    public InputSystem()
    {
        _inputHandler.Init();
    }

    public void SetPointerPosition(Vector2 pointerPosition)
    {
        PointerPosition = pointerPosition;
    }
    
}

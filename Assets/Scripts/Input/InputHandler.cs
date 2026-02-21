using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : InputSystem_Actions.IPlayerActions
{
    private InputSystem_Actions _inputSystemActions;
    
    public void Init()
    {
        _inputSystemActions = new InputSystem_Actions();
        _inputSystemActions.Player.SetCallbacks(this);
        _inputSystemActions.Player.Enable();
    }
    
    public void OnPointerMoved(InputAction.CallbackContext context)
    {
        Systems.InputSystem.SetPointerPosition(context.ReadValue<Vector2>());
    }

    public void OnPrimaryClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // if clicked ball
            Ray ray = Camera.main.ScreenPointToRay(Systems.InputSystem.PointerPosition);

            Physics.Raycast(ray, out var hit);
            if (hit.collider.TryGetComponent(out BallClickCollider _))
            {
                Systems.InputSystem.ActionDefinitions.SwingStart.Invoke();
            }
            
            // if clicked a dragger
            if (hit.collider.TryGetComponent(out MoverHandle mover))
            {
                Systems.InputSystem.ActionDefinitions.MoverDragStart.Invoke(mover);
            }
        }

        if (context.canceled)
        {
            Systems.InputSystem.ActionDefinitions.SwingEnd.Invoke();
            Systems.InputSystem.ActionDefinitions.MoverDragEnd.Invoke();
        }
    }
}

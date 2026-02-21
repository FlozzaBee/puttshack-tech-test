using Input;
using UnityEngine;

public class ActionDefinitions
{
    public InputAction SwingStart = new();
    public InputAction SwingEnd = new();

    public InputAction<MoverHandle> MoverDragStart = new();
    public InputAction MoverDragEnd = new();
}

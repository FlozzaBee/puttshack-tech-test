using UnityEngine;

public static class MoverUtils
{
    public static Vector3 GetAxisVector(Mover.Axis axis)
    {
        switch (axis)
        {
            case Mover.Axis.X:
                return Vector3.right;
            case Mover.Axis.Y: 
                return Vector3.up;
            case Mover.Axis.Z:
                return Vector3.forward;
            default: 
                return Vector3.zero;
        }
    }
}

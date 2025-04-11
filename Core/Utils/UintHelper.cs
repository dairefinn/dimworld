namespace Dimworld.Utils;


public static class UIntHelper
{

    /// <summary>
    /// Converts the collision layers from an array of integers to a uint.
    /// </summary>
    /// <param name="layers"></param>
    /// <returns></returns>
    public static uint ParseCollisionLayers(params int[] layers)
    {
        uint collisionLayers = 0;
        foreach (int layer in layers)
        {
            collisionLayers |= (uint)(1 << (layer - 1));
        }
        return collisionLayers;
    }

}

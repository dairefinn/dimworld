namespace Dimworld.Core.Utils;


/// <summary>
/// Helper class for manipulating unsigned integers.
/// Godot properties that control layers are often unsigned integers so this is primarily used for that.
/// </summary>
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

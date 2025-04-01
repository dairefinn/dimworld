namespace Dimworld.MemoryEntries;

using Godot;


public partial class NodeLocation : MemoryEntry
{

    public Node2D Node { get; set; } = null;
    public Vector2 Position { get; set; } = Vector2.Zero;

    public override string ToString()
    {
        if (Node != null)
        {
            return $"{Node.Name} {Node.GlobalPosition}";
        }

        return base.ToString();
    }
}

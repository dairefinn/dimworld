namespace Dimworld;

using Godot;
using Godot.Collections;


[Tool]
public partial class InventorySlotDragArea : Area2D
{

    public InventorySlotUI ParentSlot { get; set; }
    public CollisionShape2D CollisionShape { get; set; }

    public Array<InventorySlotDragArea> Targets = [];


    public override void _Ready()
    {
        CollisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        AreaEntered += OnDragAreaEntered;
        AreaExited += OnDragAreaExited;
    }


    private void OnDragAreaEntered(Area2D area)
    {
        if (area is InventorySlotDragArea dragArea)
        {
            Targets.Add(dragArea);
        }
    }

    private void OnDragAreaExited(Area2D area)
    {
        if (area is InventorySlotDragArea dragArea)
        {
            Targets.Remove(dragArea);
        }
    }
}

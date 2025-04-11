namespace Dimworld.Items.UI;

using Godot;
using Godot.Collections;


public partial class InventorySlotDragArea : Area2D
{

    public InventorySlotUI ParentSlot { get; set; }
    public CollisionShape2D CollisionShape { get; set; }
    public InventorySlotDragArea Target = null;


    public override void _Ready()
    {
        CollisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        AreaEntered += OnDragAreaEntered;
        AreaExited += OnDragAreaExited;
    }


    private void OnDragAreaEntered(Area2D area)
    {
        if (area is not InventorySlotDragArea dragArea) return;
        Target = dragArea;
    }

    private void OnDragAreaExited(Area2D area)
    {
        if (area != Target) return;
        Target = null;
    }
}

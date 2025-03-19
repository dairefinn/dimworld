namespace Dimworld;

using Godot;


public partial class PlayerInteractionHandler : Node
{

    [Export] public Inventory PlayerInventory { get; set; }
    [Export] public AgentDetectionHandler AgentDetectionHandler { get; set; }
    [Export] public CursorFollower CursorFollower { get; set; }


    private void InteractWithItem(ICanBeInteractedWith interactableObject)
    {
        interactableObject.InteractWith();

        InventoryItem itemDuplicate = GD.Load("res://Items/TestItem.tres").Duplicate() as InventoryItem;
        bool success = PlayerInventory.AddItem(itemDuplicate);
        if (success)
        {
            GD.Print("Added item to inventory: " + itemDuplicate.ItemName);
        }
        else
        {
            GD.Print("Failed to add item to inventory: " + itemDuplicate.ItemName);
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (Input.IsActionJustPressed("test_input"))
        {
            ICanBeInteractedWith interactableObject = CursorFollower.InteractableObject;
            if (interactableObject != null)
            {
                InteractWithItem(interactableObject);
            }
        }
    }

}

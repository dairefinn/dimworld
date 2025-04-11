namespace Dimworld.Dialogue.Options;

using System;
using Dimworld.Developer;
using Dimworld.Items;
using Godot;


[GlobalClass]
public partial class DialogueOptionTradeToWorld : DialogueOptionTrade
{

    [Export] public Vector2 SpawnPosition { get; set; }
    [Export] public Vector2 SpawnRotation { get; set; }
    [Export] public Vector2 SpawnScale { get; set; }

    // TODO: Finish implementing this once Vehicles and Dropping items is added

    public override bool OnSelected()
    {
        if (Item == null) return false;
        var wasBought = Globals.Instance.TradeController.TryBuy(Item, Price, Quantity, false);
        if (!wasBought) return false;

        // if (Item is IVehicleItem)
        // {
        //     return SpawnVehicle( Item, SpawnPosition, SpawnRotation, SpawnScale );
        // }
        // else
        // {
        //     return SpawnItem( Item, SpawnPosition, SpawnRotation, SpawnScale );
        // }
        
        DeveloperConsole.PrintErr( $"Failed to spawn item: {Item}" );
        return false;
        
    }

    // private static bool SpawnItem(InventoryItem item, Vector2 position, Vector2 rotation, Vector2 scale )
    // {
    //     try {            
    //         InventoryItemController inventoryItem = new InventoryItemController();
    //         inventoryItem.InitializeFromExisting( item );

    //         inventoryItem.Transform.Rotation = rotation;
    //         inventoryItem.Transform.Position = position;
    //         inventoryItem.Transform.Position = inventoryItem.Transform.Position.WithZ( inventoryItem.Transform.Position.z + 100.0f );
    //         inventoryItem.Transform.Scale = scale;

    //         inventoryItem.Name = item.ItemName;
    //         inventoryItem.Tags.Add( "item" );

    //         return true;
    //     }
    //     catch (Exception ex)
    //     {
    //         DeveloperConsole.PrintErr( $"Failed to spawn item: {ex.Message}" );
    //         return false;
    //     }
    // }

    // private static bool SpawnVehicle(InventoryItem item, Vector2 position, Vector2 rotation, Vector2 scale)
    // {
    //     try {
    //         VehicleMetadata vehicleMetadata = VehicleMetadata.FindByAssociatedItem(item);
    //         Vehicle vehicle = new Vehicle(vehicleMetadata.ScenePath);

    //         newObject.Transform.Rotation = rotation;
    //         newObject.Transform.Position = position;
    //         newObject.Transform.Scale = scale;

    //         return true;
    //     }
    //     catch (Exception ex)
    //     {
    //         DeveloperConsole.PrintErr( $"Failed to spawn vehicle: {ex.Message}" );
    //         return false;
    //     }
    // }

}

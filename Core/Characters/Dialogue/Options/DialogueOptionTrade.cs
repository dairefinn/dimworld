namespace Dimworld.Core.Characters.Dialogue.Options;

using Dimworld.Core.Items;
using Godot;


[GlobalClass]
public abstract partial class DialogueOptionTrade : DialogueOption
{

    [Export] public int Price { get; set; } = 0;
    [Export] public int Quantity { get; set; } = 1;
    [Export] public InventoryItem Item { get; set; }
    [Export] public TradeOption TradeOption { get; set; }

}

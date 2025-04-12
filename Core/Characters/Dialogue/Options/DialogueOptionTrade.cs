namespace Dimworld.Core.Characters.Dialogue.Options;

using Dimworld.Core.Items;
using Godot;


/// <summary>
/// Abstract class representing a trade option in the dialogue.
/// Any trade-related dialogue options should inherit from this class.
/// </summary>
[GlobalClass]
public abstract partial class DialogueOptionTrade : DialogueOption
{

    [Export] public int Price { get; set; } = 0;
    [Export] public int Quantity { get; set; } = 1;
    [Export] public InventoryItem Item { get; set; }
    [Export] public TradeOption TradeOption { get; set; }

}

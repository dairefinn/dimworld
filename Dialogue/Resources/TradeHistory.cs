namespace Dimworld.Dialogue;

using System;
using Dimworld.Items;
using Godot;


public partial class TradeHistory : Resource {

    public InventoryItem Item { get; set; }
    public int Price { get; set; }
    public int Quantity { get; set; }
    public DateTime Date { get; set; }
    public TradeOption TradeOption { get; set; }
    public bool AddedToInventory { get; set; }


    public override string ToString() {
        return $"{Date} - {TradeOption} {Quantity} {Item.ItemName} for ${Price}";
    }

}

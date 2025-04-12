namespace Dimworld.Core.Characters.Dialogue;

using System;
using System.Linq;
using Dimworld.Core;
using Dimworld.Core.Developer;
using Dimworld.Core.Items;
using Godot;
using Godot.Collections;


/// <summary>
/// Controller for managing trade operations.
/// This includes buying and selling items, as well as tracking trade history.
/// </summary>
public partial class TradeController : Node
{

    [Export] public Array<TradeHistory> TradeHistory { get; private set; } = [];


    private void AddTradeHistory(InventoryItem item, int price, int quantity, TradeOption tradeOption, bool addToInventory = true) {
        if (!addToInventory) return;

        TradeHistory newHistoryItem = new()
        {
            Item = item,
            Price = price,
            Quantity = quantity,
            Date = DateTime.Now,
            TradeOption = tradeOption,
            AddedToInventory = addToInventory
        };

        TradeHistory.Add(newHistoryItem);
    }


    /// <summary>
    /// Attempts to buy an item.
    /// </summary>
    /// <param name="item">The item to buy.</param>
    /// <param name="price">The price of the item.</param>
    /// <param name="quantity">The quantity of the item to buy.</param>
    /// <param name="addToInventory">True if the item should be added to the inventory, false otherwise.</param>
    /// <returns>True if the item was successfully bought, false otherwise.</returns>
    public bool TryBuy(InventoryItem item, int price, int quantity, bool addToInventory) {
        bool canAfford = Globals.Instance.CurrencyController.HasMoney(price);
        if (!canAfford) { DeveloperConsole.PrintInfo("Not enough money"); return false; }

        if (addToInventory) {
            bool canFitInInventory = Globals.Instance.Player.Inventory.CanAddItem(item, quantity);
            if (!canFitInInventory) { DeveloperConsole.PrintInfo("Inventory is full"); return false; }
            Globals.Instance.Player.Inventory.AddItem(item, quantity);
        }

        Globals.Instance.CurrencyController.RemoveMoney(price);
        
        AddTradeHistory(item, price, quantity, TradeOption.BUY, addToInventory);

        return true;
    }

    /// <summary>
    /// Attempts to sell an item.
    /// </summary>
    /// <param name="item">The item to sell.</param>
    /// <param name="price">The price of the item.</param>
    /// <returns>True if the item was successfully sold, false otherwise.</returns>
    public bool TrySell(InventoryItem item, int price) {
        InventorySlot slotWithItem = Globals.Instance.Player.Inventory.GetFirstSlotWithItem(item);
        if (slotWithItem == null) return false;
        Globals.Instance.Player.Inventory.RemoveItem(item);
        Globals.Instance.CurrencyController.AddMoney(price);

        AddTradeHistory(item, price, 1, TradeOption.SELL);

        return true;
    }

    /// <summary>
    /// Ends the trade session.
    /// </summary>
    public void EndTrade() {
        TradeHistory = [];
    }

    /// <summary>
    /// Undoes the last trade operation from the trade history.
    /// </summary>
    public void UndoLastTrade() {
        if (TradeHistory.Count == 0) return;
        TradeHistory lastTrade = TradeHistory.Last();
        if (lastTrade.TradeOption == TradeOption.BUY) {
            Globals.Instance.CurrencyController.AddMoney(lastTrade.Price);
            if (lastTrade.AddedToInventory) {
                Globals.Instance.Player.Inventory.RemoveItem(lastTrade.Item, lastTrade.Quantity);
            }
        } else {
            Globals.Instance.CurrencyController.RemoveMoney(lastTrade.Price);
            if (lastTrade.AddedToInventory) {
                Globals.Instance.Player.Inventory.AddItem(lastTrade.Item, lastTrade.Quantity);
            }
        }
        TradeHistory.Remove(lastTrade);
    }

}

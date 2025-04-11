namespace Dimworld.Dialogue;

using System;
using System.Linq;
using Dimworld.Developer;
using Dimworld.Items;
using Godot;
using Godot.Collections;

public partial class TradeController : Node {

    public static TradeController Instance { get; private set; }

    [Export] public Array<TradeHistory> TradeHistory { get; private set; } = [];

    public override void _Ready() {
        Instance = this;
    }

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

    public bool TrySell(InventoryItem item, int price) {
        InventorySlot slotWithItem = Globals.Instance.Player.Inventory.GetFirstSlotWithItem(item);
        if (slotWithItem == null) return false;
        Globals.Instance.Player.Inventory.RemoveItem(item);
        Globals.Instance.CurrencyController.AddMoney(price);

        AddTradeHistory(item, price, 1, TradeOption.SELL);

        return true;
    }

    public void EndTrade() {
        TradeHistory = [];
    }

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

    private void AddTradeHistory(InventoryItem item, int price, int quantity, TradeOption tradeOption, bool addToInventory = true) {
        // TODO: We don't track one off purchases so they cannot be refunded. Eg - buying a car or house upgrade. Might just want to have another bool on the tradeHistory item to control if you can refund or no.
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

}

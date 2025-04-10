namespace Dimworld.Dialogue.Options;

using Godot;


[GlobalClass]
public partial class DialogueOptionTradeToInventory : DialogueOptionTrade
{

    public override bool OnSelected()
    {
        if (Item == null) return false;

		return TradeOption switch
		{
			TradeOption.BUY => TradeController.Instance.TryBuy(Item, Price, Quantity, true),
			TradeOption.SELL => TradeController.Instance.TrySell(Item, Price),
			_ => false,
		};
	}
}

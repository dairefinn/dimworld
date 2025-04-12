namespace Dimworld.Core.Characters.Dialogue.Options;

using Dimworld.Core;
using Godot;


[GlobalClass]
public partial class DialogueOptionTradeToInventory : DialogueOptionTrade
{

    public override bool OnSelected()
    {
        if (Item == null) return false;

		return TradeOption switch
		{
			TradeOption.BUY => Globals.Instance.TradeController.TryBuy(Item, Price, Quantity, true),
			TradeOption.SELL => Globals.Instance.TradeController.TrySell(Item, Price),
			_ => false,
		};
	}
}

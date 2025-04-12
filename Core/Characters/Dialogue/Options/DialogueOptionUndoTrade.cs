namespace Dimworld.Core.Characters.Dialogue.Options;

using Dimworld.Core;
using Godot;


/// <summary>
/// Class representing an option to undo the last trade.
/// This option is only shown if there is at least one trade in the history.
/// </summary>
[GlobalClass]
public partial class DialogueOptionUndoTrade : DialogueOption
{

    public override bool ShouldShow() {
        return Globals.Instance.TradeController.TradeHistory.Count > 0;
    }

    public override bool OnSelected()
    {   
        Globals.Instance.TradeController.UndoLastTrade();
        return true;
    }

}

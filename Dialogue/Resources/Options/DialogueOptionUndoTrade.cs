namespace Dimworld.Dialogue.Options;

using Godot;


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

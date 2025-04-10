namespace Dimworld.Dialogue.Options;

using Godot;


[GlobalClass]
public partial class DialogueOptionUndoTrade : DialogueOption
{

    public override bool ShouldShow() {
        return TradeController.Instance.TradeHistory.Count > 0;
    }

    public override bool OnSelected()
    {   
        TradeController.Instance.UndoLastTrade();
        return true;
    }

}

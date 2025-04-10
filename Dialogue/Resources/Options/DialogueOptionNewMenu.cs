namespace Dimworld.Dialogue.Options;

using Dimworld.Dialogue.Menus;
using Dimworld.Items.UI;
using Dimworld.UI;
using Godot;


[GlobalClass]
public partial class DialogueOptionNewMenu : DialogueOption
{

    [Export] public DialogueMenu NextDialogueRoot { get; set; }

    public override bool OnSelected()
    {
        if (NextDialogueRoot == null) return false;
        NextDialogueRoot.ShuffleMessage();
        Globals.Instance.DialogueHandler.StartDialogue(NextDialogueRoot);

        if (NextDialogueRoot is DialogueMenuTrade)
        {
            UIRootPanel.Instance?.EnablePanel<InventoryViewer>();
            Globals.Instance.InventoryViewer.CanOpen = false;
        }

        return true;
    }
}

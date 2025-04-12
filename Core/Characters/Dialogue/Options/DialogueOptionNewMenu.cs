namespace Dimworld.Core.Characters.Dialogue.Options;

using Dimworld.Core;
using Dimworld.Core.Characters.Dialogue.Menus;
using Dimworld.UI.Inventory;
using Godot;


/// <summary>
/// Class representing a new menu option in the dialogue.
/// </summary>
[GlobalClass]
public partial class DialogueOptionNewMenu : DialogueOption
{

    [Export] public DialogueMenu NextDialogueRoot { get; set; }


    public override bool OnSelected()
    {
        if (NextDialogueRoot == null) return false;
        NextDialogueRoot.ShuffleMessage();
        Globals.Instance.DialoguePanelUI.StartDialogue(NextDialogueRoot);

        if (NextDialogueRoot is DialogueMenuTrade)
        {
            Globals.Instance.UIRoot.EnablePanel<InventoryViewer>();
            Globals.Instance.InventoryViewer.CanOpen = false;
        }

        return true;
    }
}

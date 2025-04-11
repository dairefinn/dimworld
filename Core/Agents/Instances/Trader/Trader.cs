namespace Dimworld.Agents.Instances;

using Dimworld.Dialogue;
using Godot;

public partial class Trader : NpcController, IHasDialogueTree, ICanBeInteractedWith
{

    [Export] public DialogueMenu DialogueTree { get; set; }


    public void InteractWith()
    {
        if (DialogueTree == null)
        {
            GD.Print("No dialogue tree assigned to trader.");
            return;
        }

        Globals.Instance.DialoguePanelUI.StartDialogue(DialogueTree);
    }

}

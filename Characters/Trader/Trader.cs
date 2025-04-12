namespace Dimworld.Agents.Instances;

using Dimworld.Core;
using Dimworld.Core.Characters;
using Dimworld.Core.Characters.Dialogue;
using Dimworld.Core.Characters.Dialogue.Menus;
using Dimworld.Core.Interaction;
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

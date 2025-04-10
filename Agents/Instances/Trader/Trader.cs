namespace Dimworld.Agents.Instances;

using Dimworld.Dialogue;
using Godot;

public partial class Trader : NpcController, IHasDialogueTree, ICanBeInteractedWith
{

    [Export] public DialogueMenu DialogueTree { get; set; }


    public void InteractWith()
    {
        GD.Print("Interacted with trader");
        // TODO: Send the dialogue tree to the dialogue UI
    }

}

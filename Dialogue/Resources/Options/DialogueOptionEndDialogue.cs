namespace Dimworld.Dialogue.Options;

using Godot;


[GlobalClass]
public partial class DialogueOptionEndDialogue : DialogueOption
{

    public override string Name { get; set; } = "End dialogue";


    public override bool OnSelected() {
        Globals.Instance.DialogueHandler.EndDialogue();
        return true;
    }

}

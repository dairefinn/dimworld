namespace Dimworld.Core.Characters.Dialogue.Options;

using Dimworld.Core;
using Godot;


[GlobalClass]
public partial class DialogueOptionEndDialogue : DialogueOption
{

    public override bool OnSelected() {
        Globals.Instance.DialoguePanelUI.EndDialogue();
        return true;
    }

}

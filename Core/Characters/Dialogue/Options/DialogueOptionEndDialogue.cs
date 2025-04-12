namespace Dimworld.Core.Characters.Dialogue.Options;

using Dimworld.Core;
using Godot;


/// <summary>
/// Class representing an option to end the dialogue.
/// </summary>
[GlobalClass]
public partial class DialogueOptionEndDialogue : DialogueOption
{

    public override bool OnSelected() {
        Globals.Instance.DialoguePanelUI.EndDialogue();
        return true;
    }

}

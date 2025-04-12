namespace Dimworld.Core.Characters.Dialogue;

using Dimworld.Core.Characters.Dialogue.Menus;


/// <summary>
/// Implemented by entities that have a dialogue tree.
/// </summary>
public interface IHasDialogueTree
{

    public DialogueMenu DialogueTree { get; set; }

}

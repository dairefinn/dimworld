namespace Dimworld.Core.Characters.Dialogue.Options;

using Godot;


/// <summary>
/// Base class for dialogue options.
/// </summary>
[GlobalClass]
public abstract partial class DialogueOption : Resource
{

    [Export] public virtual string Label { get; set; } = string.Empty;


    /// <summary>
    /// Determines if the option should be shown or not.
    /// </summary>
    /// <returns>True if the option should be shown, false otherwise.</returns>
    public virtual bool ShouldShow() {
        return true;
    }

    /// <summary>
    /// Called when the option is selected.
    /// </summary>
    /// <returns>True if the option was successfully selected, false otherwise.</returns>
    public virtual bool OnSelected() {
        return true;
    }

}

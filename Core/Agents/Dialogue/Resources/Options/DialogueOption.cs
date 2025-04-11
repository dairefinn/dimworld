namespace Dimworld.Dialogue.Options;

using Godot;


[GlobalClass]
public abstract partial class DialogueOption : Resource
{

    [Export] public virtual string Name { get; set; } = string.Empty;


    public virtual bool ShouldShow() {
        return true;
    }

    public virtual bool OnSelected() {
        return true;
    }

}

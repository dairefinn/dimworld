namespace Dimworld.Dialogue.Options;

using Godot;


[GlobalClass]
public abstract partial class DialogueOption : Resource
{

    public virtual bool ShouldShow() {
        return true;
    }

    public virtual bool OnSelected() {
        return true;
    }

}

namespace Dimworld.Dialogue.Options;

using Godot;


[GlobalClass]
public abstract partial class DialogueOption : RefCounted
{

    public virtual string Name { get; set; } = "Dialogue option";


    public virtual bool ShouldShow() {
        return true;
    }

    public virtual bool OnSelected() {
        return true;
    }

	public override string ToString()
	{
        return Name;
	}

}

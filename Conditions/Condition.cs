namespace Dimworld;

using Godot;


[GlobalClass]
public abstract partial class Condition : Resource
{
    [Export] public string Name { get; set; } = "Condition Name";
    [Export] public double Duration { get; set; } = -1f; // -1 means infinite duration

    public virtual void ApplyTo(IAffectedByConditions target)
    {
        GD.Print($"Applying condition {Name} to {target}");
        Condition conditionCopy = Duplicate() as Condition;
        target.AddCondition(conditionCopy);
    }

    public virtual void RemoveFrom(IAffectedByConditions target)
    {
        GD.Print($"Removing condition {Name} from {target}");
        target.RemoveCondition(this);
    }

    public virtual void OnProcess(double delta, IAffectedByConditions target)
    {
        UpdateDuration(delta, target);
    }


    private void UpdateDuration(double delta, IAffectedByConditions target)
    {
        Duration -= delta;
        if (Duration <= 0f)
        {
            RemoveFrom(target);
        }
    }

}

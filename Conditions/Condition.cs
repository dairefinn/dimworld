namespace Dimworld;

using Godot;


[GlobalClass]
public partial class Condition : Resource
{

    [Export] public string Name { get; set; } = "Condition Name";
    [Export] public double Duration { get; set; } = -1f; // -1 means infinite duration
    [Export] public bool Stacks { get; set; } = false;

    public virtual bool ApplyTo(IAffectedByConditions target)
    {
        GD.Print($"Trying to apply condition {Name} to {target}");

        if (!Stacks && target.HasCondition(this))
        {
            GD.Print($"Condition {Name} already exists on {target}");
            return false;
        }

        Condition conditionCopy = Duplicate() as Condition;
        return target.AddCondition(conditionCopy);
    }

    public virtual bool RemoveFrom(IAffectedByConditions target)
    {
        GD.Print($"Removing condition {Name} from {target}");
        return target.RemoveCondition(this);
    }

    public virtual void OnProcess(double delta, IAffectedByConditions target)
    {
        UpdateDuration(delta, target);
    }

    public virtual void OnPhysicsProcess(double delta, IAffectedByConditions target)
    {
        return;
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

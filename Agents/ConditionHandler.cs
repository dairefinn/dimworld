namespace Dimworld;

using System.Linq;
using Godot;
using Godot.Collections;

public partial class ConditionHandler : Node2D
{

    public Array<Condition> Conditions { get; set; } = [];


    public bool AddCondition(Condition condition)
    {
        if (condition == null) return false;
		if (Conditions.Contains(condition)) return false;

		Conditions.Add(condition);
		return true;
    }

    public bool RemoveCondition(Condition condition)
    {
		if (condition == null) return false;
		if (!HasCondition(condition)) return false;

		Conditions.Remove(condition);
		return true;
    }

	public bool HasCondition(Condition condition)
	{
		return Conditions.Where(c => c.Name == condition.Name).Any();
	}

    public void ProcessConditions(CharacterController characterController, double delta)
    {
		foreach (Condition condition in Conditions)
		{
			condition.OnProcess(delta, characterController);
		}
    }

	public void PhysicsProcessConditions(CharacterController characterController, double delta)
	{
		foreach (Condition condition in Conditions)
		{
			condition.OnPhysicsProcess(delta, characterController);
		}
	}

}

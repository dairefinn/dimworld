namespace Dimworld;

using Godot;
using Godot.Collections;


[GlobalClass]
public partial class GoapAction : Resource
{

	[Export] public string Name = "GOAP Action";

	[Export] public Dictionary<string, Variant> Preconditions;

	[Export] public Dictionary<string, Variant> Effects;

	[Export] public int Cost = 1000;


	// EVALUATION

	public bool CanPerform(IGoapAgent goapAgent, Dictionary<string, Variant> worldState)
	{
		bool staticPreconditionsSatisfied = CheckStaticPreconditions(worldState);
		bool proceduralPreconditionsSatisfied = CheckProceduralPrecondition(goapAgent);
		return staticPreconditionsSatisfied && proceduralPreconditionsSatisfied;
	}

	public bool CheckStaticPreconditions(Dictionary<string, Variant> worldState)
	{
		return GoapStateUtils.IsSubsetOf(Preconditions, worldState);
	}

	public virtual bool CheckProceduralPrecondition(IGoapAgent goapAgent)
	{
		return true;
	}

	// EXECUTION

	public virtual Dictionary<string, Variant> OnStart(IGoapAgent goapAgent, Dictionary<string, Variant> worldState)
	{
		return worldState;
	}

	public virtual bool Perform(IGoapAgent agent, Dictionary<string, Variant> worldState, double delta)
	{
		return false;
	}

	public virtual Dictionary<string, Variant> OnEnd(IGoapAgent goapAgent, Dictionary<string, Variant> worldState)
	{
		return worldState;
	}

}

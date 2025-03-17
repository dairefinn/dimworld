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


	// LIFECYCLE EVENTS

	public virtual Dictionary<string, Variant> OnStart(AgentBrain agentBrain, Dictionary<string, Variant> worldState)
	{
		GD.Print("Starting action: " + Name);
		return worldState;
	}

	public virtual bool Perform(AgentBrain agent, Dictionary<string, Variant> worldState, double delta)
	{
		GD.Print("Performing action: " + Name);
		return false;
	}

	public virtual Dictionary<string, Variant> OnEnd(AgentBrain agentBrain, Dictionary<string, Variant> worldState)
	{
		GD.Print("Ending action: " + Name);
		return worldState;
	}


	// EXECUTION CHECKS

	public bool CanPerform(AgentBrain agentBrain, Dictionary<string, Variant> worldState)
	{
		bool staticPreconditionsSatisfied = GoapStateUtils.IsSubsetOf(Preconditions, worldState);
		bool proceduralPreconditionsSatisfied = CheckProceduralPrecondition(agentBrain);
		return staticPreconditionsSatisfied && proceduralPreconditionsSatisfied;
	}

	public virtual bool CheckProceduralPrecondition(AgentBrain agentBrain)
	{
		return true;
	}

}

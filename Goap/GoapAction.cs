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

	public virtual Dictionary<string, Variant> OnStart(CharacterController characterController, Dictionary<string, Variant> worldState)
	{
		return worldState;
	}

	public virtual bool Perform(CharacterController agent, Dictionary<string, Variant> worldState, double delta)
	{
		return false;
	}

	public virtual Dictionary<string, Variant> OnEnd(CharacterController characterController, Dictionary<string, Variant> worldState)
	{
		return worldState;
	}


	// EXECUTION CHECKS

	public bool CanPerform(CharacterController characterController, Dictionary<string, Variant> worldState)
	{
		bool staticPreconditionsSatisfied = CheckStaticPreconditions(worldState);
		bool proceduralPreconditionsSatisfied = CheckProceduralPrecondition(characterController);
		return staticPreconditionsSatisfied && proceduralPreconditionsSatisfied;
	}

	public bool CheckStaticPreconditions(Dictionary<string, Variant> worldState)
	{
		return GoapStateUtils.IsSubsetOf(Preconditions, worldState);
	}

	public virtual bool CheckProceduralPrecondition(CharacterController characterController)
	{
		return true;
	}

}

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

	public virtual bool Perform(AgentController agent, double delta)
	{
		GD.Print("Performing action: " + Name);
		return false;
	}

	// TODO: Implement these in the planner. Currently it only supports static preconditions and effects. This could be used to check if the player is in range of the agent performing the action.
	public virtual bool CheckProceduralPrecondition(AgentController agent)
	{
		return true;
	}

}

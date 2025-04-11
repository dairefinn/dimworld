namespace Dimworld.GOAP;

using Godot;


[GlobalClass]
public partial class GoapAction : Resource
{

	[Export] public string Name = "GOAP Action";

	[Export] public int Cost = 1000;


    public GoapState Preconditions => GetPreconditions();
	public GoapState Effects => GetEffects();


	public GoapAction() {}
	
	public GoapAction(GoapAction action)
	{
		Name = action.Name;
		Cost = action.Cost;
	}


    // DYNAMIC PROPERTIES

    public virtual GoapState GetPreconditions()
	{
		return GoapState.Empty;
	}

	public virtual GoapState GetEffects()
	{
		return GoapState.Empty;
	}


	// EVALUATION

	/// <summary>
	/// Called before the action is evaluated. This is where you can set up any dynamic properties that are needed for the action.
	/// For example, the equip action needs a target item to be set before it can be evaluated.
	/// </summary>
	/// <param name="worldState"></param>
	public virtual void PreEvaluate(GoapState worldState, GoapState desiredState)
	{
	}

	public virtual bool CheckStaticPreconditions(GoapState worldState)
	{
		return Preconditions.IsSubsetOf(worldState);
	}

	public virtual bool CheckProceduralPrecondition(IGoapAgent goapAgent, GoapState worldState)
	{
		return true;
	}

	public virtual bool CanSatisfy(GoapState desiredState)
	{
		return Effects.IsSubsetOf(desiredState);
	}


	// EXECUTION

	public virtual GoapState OnStart(IGoapAgent goapAgent, GoapState worldState)
	{
		return worldState;
	}

	public virtual bool Perform(IGoapAgent agent, GoapState worldState, double delta)
	{
		return false;
	}

	public virtual GoapState OnEnd(IGoapAgent goapAgent, GoapState worldState)
	{
		return worldState;
	}

}

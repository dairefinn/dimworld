namespace Dimworld.GOAP;

using Godot;
using Godot.Collections;

public interface IGoapAgent
{

	public Vector2 GlobalPositionThreadSafe { get; set; }

    [Export] public GoapState WorldState { get; set; }
	[Export] public Array<GoapAction> ActionSet { get; set; }
	[Export] public Array<GoapGoal> GoalSet { get; set; }

}
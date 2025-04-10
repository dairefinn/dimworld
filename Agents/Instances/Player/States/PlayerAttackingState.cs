namespace Dimworld.Agents.Instances.States;

using Dimworld.States;
using Godot;


public partial class PlayerAttackingState : State<Player>
{

    public override string Key { get; set; } = "ATTACKING";

}

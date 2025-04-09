namespace Dimworld.Agents.Instances;

using Dimworld.Levels;


public partial class Player : CharacterController, ICanTriggerLevelTransitions
{

    public Player()
    {
        Globals.Instance.Player = this;
    }

}

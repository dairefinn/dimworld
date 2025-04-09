namespace Dimworld.GOAP.Actions;

using Dimworld.Agents;
using Godot;
using Godot.Collections;


public partial class Idle : GoapAction
{

    [Export] public float IdleTime = 5.0f; // Time to wait before moving again
    [Export] public float Radius = 50.0f; // Radius to move around the character


    private Vector2 currentTarget = Vector2.Zero;
    private float currentIdleTimeRemaining = 0.0f;


    public override GoapState GetEffects()
    {
        return new GoapState(new Dictionary<string, Variant> {
            {"current_action", "idle"}
        });
    }

    public override bool Perform(IGoapAgent agent, GoapState worldState, double delta)
    {
        if (agent is not CharacterController characterController) return false;

        // Check if the idle time has elapsed
        currentIdleTimeRemaining -= (float)delta;
        if (currentIdleTimeRemaining >= 0.0f) return false;

        // Get a random position around the character
        Vector2 randomPosition = characterController.GlobalPosition;
        randomPosition += new Vector2((float)GD.RandRange(-Radius, Radius), (float)GD.RandRange(-Radius, Radius));
        characterController.NavigateTo(randomPosition);

        // Reset the idle timer
        currentIdleTimeRemaining = IdleTime;

        // Return false so the action repeats
        return false;
    }

}

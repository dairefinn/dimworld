namespace Dimworld;

using System.Linq;
using Godot;


public partial class InputHandler : Node2D
{

    [Export] public AgentController PlayerAgent { get; set; }

    // TODO: These were added temporarily to test the GOAP system
    [Export] public AgentBrain TempAgentBrain { get; set; }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (Input.IsActionJustPressed("lmb"))
        {
            GD.Print("Setting target to mouse position: " + GetGlobalMousePosition());
            Vector2 mousePosition = GetGlobalMousePosition();
            PlayerAgent.NavigateTo(mousePosition);
        }

        if (Input.IsActionJustPressed("rmb"))
        {
            PlayerAgent.StopNavigating();
        }

        if (Input.IsActionJustPressed("toggle_lights"))
        {
            // TODO: Make world state global or add a "Vision" system to the agent which will sync their world state with the global world state depending on what they can observe.
            bool existingValue = (bool) GoapStateUtils.GetState(TempAgentBrain.WorldState, "lights_on", false);
            GoapStateUtils.SetState(TempAgentBrain.WorldState, "lights_on", !existingValue);
            GD.Print("Lights are now " + (existingValue ? "OFF" : "ON"));
            Light2D Light = GetTree().GetNodesInGroup("lights").OfType<Light2D>().FirstOrDefault();
            Light.Enabled = !existingValue;
        }
    }

}

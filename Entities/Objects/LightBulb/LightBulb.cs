namespace Dimworld.Entities.Objects;

using Dimworld.Core.Characters.Memory;
using Dimworld.Core.Characters.Memory.MemoryEntries;
using Godot;


/// <summary>
/// A light bulb that can be turned on or off.
/// </summary>
public partial class LightBulb : StaticBody2D, IMemorableNode
{

	[Export] public Light2D Light { get; set; }
	[Export] public bool IsOn {
		get {
			return Light.Enabled;
		}
		set {
			Light.Enabled = value;
		}
	}


	/// <summary>
	/// Turns the light bulb on.
	/// </summary>
	public void TurnOn()
	{
		IsOn = true;
	}

	/// <summary>
	/// Turns the light bulb off.
	/// </summary>
	public void TurnOff()
	{
		IsOn = false;
	}

	/// <summary>
	/// Toggles the light bulb on or off.
	/// </summary>
	public void Toggle()
	{
		IsOn = !IsOn;
	}


	// INTERFACES

    public NodeLocation GetNodeLocationMemory()
    {
        return new NodeLocation()
        {
            Node = this,
            Position = GlobalPosition
        };
    }

}

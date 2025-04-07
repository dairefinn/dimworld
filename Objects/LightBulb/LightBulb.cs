namespace Dimworld.Objects;

using Dimworld.Memory;
using Dimworld.Memory.MemoryEntries;
using Godot;


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

	public void TurnOn()
	{
		IsOn = true;
	}

	public void TurnOff()
	{
		IsOn = false;
	}

	public void Toggle()
	{
		IsOn = !IsOn;
	}

    public NodeLocation GetNodeLocationMemory()
    {
        return new NodeLocation()
        {
            Node = this,
            Position = GlobalPosition
        };
    }

}

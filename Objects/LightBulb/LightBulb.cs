namespace Dimworld;

using Godot;

public partial class LightBulb : StaticBody2D
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

}

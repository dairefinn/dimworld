namespace Dimworld;

using Godot;
using Godot.Collections;


public partial class LightSwitch : StaticBody2D, ICanBeInteractedWith
{

    [Export] public Array<LightBulb> AssociatedLights { get; set; }

    [Export] public bool IsOn {
        get => _isOn;
        set => OnSetIsOn(value);
    }
    private bool _isOn = false;

    public bool HasAssociatedLights()
    {
        if (AssociatedLights == null) return false;
        if (AssociatedLights.Count == 0) return false;
        return true;
    }

    public bool ControlsLight(LightBulb light)
    {
        if (!HasAssociatedLights()) return false;
        return AssociatedLights.Contains(light);
    }

    public bool ControlsAny(Array<LightBulb> lights)
    {
        foreach (LightBulb light in lights)
        {
            if (AssociatedLights.Contains(light)) return true;
        }
        return false;
    }

    private void OnSetIsOn(bool value)
    {
        _isOn = value;
        foreach (LightBulb light in AssociatedLights)
        {
            light.IsOn = value;
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

    public void InteractWith()
    {
        GD.Print("Interacting with chest");
    }
}

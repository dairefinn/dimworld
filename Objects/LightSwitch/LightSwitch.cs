namespace Dimworld;

using Godot;
using Godot.Collections;
using System.Linq;


public partial class LightSwitch : StaticBody2D
{

    [Export] public Array<LightBulb> AssociatedLights { get; set; }

    [Export] public bool IsOn {
        get => _isOn;
        set {
            _isOn = value;
            if (_isOn)
            {
                TurnOn();
            }
            else
            {
                TurnOff();
            }
        }
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

    public void TurnOn()
    {
        foreach (LightBulb light in AssociatedLights)
        {
            light.TurnOn();
        }
    }

    public void TurnOff()
    {
        foreach (LightBulb light in AssociatedLights)
        {
            light.TurnOff();
        }
    }

    public void Toggle()
    {
        if (!HasAssociatedLights()) return;
        foreach (LightBulb light in AssociatedLights)
        {
            light.Toggle();
        }
    }

}

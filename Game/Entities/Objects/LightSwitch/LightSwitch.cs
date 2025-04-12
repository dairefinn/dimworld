namespace Dimworld.Entities.Objects;

using Dimworld.Core.Characters.Memory;
using Dimworld.Core.Characters.Memory.MemoryEntries;
using Dimworld.Core.Interaction;
using Godot;
using Godot.Collections;


/// <summary>
/// A light switch that can control multiple lights.
/// </summary>
public partial class LightSwitch : StaticBody2D, ICanBeInteractedWith, IMemorableNode
{

    [Export] public Array<LightBulb> AssociatedLights { get; set; }

    [Export] public bool IsOn {
        get => _isOn;
        set => OnSetIsOn(value);
    }
    private bool _isOn = false;


    private void OnSetIsOn(bool value)
    {
        _isOn = value;
        foreach (LightBulb light in AssociatedLights)
        {
            light.IsOn = value;
        }
    }


    /// <summary>
    /// Checks if the light switch has any associated lights.
    /// </summary>
    /// <returns>True if the light switch has associated lights, false otherwise.</returns>
    public bool HasAssociatedLights()
    {
        if (AssociatedLights == null) return false;
        if (AssociatedLights.Count == 0) return false;
        return true;
    }

    /// <summary>
    /// Checks if the light switch controls a specific light.
    /// </summary>
    /// <param name="light"></param>
    /// <returns>True if the light switch controls the light, false otherwise.</returns>
    public bool ControlsLight(LightBulb light)
    {
        if (!HasAssociatedLights()) return false;
        return AssociatedLights.Contains(light);
    }

    /// <summary>
    /// Checks if the light switch controls any of the lights in the array.
    /// </summary>
    /// <param name="lights">An array of lights to check.</param>
    /// <returns>True if the light switch controls any of the lights, false otherwise.</returns>
    public bool ControlsAny(Array<LightBulb> lights)
    {
        foreach (LightBulb light in lights)
        {
            if (AssociatedLights.Contains(light)) return true;
        }
        return false;
    }

    /// <summary>
    /// Turns on the light switch and all associated lights.
    /// </summary>
    public void TurnOn()
    {
        IsOn = true;
    }

    /// <summary>
    /// Turns off the light switch and all associated lights.
    /// </summary>
    public void TurnOff()
    {
        IsOn = false;
    }

    /// <summary>
    /// Toggles the light switch and all associated lights.
    /// </summary>
    public void Toggle()
    {
        IsOn = !IsOn;
    }

    /// <summary>
    /// Interacts with the light switch, toggling its state.
    /// </summary>
    public void InteractWith()
    {
        Toggle();
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

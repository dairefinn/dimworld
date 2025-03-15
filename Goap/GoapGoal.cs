namespace Dimworld;

using Godot;
using Godot.Collections;

[GlobalClass]
public partial class GoapGoal : Resource
{

    [Export] public string Name { get; set; }

    [Export] public int Priority { get; set; }

    [Export] public Dictionary<string, Variant> DesiredState { get; set; }

    public bool IsValid()
    {
        return true;
    }

}

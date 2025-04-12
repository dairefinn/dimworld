namespace Dimworld.Core.Factions;

using Godot;
using Godot.Collections;


/// <summary>
/// Represents a faction in the game.
/// </summary>
[GlobalClass]
public partial class Faction : Resource
{

    [Export] public string Name { get; set; } = "Faction Name";
    [Export] public string Description { get; set; } = "Description of the faction.";
    [Export] public Color Color { get; set; } = Colors.White;
    [Export] public Texture2D Icon { get; set; }
    [Export] public Dictionary<Faction, float> Relations { get; private set; } = [];


    /// <summary>
    /// Sets the affinity towards another faction.
    /// </summary>
    /// <param name="otherFaction">The other faction to set the affinity with.</param>
    /// <param name="affinity">The affinity value to set. Should be between -1 and 1.</param>
    public void SetAffinity(Faction otherFaction, float affinity)
    {
        if (otherFaction == null) return;
        if (otherFaction == this) return;

        float newValue = Mathf.Clamp(affinity, -1, 1);

        Relations[otherFaction] = newValue;
        otherFaction.Relations[this] = newValue;
    }

    /// <summary>
    /// Retrieves the affinity towards another faction. Defaults to 0 if not set.
    /// </summary>
    /// <param name="otherFaction">The other faction to get the affinity with.</param>
    /// <returns>The affinity value. Returns 0 if not set.</returns>
    public float GetAffinity(Faction otherFaction)
    {
        if (otherFaction == null) return 0;
        if (otherFaction == this) return 0;

        return Relations.TryGetValue(otherFaction, out var affinity) ? affinity : 0;
    }

}

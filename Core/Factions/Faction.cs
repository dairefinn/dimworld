namespace Dimworld.Core.Factions;

using Godot;
using Godot.Collections;


/// <summary>
/// Represents a faction in the game.
/// </summary>
[GlobalClass]
public partial class Faction : Resource
{

    /// <summary>
    /// Checks if a source node can access a target node based on their faction affiliations.
    /// This is used to determine if a character can interact with another character or object.
    /// </summary>
    /// <param name="source">The node trying to access the target node.</param>
    /// <param name="target">The node being accessed.</param>
    /// <returns>True if the source node can access the target node, false otherwise.</returns>
    public static bool CanAccessNode(object source, object target)
    {
        // Source and target must exist
        if (source == null) return false;
        if (target == null) return false;

        Faction sourceFaction = GetAffiliationFor(source);
        Faction targetFaction = GetAffiliationFor(target);

        // If target has no affiliation, we can search it
        if (targetFaction == null) return true;

        // If source AND target have no affiliation, we can search it
        if (sourceFaction == null)
        {
            return targetFaction == null;
        }

        // If source and target have the same affiliation, we can search it
        if (sourceFaction == targetFaction)
        {
            return true;
        }

        // If source and target have different affiliations, check the relations
        if (sourceFaction.Relations.TryGetValue(targetFaction, out var affinity))
        {
            // If the affinity is greater than 0, we can search it
            if (affinity > 0)
            {
                return true;
            }
        }

        // If we reach this point, we cannot search it
        return false;
    }

    /// <summary>
    /// Retrieves the faction affiliation of a node.
    /// </summary>
    /// <param name="object">The object to check for a faction affiliation.</param>
    /// <returns>>The faction affiliation of the node, or null if it doesn't have one.</returns>
    public static Faction GetAffiliationFor(object @object)
    {
        if (@object == null) return null;
        if (@object is not IHasFactionAffiliation factionAffiliation) return null;
        return factionAffiliation.Affiliation;
    }


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

namespace Dimworld.Core.Factions;

using Godot;


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

}

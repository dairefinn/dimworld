namespace Dimworld.Core.Levels;

using Godot;


/// <summary>
/// A node which represents a game level. This is used to store any required data about the level.
/// </summary>
[GlobalClass]
public partial class GameLevel : Node2D
{

    [Export] public string LevelName;
    [Export] public SpawnPoint DefaultSpawnPoint;
    [Export] public NavigationRegion2D NavigationRegion;

}

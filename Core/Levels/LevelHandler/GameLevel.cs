namespace Dimworld.Levels;

using Godot;


public partial class GameLevel : Node2D
{

    [Export] public string LevelName;
    [Export] public SpawnPoint DefaultSpawnPoint;
    [Export] public NavigationRegion2D NavigationRegion;

}

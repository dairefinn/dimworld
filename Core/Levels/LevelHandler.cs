namespace Dimworld.Core.Levels;

using System.Linq;
using Dimworld.Characters;
using Godot;


/// <summary>
/// A handler for managing the current level and transitioning to new levels.
/// </summary>
[GlobalClass]
public partial class LevelHandler : Node
{

    public GameLevel CurrentLevel { get; private set; } = null;


    private void UnloadCurrentLevel()
    {
        foreach (Node child in GetChildren())
        {
            if (child is GameLevel gameLevel)
            {
                gameLevel.QueueFree();
            }
        }
    }

    // FIXME: There's an issue here that occurs if the current level has a spawn point with the same name as the target spawn point in the new level. It will use the location of the spawn point in the previous level instead.
    private void LoadLevel(string newLevelPath, string spawnPointName)
    {
        PackedScene newLevelScene = ResourceLoader.Load<PackedScene>(newLevelPath);
        GameLevel gameLevel = newLevelScene.Instantiate<GameLevel>();
        AddChild(gameLevel);
        CurrentLevel = gameLevel;
        CallDeferred(MethodName.MovePlayerToSpawnPoint, [gameLevel, spawnPointName]);
    }

    private void MovePlayerToSpawnPoint(GameLevel currentLevel, string spawnPointName)
    {
        SpawnPoint spawnPoint = currentLevel.DefaultSpawnPoint;

        if (spawnPointName != null && spawnPointName.Length > 0)
        {
            spawnPoint = GetTree().GetNodesInGroup("spawnpoint").Where(node => node.Name == spawnPointName).FirstOrDefault(spawnPoint) as SpawnPoint;
        }

        if(spawnPoint != null)
        {
            Player player = Globals.Instance.Player;
            if (player != null)
            {
                player.GlobalPosition = spawnPoint.GlobalPosition;
            }
        }
    }


    public override void _Ready()
    {
        base._Ready();

        CurrentLevel = GetChildren().Where(child => child is GameLevel).FirstOrDefault() as GameLevel;
    }


    /// <summary>
    /// Change the current level to a new level.
    /// This will unload the current level and load the new level.
    /// </summary>
    /// <param name="newLevelPath">The path to the new level.</param>
    /// <param name="spawnPoint">The ID of the spawn point to spawn at.</param>
    public void ChangeLevel(string newLevelPath, string spawnPoint = "")
    {
        if (newLevelPath == null)
        {
            GD.PrintErr("New level == null");
            return;
        }

        UnloadCurrentLevel();
        CallDeferred(MethodName.LoadLevel, [newLevelPath, spawnPoint]);
    }

}

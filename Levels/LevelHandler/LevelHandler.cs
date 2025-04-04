namespace Dimworld;

using System.Linq;
using Godot;


public partial class LevelHandler : Node
{

    public static LevelHandler Instance { get; private set; }

    public LevelHandler()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void ChangeLevel(string newLevelPath, string spawnPoint = "")
    {
        if (newLevelPath == null)
        {
            GD.PrintErr("New level is null");
            return;
        }

        UnloadCurrentLevel();
        CallDeferred(MethodName.LoadLevel, [newLevelPath, spawnPoint]);
    }

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
            CharacterController player = Globals.Instance.Player;
            if (player != null)
            {
                player.GlobalPosition = spawnPoint.GlobalPosition;
            }
        }
    }

}

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

    private void LoadLevel(string newLevelPath, string spawnPointName)
    {
        PackedScene newLevelScene = ResourceLoader.Load<PackedScene>(newLevelPath);
        GameLevel gameLevel = newLevelScene.Instantiate<GameLevel>();
        AddChild(gameLevel);

        SpawnPoint spawnPoint = gameLevel.DefaultSpawnPoint;
        if (spawnPointName != null && spawnPointName.Length > 0)
        {
            spawnPoint = GetTree().GetNodesInGroup("spawnpoint").Where(node => node.Name == spawnPointName).FirstOrDefault(spawnPoint) as SpawnPoint;
        }

        if(spawnPoint != null)
        {
            CharacterController player = InputHandler.Instance.PlayerAgent;
            if (player != null)
            {
                player.GlobalPosition = spawnPoint.GlobalPosition;
            }
        }
    }

}

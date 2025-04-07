namespace Dimworld.Levels;

using Godot;


public partial class LevelTransitionArea : Area2D
{

    [Export(PropertyHint.File, "*.tscn")] public string TargetLevelPath;
    [Export] public string SpawnPointName;

    public override void _Ready()
    {
        base._Ready();

        BodyEntered += OnBodyEntered;
    }


    private void OnBodyEntered(Node2D body)
    {
        if (body is CharacterController character && Globals.Instance.Player == character)
        {
            LevelHandler.Instance.ChangeLevel(TargetLevelPath, SpawnPointName);
        }
    }

}

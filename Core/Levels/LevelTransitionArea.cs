namespace Dimworld.Core.Levels.Transitions;

using Godot;


/// <summary>
/// An area that triggers a level transition when entered by an entity which implements ICanTriggerLevelTransitions.
/// </summary>
[GlobalClass]
public partial class LevelTransitionArea : Area2D
{

    [Export(PropertyHint.File, "*.tscn")] public string TargetLevelPath;
    [Export] public string SpawnPointName;


    private void OnBodyEntered(Node2D body)
    {
        if (body is ICanTriggerLevelTransitions)
        {
            Globals.Instance.LevelHandler.ChangeLevel(TargetLevelPath, SpawnPointName);
        }
    }


    public override void _Ready()
    {
        base._Ready();

        BodyEntered += OnBodyEntered;
    }

}

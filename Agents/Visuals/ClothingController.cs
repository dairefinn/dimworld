namespace Dimworld;

using Godot;


public partial class ClothingController : Sprite2D
{
    
	[ExportGroup("Visuals")]
	[Export] public Sprite2D HairSprite { get; set; }
	[Export] public Sprite2D EyesSprite { get; set; }
	[Export] public Sprite2D TopSprite { get; set; }
	[Export] public Sprite2D OvertopSprite { get; set; }
	[Export] public Sprite2D BottomsSprite { get; set; }
	[Export] public Sprite2D ShoesSprite { get; set; }
	[Export] public bool Blinking { get; set; }


    private SceneTreeTimer blinkTimer;


    public override void _Ready()
    {
        base._Ready();

        StartBlinking();
    }

    private void SetEyesState(bool eyesOpen)
    {
        if (!Blinking) return;

        EyesSprite.Visible = eyesOpen;
        blinkTimer = GetTree().CreateTimer(0.25f);
        blinkTimer.Timeout += ToggleEyesState;
    }

    private void ToggleEyesState()
    {
        if (!Blinking) return;

        EyesSprite.Visible = !EyesSprite.Visible;
        blinkTimer = GetTree().CreateTimer(0.25f);
        blinkTimer.Timeout += ToggleEyesState;
    }


    public void StartBlinking()
    {
        ToggleEyesState();
    }

    public void StopBlinking(bool eyesOpen = true)
    {
        if (blinkTimer != null)
        {
            blinkTimer.Timeout -= ToggleEyesState;
            blinkTimer.EmitSignal(SceneTreeTimer.SignalName.Timeout);
        }

        Blinking = false;
        EyesSprite.Visible = eyesOpen;
    }


}

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


    private SceneTreeTimer blinkTimer;


    public override void _Ready()
    {
        base._Ready();

        blinkTimer = GetTree().CreateTimer(2f);
        blinkTimer.Timeout += () => CloseEyes();
    }

    private void CloseEyes()
    {
        EyesSprite.Visible = false;
        blinkTimer = GetTree().CreateTimer(0.25f);
        blinkTimer.Timeout += () => OpenEyes();
    }

    private void OpenEyes()
    {
        EyesSprite.Visible = true;
        blinkTimer = GetTree().CreateTimer(2f);
        blinkTimer.Timeout += () => CloseEyes();
    }


}

namespace Dimworld.Developer;

using Godot;


public partial class DeveloperMenu : Control
{

    public static DeveloperMenu Instance { get; private set; }


    public static bool IsOpen => Instance != null && Instance.Visible;


    private Button ButtonClose;
    private DeveloperConsole Console;


    public DeveloperMenu()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            GD.PrintErr("DeveloperMenu: Attempted to create multiple instances of DeveloperMenu.");
            QueueFree();
        }
    }


    public override void _Ready()
    {
        base._Ready();

        ButtonClose = GetNode<Button>("%ButtonClose");
        ButtonClose.Pressed += ToggleVisibility;

        Console = GetNode<DeveloperConsole>("%DeveloperConsole");
    }

    
    public void ToggleVisibility()
    {
        if (Visible)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    private new void Show()
    {
        base.Show();

        Console?.ClearInput();
    }

}

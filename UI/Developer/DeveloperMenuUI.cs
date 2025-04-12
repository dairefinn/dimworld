namespace Dimworld.UI.Developer;

using Dimworld.Core.Developer;
using Godot;


public partial class DeveloperMenuUI : Control
{

    private Button ButtonClose;
    private DeveloperConsole Console;


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
        Console.FocusConsoleInput();
    }

}

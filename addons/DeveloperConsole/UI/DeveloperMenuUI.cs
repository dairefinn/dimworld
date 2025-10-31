namespace DaireFinn.Plugins.DeveloperConsole.UI;

using Godot;
using Dimworld.Core.Developer;


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

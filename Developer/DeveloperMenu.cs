namespace Dimworld.Developer;

using Godot;


public partial class DeveloperMenu : Control
{

    public static DeveloperMenu Instance { get; private set; }


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

}

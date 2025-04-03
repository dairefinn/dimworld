namespace Dimworld.Developer;

using Godot;
using System;


public partial class DeveloperConsole : PanelContainer
{

    /// <summary>
    /// Determines if the console should print to the editor's debug console. For the most part, it should be fine to leave this as false and look at most things through the in-game console but this could be useful if having crashes or other runtime issues.
    /// </summary>
    [Export] public bool PrintToDebugConsole = false;

    public static DeveloperConsole Instance { get; private set; }

    private VBoxContainer consoleEntriesContainer;
    private LineEdit consoleInput;
    private bool isMouseOver = false;


    public bool IsFocused => consoleInput != null && consoleInput.HasFocus();


    public DeveloperConsole()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            GD.PrintErr("DeveloperConsole: Attempted to create multiple instances of DeveloperConsole.");
            QueueFree();
        }
    }


    public override void _Ready()
    {
        consoleEntriesContainer = GetNode<VBoxContainer>("%ConsoleEntries");
        consoleInput = GetNode<LineEdit>("%ConsoleInput");

        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;

        consoleInput.TextSubmitted += OnSubmitConsoleInput;
    }


    public void AddConsoleEntry(string text)
    {
        Label entry = new()
        {
            Text = text
        };
        consoleEntriesContainer.AddChild(entry);

        if (PrintToDebugConsole)
        {
            GD.Print(text);
        }
    }

    public override void _GuiInput(InputEvent @event)
	{
		if (!isMouseOver) return;
        if (@event.IsActionPressed("lmb"))
        {
            GD.Print("Left mouse button pressed on console.");
            FocusConsoleInput();
        }
	}

    private void OnMouseEntered()
    {
		isMouseOver = true;
    }

	private void OnMouseExited()
	{
		isMouseOver = false;
	}

    private void OnSubmitConsoleInput(string text)
    {
        AddConsoleEntry(text);
        consoleInput.Clear();
        FocusConsoleInput();
    }

    private void FocusConsoleInput()
    {
        if (consoleInput == null) return;
        consoleInput.ReleaseFocus();
        consoleInput.CallDeferred(LineEdit.MethodName.GrabFocus);        
    }

}

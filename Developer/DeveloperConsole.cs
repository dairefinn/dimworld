namespace Dimworld.Developer;

using Godot;
using System;


public partial class DeveloperConsole : PanelContainer
{

    private VBoxContainer consoleEntriesContainer;
    private LineEdit consoleInput;
    private bool isMouseOver = false;

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
        GD.Print(text);
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

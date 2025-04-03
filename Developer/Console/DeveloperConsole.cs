namespace Dimworld.Developer;

using Godot;
using Godot.Collections;
using System;


public partial class DeveloperConsole : PanelContainer
{
    public static DeveloperConsole Instance { get; private set; }

    public static void Print(string text, Color? color = null)
    {
        if (Instance == null)
        {
            GD.PrintErr("DeveloperConsole: Attempted to add console entry before DeveloperConsole was initialized.");
            return;
        }

        if (color == null)
        {
            color = Colors.White;
        }

        Instance.AddConsoleEntry(text, color.Value);
    }

    public static bool IsFocused => Instance != null && Instance.IsInstanceFocused;


    /// <summary>
    /// Determines if the console should print to the editor's debug console. For the most part, it should be fine to leave this as false and look at most things through the in-game console but this could be useful if having crashes or other runtime issues.
    /// </summary>
    [Export] public bool PrintToDebugConsole = false;
    public bool IsInstanceFocused => consoleInput != null && consoleInput.HasFocus();


    private ScrollContainer consoleScrollContainer;
    private VBoxContainer consoleEntriesContainer;
    private LineEdit consoleInput;
    private bool isMouseOver = false;


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
        consoleScrollContainer = GetNode<ScrollContainer>("%ConsoleScrollContainer");
        consoleEntriesContainer = GetNode<VBoxContainer>("%ConsoleEntries");
        consoleInput = GetNode<LineEdit>("%ConsoleInput");

        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;

        consoleInput.TextSubmitted += OnSubmitConsoleInput;
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


    public void ClearInput()
    {
        if (consoleInput == null) return;
        consoleInput.Clear();
    }


    private void AddConsoleEntry(string text, Color color)
    {
        // Print before adding just incase it breaks
        if (PrintToDebugConsole)
        {
            GD.Print(text);
        }

        // NOTE: This has to be a primitive because CallDeferred arguments have to be either primitive types or extensions of Godot.Variant
        ulong colorPrimitive = color.ToRgba64();

        try
        {
            if (consoleEntriesContainer == null)
            {
                CallDeferred(MethodName.AddEntryToList, [text, colorPrimitive]);
            }
            else
            {
                AddEntryToList(text, colorPrimitive);
            }
        }
        catch (Exception e)
        {
            GD.PrintErr("DeveloperConsole: Failed to add console entry: " + e);
            throw;
        }
    }

    private void AddEntryToList(string text, ulong colorPrimitive)
    {
        Label entry = new()
        {
            Text = text,
            LabelSettings = new LabelSettings()
            {
                FontColor = new(colorPrimitive)
            }
        };

        consoleEntriesContainer.AddChild(entry);
        CallDeferred(MethodName.ScrollToBottom);
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
        Print(text);
        DeveloperConsoleCommandHandler.HandleCommand(text);
        consoleInput.Clear();
        FocusConsoleInput();
    }

    private void FocusConsoleInput()
    {
        if (consoleInput == null) return;
        consoleInput.ReleaseFocus();
        consoleInput.CallDeferred(LineEdit.MethodName.GrabFocus);
    }

    private void ScrollToBottom()
    {
        if (consoleScrollContainer == null) return;

        // This timer is a bit of a hack but it was necessary becasue CallDeferred wasn't keeping up with the UI updates.
        GetTree().CreateTimer(0.01f).Timeout += () => {
            consoleScrollContainer.ScrollVertical = (int)consoleScrollContainer.GetVScrollBar().MaxValue;
        };
    }

}

namespace DaireFinn.Plugins.DeveloperConsole;

using Godot;
using Godot.Collections;
using System;
using DaireFinn.Plugins.DeveloperConsole.Utils;

/// <summary>
/// The DeveloperConsole class is a singleton that manages the developer console UI.
/// </summary>
public partial class DeveloperConsole : PanelContainer
{
    public static DeveloperConsole Instance { get; private set; }

    public static void Print(string text)
    {
        if (Instance == null)
        {
            GD.PrintErr("DeveloperConsole: Attempted to add console entry before DeveloperConsole was initialized.");
            return;
        }

        Instance.AddConsoleEntry(text);
    }

    public static void PrintSuccess(string text)
    {
        if (Instance == null)
        {
            GD.PrintErr("DeveloperConsole: Attempted to add console entry before DeveloperConsole was initialized.");
            return;
        }

        Instance.AddConsoleEntry(BBCodeHelper.Colors.Green(text));
    }

    public static void PrintInfo(string text)
    {
        if (Instance == null)
        {
            GD.PrintErr("DeveloperConsole: Attempted to add console entry before DeveloperConsole was initialized.");
            return;
        }

        Instance.AddConsoleEntry(BBCodeHelper.Colors.Blue(text));
    }

    public static void PrintWarning(string text)
    {
        if (Instance == null)
        {
            GD.PrintErr("DeveloperConsole: Attempted to add console entry before DeveloperConsole was initialized.");
            return;
        }

        Instance.AddConsoleEntry(BBCodeHelper.Colors.Yellow(text));
    }

    public static void PrintErr(string text)
    {
        if (Instance == null)
        {
            GD.PrintErr("DeveloperConsole: Attempted to add console entry before DeveloperConsole was initialized.");
            return;
        }

        Instance.AddConsoleEntry(BBCodeHelper.Colors.Red(text));
    }

    public static void Clear()
    {
        if (Instance == null)
        {
            GD.PrintErr("DeveloperConsole: Attempted to clear console before DeveloperConsole was initialized.");
            return;
        }

        Instance.ClearInstance();
    }

    public static bool IsFocused => Instance != null && Instance.IsInstanceFocused;


    /// <summary>
    /// Determines if the console should print to the editor's debug console. For the most part, it should be fine to leave this as false and look at most things through the in-game console but this could be useful if having crashes or other runtime issues.
    /// This value is loaded from project settings: developer_console/print_to_debug_console
    /// </summary>
    public bool PrintToDebugConsole { get; private set; } = false;
    public bool IsInstanceFocused => consoleInput != null && consoleInput.HasFocus();


    private ScrollContainer consoleScrollContainer;
    private VBoxContainer consoleEntriesContainer;
    private LineEdit consoleInput;
    private bool isMouseOver = false;


    private Array<string> typedCommandHistory = [];
    private int historyIndex = 0; // This is in reverse order and 0 means you're not looking at any history so 1 is the most recent command.


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

        // Load PrintToDebugConsole setting from project settings
        if (ProjectSettings.HasSetting("developer_console/print_to_debug_console"))
        {
            PrintToDebugConsole = (bool)ProjectSettings.GetSetting("developer_console/print_to_debug_console");
        }
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (isMouseOver && @event is InputEventMouseButton mouseButton &&
            mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed)
        {
            FocusConsoleInput();
        }
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        // If the up arrow is pressed, show the previous command
        if (IsFocused && @event.IsActionPressed("ui_up"))
        {
            ShowPreviousCommand();
        }
        else if (@event is InputEventKey keyEvent && keyEvent.IsPressed())
        {
            historyIndex = 0; // Reset the history index if any other key is pressed
        }
    }



    public void ClearInstance()
    {
        if (consoleEntriesContainer == null) return;

        foreach (Node child in consoleEntriesContainer.GetChildren())
        {
            child.QueueFree();
        }

        typedCommandHistory.Clear();
        historyIndex = 0;
    }

    public void ClearInput()
    {
        if (consoleInput == null) return;
        consoleInput.Clear();
    }

    public void ShowPreviousCommand()
    {
        if (typedCommandHistory.Count == 0) return;

        historyIndex++;
        if (historyIndex > typedCommandHistory.Count)
        {
            historyIndex = typedCommandHistory.Count;
        }

        int index = typedCommandHistory.Count - historyIndex;
        string command = typedCommandHistory[index];
        consoleInput.Text = command;
    }

    public void FocusConsoleInput()
    {
        if (consoleInput == null) return;
        consoleInput.ReleaseFocus();
        consoleInput.CallDeferred(LineEdit.MethodName.GrabFocus);
    }


    private void AddConsoleEntry(string text)
    {
        // Print before adding just incase it breaks
        if (PrintToDebugConsole)
        {
            GD.Print(text);
        }

        try
        {
            // Always use CallDeferred to ensure we're on the main thread when modifying UI
            if (consoleEntriesContainer == null)
            {
                CallDeferred(MethodName.AddEntryToList, text);
            }
            else
            {
                AddEntryToList(text);
            }
        }
        catch (Exception e)
        {
            GD.PrintErr("DeveloperConsole: Failed to add console entry: " + e);
            throw;
        }
    }

    private void AddEntryToList(string text)
    {
        RichTextLabel entry = new()
        {
            Text = text,
            FitContent = true,
            BbcodeEnabled = true
        };

        CallDeferred(MethodName.AddChildAndScrollToBottom, entry);
    }

    private void AddChildAndScrollToBottom(Node child)
    {
        consoleEntriesContainer.AddChild(child);
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
        Print($"] {text}");
        typedCommandHistory.Add(text);
        DeveloperConsoleCommandHandler.HandleCommand(text);
        consoleInput.Clear();
        FocusConsoleInput();
        historyIndex = 0;
    }

    private void ScrollToBottom()
    {
        if (consoleScrollContainer == null) return;

        // This timer is a bit of a hack but it was necessary becasue CallDeferred wasn't keeping up with the UI updates.
        GetTree().CreateTimer(0.01f).Timeout += () =>
        {
            consoleScrollContainer.ScrollVertical = (int)consoleScrollContainer.GetVScrollBar().MaxValue;
        };
    }

}

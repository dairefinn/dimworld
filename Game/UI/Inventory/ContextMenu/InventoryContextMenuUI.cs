namespace Dimworld.UI.Inventory;

using System;
using Dimworld.Core.Items;
using Godot;


public partial class InventoryContextMenuUI : Control, IContextMenuUI
{

    [Signal] public delegate void OnOptionSelectedEventHandler();

    private VBoxContainer MenuOptionsContainer;


    public override void _Ready()
    {
        base._Ready();

        MenuOptionsContainer = GetNode<VBoxContainer>("%MenuOptionsList");

        RemoveAllOptions();
    }

    private void RemoveAllOptions()
    {
        if (!IsInstanceValid(MenuOptionsContainer)) return;

        // Remove all testing menu options
        foreach (Node child in MenuOptionsContainer.GetChildren())
        {
            if (!IsInstanceValid(child)) continue;
            child.QueueFree();
        }
    }

    public void AddOption(string optionName, Action action)
    {
        Button optionButton = new()
        {
            Text = optionName
        };
        optionButton.Pressed += () => {
            action.Invoke();
            EmitSignal(SignalName.OnOptionSelected);
        };
        MenuOptionsContainer.AddChild(optionButton);
    }
    
    public void RemoveOption(string optionName)
    {
        foreach (Node child in MenuOptionsContainer.GetChildren())
        {
            if (child is Button button && button.Text == optionName)
            {
                button.QueueFree();
                return;
            }
        }
    }

    /// <summary>
    /// Shows the context menu at the given global position with the given options.
    /// </summary>
    /// <param name="globalPosition">The position to show the context menu at.</param>
    /// <param name="options">The options to show in the context menu.</param>
    public void Show(Vector2 globalPosition, ContextMenuOption[] options = null)
    {
        RemoveAllOptions();
        GlobalPosition = globalPosition;
        Show(options);
    }

    /// <summary>
    /// Shows the context menu with the given options.
    /// </summary>
    /// <param name="options">The options to show in the context menu.</param>
    public void Show(ContextMenuOption[] options = null)
    {
        if (options != null)
        {
            foreach (ContextMenuOption option in options)
            {
                AddOption(option.Label, option.Action);
            }
        }

        base.Show();
    }

    public new void Hide()
    {
        RemoveAllOptions();
        base.Hide();
    }

}

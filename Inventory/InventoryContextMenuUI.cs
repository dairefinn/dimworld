namespace Dimworld;

using System;
using Godot;


public partial class InventoryContextMenuUI : PanelContainer
{

    private VBoxContainer MenuOptionsContainer;


    public override void _Ready()
    {
        base._Ready();

        MenuOptionsContainer = GetNode<VBoxContainer>("%MenuOptionsList");

        RemoveAllOptions();
    }

    private void RemoveAllOptions()
    {
        // Remove all testing menu options
        foreach (Node child in MenuOptionsContainer.GetChildren())
        {
            child.QueueFree();
        }
    }

    public void AddOption(string optionName, Action action)
    {
        Button optionButton = new()
        {
            Text = optionName
        };
        optionButton.Pressed += action;
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

    public class ContextMenuOption(string label, Action action)
    {
        public string Label = label;
        public Action Action = action;
    }

}

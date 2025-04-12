namespace Dimworld.Core.Items;

using System;


/// <summary>
/// Represents an option in a context menu.
/// </summary>
public class ContextMenuOption
{

    public string Label;
    public Action Action;


    public ContextMenuOption(string label, Action action)
    {
        Label = label;
        Action = action;
    }

}

namespace Dimworld.Core.Items;

using System;


/// <summary>
/// Interface for the context menu UI. This is used to decouple the core from the game specific context menu UI.
/// </summary>
public interface IContextMenuUI
{

    public void AddOption(string optionName, Action action);

    public void RemoveOption(string optionName);

}

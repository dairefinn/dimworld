namespace Dimworld.Core.Modifiers;


/// <summary>
/// An interface that can be implemented by any class that is affected by modifiers.
/// </summary>
public interface IAffectedByModifiers
{

    public ModifierHandler ModifierHandler { get; set; }

}

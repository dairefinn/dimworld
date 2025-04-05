namespace Dimworld.Modifiers;

using Godot;


public abstract partial class Modifier : Resource
{

    /// <summary>
    /// The key of the modifier. This should be unique for each modifier. This ensures that modifiers don't conflict with each other.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// The type of processing for the modifier. It can be either Frame or Physics. This determines when the modifier will be processed in the node lifecycle.
    /// </summary>
    public ProcessingType ProcessOn { get; set; } = ProcessingType.Frame;

    /// <summary>
    /// The duration of the modifier. This is the time in seconds that the modifier will be active. If the duration is -1, it means that the modifier will be active indefinitely until removed.
    /// </summary>
    public float Duration { get; set; } = -1f;


    // CONSTRUCTORS AND BUILDERS

    public Modifier(string key)
    {
        Key = key;
    }

    public Modifier SetProcessOn(ProcessingType processOn)
    {
        ProcessOn = processOn;
        return this;
    }

    public Modifier SetDuration(float duration)
    {
        Duration = duration;
        return this;
    }


    // PROCESSING

    /// <summary>
    /// Processes the modifier. This is called in the node lifecycle. The delta parameter is the time since the last frame.
    /// /// This method should be overridden in the derived classes to implement the specific behavior of the modifier.
    /// </summary>
    /// <param name="parent">The parent node of the modifier. This is the node that the modifier is attached to. e.g. a character or an object.</param>
    /// <param name="delta">The time since the last frame.</param>
    /// <returns>>True if the modifier is still active, false if it has expired.</returns>
    public virtual bool Process(double delta)
    {
        return UpdateDuration(delta);
    }

    private bool UpdateDuration(double delta)
    {
        if (Duration == -1) return true; // Infinite duration

        Duration -= (float)delta;

        return Duration > 0; // Return true if the modifier is still active, false if it has expired.
    }


    public enum ProcessingType
    {
        Frame,
        Physics
    }

}

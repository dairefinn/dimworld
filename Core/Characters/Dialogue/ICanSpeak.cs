namespace Dimworld.Core.Characters.Dialogue;


/// <summary>
/// Interface for entities that can create speech bubbles.
/// </summary>
public interface ICanSpeak
{

    public ISpeechBubble SpeechBubble { get; set; }

}

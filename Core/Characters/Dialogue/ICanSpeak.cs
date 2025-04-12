namespace Dimworld.Core.Characters.Dialogue;


/// <summary>
/// Interface for entities that can create speech bubbles.
/// </summary>
public interface ICanSpeak
{

    public SpeechHandler SpeechHandler { get; set; }

    public void Say(string message);

}

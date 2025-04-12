namespace Dimworld.Core.Characters.Dialogue;

using Godot;


public partial class SpeechHandler : Node
{

    [Export(PropertyHint.File, "*.tscn")] public PackedScene SpeechBubbleScene;


    public void Say(string message)
    {
        if (SpeechBubbleScene == null)
        {
            GD.PrintErr("SpeechBubbleScene is not set. Please assign a scene to the SpeechHandler.");
            return;
        }

        ISpeechBubble speechBubble = SpeechBubbleScene.Instantiate<ISpeechBubble>();
        if (speechBubble == null)
        {
            GD.PrintErr("Failed to instantiate speech bubble. Please check the scene.");
            return;
        }

        AddChild(speechBubble as Node); // Add the speech bubble to the scene tree
        speechBubble.Text = message; // Call the Say method on the speech bubble
    }
    

}
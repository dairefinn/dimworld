namespace Dimworld.Core.Characters.Dialogue;

using Godot;


[GlobalClass]
public partial class SpeechHandler : Resource
{

    [Export] public NodePath SpeechBubblePath { get; set; }


    private Node2D _parent;


    public void Initalize(Node2D parent)
    {
        _parent = parent;
    }


    // public void Say(string message)
    // {
    //     if (SpeechBubbleScene == null)
    //     {
    //         GD.PrintErr("SpeechBubbleScene is not set. Please assign a scene to the SpeechHandler.");
    //         return;
    //     }

    //     ISpeechBubble speechBubble = SpeechBubbleScene.Instantiate<ISpeechBubble>();
    //     if (speechBubble == null)
    //     {
    //         GD.PrintErr("Failed to instantiate speech bubble. Please check the scene.");
    //         return;
    //     }

    //     _parent.AddChild(speechBubble as Node2D);
    //     (speechBubble as Node2D).GlobalPosition = _parent.GlobalPosition;
        
    //     speechBubble.Text = message;
    // }
    
    public void Say(string message)
    {
        if (SpeechBubblePath == null)
        {
            GD.PrintErr("SpeechBubblePath is not set. Please assign a path to the SpeechHandler.");
            return;
        }

        Control speechBubble = _parent.GetNode<Control>(SpeechBubblePath);
        if (speechBubble == null)
        {
            GD.PrintErr("Failed to find speech bubble. Please check the path.");
            return;
        }

        if (speechBubble is not ISpeechBubble iSpeechBubble)
        {
            GD.PrintErr("The node at the specified path is not a speech bubble. Please check the path.");
            return;
        }

        iSpeechBubble.Text = message;
    }

}
namespace Dimworld.Core.Characters.Dialogue.Menus;

using Dimworld.Core.Characters.Dialogue.Options;
using Godot;
using Godot.Collections;


/// <summary>
/// Base class for dialogue menus.
/// These contain a number of messages and dialogue options.
/// </summary>
[GlobalClass]
public abstract partial class DialogueMenu : Resource
{

    [Export] public Array<string> Messages { get; set; } = [];
    [Export] public Array<DialogueOption> Options { get; set; } = [];


    public string Message {
        get {
            if (_message == null)
            {
                _message = GetRandomMessage();
            }

            return _message;
        }
        set {
            _message = value;
        }
    }
    private string _message = null;


	private string GetRandomMessage()
    {
        if (Messages.Count == 0) return "No messages";
        if (Messages.Count == 1) return Messages[0];
        return Messages[GD.RandRange(0, Messages.Count)];
    }

    
    /// <summary>
    /// Shuffle the message to a random one from the list of messages.
    /// </summary>
    public void ShuffleMessage()
    {
        Message = GetRandomMessage();
    }

}

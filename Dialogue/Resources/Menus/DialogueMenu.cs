namespace Dimworld.Dialogue;

using Dimworld.Dialogue.Options;
using Godot;
using Godot.Collections;


[GlobalClass]
public abstract partial class DialogueMenu : RefCounted
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

	public string GetRandomMessage()
    {
        if (Messages.Count == 0) return "No messages";
        if (Messages.Count == 1) return Messages[0];
        return Messages[GD.RandRange(0, Messages.Count)];
    }

    public void ShuffleMessage()
    {
        Message = GetRandomMessage();
    }

}

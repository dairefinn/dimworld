namespace Dimworld.Dialogue;

using Dimworld.Developer;
using Dimworld.UI;
using Godot;


public partial class DialogueHandler : Node {

    public static DialogueHandler Instance {get; private set;}

    public DialogueMenu CurrentDialogue {
        get => _currentDialogue;
        set {
            _currentDialogue = value;
            if (value != null) {
                UIRootPanel.Instance.SetActivePanel<DialoguePanelUI>();
            }
            else
            {
                UIRootPanel.Instance.DisablePanel<DialoguePanelUI>();
            }
        }
    }
    private DialogueMenu _currentDialogue;

    public override void _Ready()
    {
        Instance = this;
    }

	public override void _Process(double delta)
	{
		base._Process(delta);

        if(Input.IsActionJustPressed(InputActions.UI_CANCEL)) {
            OnEscapePressed();
        }
	}

    private void OnEscapePressed() {
        if(CurrentDialogue == null) return;

        EndDialogue();
    }

	public void StartDialogue(DialogueMenu data) {
        DeveloperConsole.PrintInfo($"Starting dialogue: {data}");
        if(data == null) return;
        CurrentDialogue = data;
    }

    public void EndDialogue(){
        CurrentDialogue = null;
        TradeController.Instance.EndTrade();
    }

}
namespace Dimworld.Dialogue;

using Dimworld.Developer;
using Dimworld.Dialogue.Options;
using Dimworld.UI;
using Godot;
using Godot.Collections;

public partial class DialoguePanelUI : Control
{

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
            UpdateUI();
        }
    }
    private DialogueMenu _currentDialogue;
    [Export] public RichTextLabel DialogueTextLabel { get; set; }
    [Export] public Container DialogueOptionsContainer { get; set; }


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
        // Globals.Instance.TradeController.EndTrade(); // TODO: Implement this
    }

    public void UpdateUI()
    {
        string message = string.Empty;
        Array<DialogueOption> options = [];

        if (CurrentDialogue != null)
        {
            message = CurrentDialogue.GetRandomMessage();
            options = CurrentDialogue.Options;
        }

        DialogueTextLabel.Text = message;
        
        foreach(Node child in DialogueOptionsContainer.GetChildren())
        {
            child.QueueFree();
        }

        foreach (DialogueOption option in options)
        {
            Button label = new()
            {
                Text = option.Name
            };
            label.Pressed += () =>
            {
                option.OnSelected();
            };
            DialogueOptionsContainer.AddChild(label);
        }
        
        if (CurrentDialogue == null)
        {
            Hide();
        }
    }

}

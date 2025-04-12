namespace Dimworld.UI.Dialogue;

using Dimworld.Core.Characters.Dialogue;
using Godot;


[Tool] // Enables the script to run in the editor
public partial class SpeechBubble : Control, ISpeechBubble
{
    [Export] public string Text
    {
        get => _text;
        set {
            _text = value;
            OnTextUpdated(value);
        }
    }
    private string _text;

    [Export] public Label Label { get; set; }

    /// <summary>
    /// The timer that controls how long the speech bubble is visible. If left empty, the speech bubble will be visible indefinitely as long as the text is not empty.
    /// </summary>
    [Export] public Timer HideTimer { get; set; }


    public override void _Ready()
    {
        base._Ready();

        if (HideTimer == null)
        {
            HideTimer = GetNode<Timer>("HideTimer");
        }
        
        if (HideTimer != null)
        {
            HideTimer.Timeout += OnTimerTimeout;
        }
    }


    private void OnTextUpdated(string text)
    {
        if (Label != null)
        {
            Label.Text = text;

            if (text == null || text.Length > 0)
            {
                Show();
                StartHideTimer();
            }
            else
            {
                Hide();
            }
        }
    }

    private void StartHideTimer()
    {
        if (!IsInstanceValid(HideTimer) || !HideTimer.IsInsideTree()) return; // Ensure Timer is valid and in the scene tree
        HideTimer?.Start();
    }

    private void OnTimerTimeout()
    {
        if (Engine.IsEditorHint()) return; // Don't hide the speech bubble in the editor
        Hide();
    }

}

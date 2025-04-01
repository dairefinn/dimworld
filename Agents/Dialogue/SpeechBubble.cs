namespace Dimworld;

using Godot;
using System;


[Tool] // Enables the script to run in the editor
public partial class SpeechBubble : Control
{
    [Export] public string Text
    {
        get => _text;
        set => SetText(value);
    }
    private string _text;

    [Export] public Label Label { get; set; }


    private void SetText(string text)
    {
        GD.Print($"Setting text to: {text}");
        _text = text;

        if (Label != null)
        {
            Label.Text = _text;

            if (_text == null || _text.Length > 0)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }
    }

}
namespace Dimworld.Helpers.BBCode;

using System;
using System.Text;
using Godot;


public class BBCodeBuilder
{

    private readonly StringBuilder value = new();


    // Basic unformatted text

    public BBCodeBuilder Text(string text)
    {
        value.Append(text);
        return this;
    }

    public BBCodeBuilder Text(string text, params object[] args)
    {
        value.Append(string.Format(text, args));
        return this;
    }


    // Formatted text

    public BBCodeBuilder Bold(string text)
    {
        value.Append(BBCodeHelper.Formatting.Bold(text));
        return this;
    }

    public BBCodeBuilder Italic(string text)
    {
        value.Append(BBCodeHelper.Formatting.Italic(text));
        return this;
    }

    public BBCodeBuilder UnderLine(string text)
    {
        value.Append(BBCodeHelper.Formatting.Underline(text));
        return this;
    }

    public BBCodeBuilder Strikethrough(string text)
    {
        value.Append(BBCodeHelper.Formatting.Strikethrough(text));
        return this;
    }

    public BBCodeBuilder NewLine()
    {
        value.Append(BBCodeHelper.Formatting.NewLine());
        return this;
    }


    // Colored text

    public BBCodeBuilder Red(string text)
    {
        value.Append(BBCodeHelper.Colors.Red(text));
        return this;
    }

    public BBCodeBuilder Green(string text)
    {
        value.Append(BBCodeHelper.Colors.Green(text));
        return this;
    }

    public BBCodeBuilder Blue(string text)
    {
        value.Append(BBCodeHelper.Colors.Blue(text));
        return this;
    }

    public BBCodeBuilder Yellow(string text)
    {
        value.Append(BBCodeHelper.Colors.Yellow(text));
        return this;
    }

    public BBCodeBuilder Pink(string text)
    {
        value.Append(BBCodeHelper.Colors.Pink(text));
        return this;
    }

    public BBCodeBuilder Cyan(string text)
    {
        value.Append(BBCodeHelper.Colors.Cyan(text));
        return this;
    }

    public BBCodeBuilder Color(string text, Color color)
    {
        value.Append(BBCodeHelper.Colors.From(text, color));
        return this;
    }

    public BBCodeBuilder Color(string text, string colorHex)
    {
        if (!System.Text.RegularExpressions.Regex.IsMatch(colorHex, "^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$"))
        {
            throw new ArgumentException("Invalid color hex code.", nameof(colorHex));
        }

        value.Append(BBCodeHelper.Colors.From(text, colorHex));
        return this;
    }


    // Building the final string

    public BBCodeBuilder NestedFormat(string text, Func<BBCodeBuilder, BBCodeBuilder> format)
    {
        var nestedBuilder = new BBCodeBuilder();
        format(nestedBuilder);
        value.Append(nestedBuilder.Build());
        return this;
    }

    public string Build()
    {
        return value.ToString();
    }

}

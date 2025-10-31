namespace DaireFinn.Plugins.DeveloperConsole.Utils;

using Godot;


/// <summary>
/// Helper class for BBCode formatting.
/// </summary>
public static class BBCodeHelper
{


    public static class Colors
    {

        // PRIMARY COLORS

        public static string Red(string text)
        {
            return $"[color=#ff5555]{text}[/color]";
        }

        public static string Green(string text)
        {
            return $"[color=#55ff55]{text}[/color]";
        }

        public static string Blue(string text)
        {
            return $"[color=#7777ff]{text}[/color]";
        }

        // SECONDARY COLORS

        public static string Yellow(string text)
        {
            return $"[color=#ffff55]{text}[/color]";
        }

        public static string Pink(string text)
        {
            return $"[color=#ff55ff]{text}[/color]";
        }

        public static string Cyan(string text)
        {
            return $"[color=#55ffff]{text}[/color]";
        }

        // OTHER

        public static string From(string text, Color color)
        {
            string colorHex = color.ToHtml();
            return $"[color=#{colorHex}]{text}[/color]";
        }

        public static string From(string text, string colorHex)
        {
            return $"[color=#{colorHex}]{text}[/color]";
        }

    }

    // FORMATTING

    public static class Formatting
    {

        public static string Bold(string text)
        {
            return $"[b]{text}[/b]";
        }

        public static string Italic(string text)
        {
            return $"[i]{text}[/i]";
        }

        public static string Underline(string text)
        {
            return $"[u]{text}[/u]";
        }

        public static string Strikethrough(string text)
        {
            return $"[s]{text}[/s]";
        }

        public static string NewLine()
        {
            return "\n";
        }

    }

}

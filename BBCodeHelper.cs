namespace Dimworld.Helpers;

using Godot;


public class BBCodeHelper
{

    public class Colors
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
            GD.Print($"Color: {colorHex}");
            return $"[color=#{colorHex}]{text}[/color]";
        }

    }

}

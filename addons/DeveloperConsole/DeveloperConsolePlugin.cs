#if TOOLS
namespace DaireFinn.Plugins.DeveloperConsole;

using Godot;

[Tool]
public partial class DeveloperConsolePlugin : EditorPlugin
{
    private const string AutoloadName = "DeveloperConsole";
    private const string ConsolePath = "res://addons/DeveloperConsole/DeveloperConsole.cs";
    private const string SettingPrintToDebugConsole = "developer_console/print_to_debug_console";

    public override void _EnterTree()
    {
        // Add the Developer Console as an autoload singleton
        AddAutoloadSingleton(AutoloadName, ConsolePath);

        // Add project settings for the Developer Console
        AddProjectSetting(SettingPrintToDebugConsole, false, 
            "If enabled, console messages will also be printed to the editor's debug console.");
    }

    public override void _ExitTree()
    {
        // Remove the Developer Console autoload when plugin is disabled
        RemoveAutoloadSingleton(AutoloadName);
    }

    private void AddProjectSetting(string name, Variant defaultValue, string hint)
    {
        if (!ProjectSettings.HasSetting(name))
        {
            ProjectSettings.SetSetting(name, defaultValue);
        }

        var propertyInfo = new Godot.Collections.Dictionary
        {
            { "name", name },
            { "type", (int)defaultValue.VariantType },
            { "hint_string", hint }
        };

        ProjectSettings.AddPropertyInfo(propertyInfo);
        ProjectSettings.SetInitialValue(name, defaultValue);
        ProjectSettings.Save();
    }
}
#endif
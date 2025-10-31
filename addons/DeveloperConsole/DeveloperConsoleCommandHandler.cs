namespace DaireFinn.Plugins.DeveloperConsole;

using System;
using System.Collections.Generic;
using Godot;
using DaireFinn.Plugins.DeveloperConsole.Utils;


/// <summary>
/// Processes commands entered in the developer console.
/// Each command is associated with a specific method that handles its execution.
/// There is a brief description for each command, which is displayed when the user requests help.
/// 
/// To add custom commands in your game, call RegisterCommand() with your command name, handler, and description.
/// </summary>
public class DeveloperConsoleCommandHandler
{

    private class CommandInfo(Action<string[]> action, string description)
    {
        public Action<string[]> Action { get; set; } = action;
        public string Description { get; set; } = description;
    }

    private static readonly Dictionary<string, CommandInfo> Commands = new()
    {
        {
            "timescale", new CommandInfo(
                CommandTimeScale,
                $"Arguments: {BBCodeHelper.Formatting.Underline("<value>")}. Sets the time scale of the game."
            )
        },
        {
            "clear", new CommandInfo(
                CommandClearConsole,
                "Clears the console."
            )
        },
        {
            "quit", new CommandInfo(
                CommandQuitGame,
                "Quits the application."
            )
        },
        {
            "help", new CommandInfo(
                CommandHelp,
                "Lists all available commands."
            )
        }
    };

    /// <summary>
    /// Registers a custom command that can be executed from the developer console.
    /// </summary>
    /// <param name="commandName">The name of the command (case-insensitive).</param>
    /// <param name="handler">The action to execute when the command is invoked. Receives command arguments.</param>
    /// <param name="description">A description of what the command does, shown in the help menu.</param>
    public static void RegisterCommand(string commandName, Action<string[]> handler, string description)
    {
        string lowerCommandName = commandName.ToLower();

        if (Commands.ContainsKey(lowerCommandName))
        {
            DeveloperConsole.PrintWarning($"Command '{commandName}' is already registered. Overwriting.");
        }

        Commands[lowerCommandName] = new CommandInfo(handler, description);
    }

    /// <summary>
    /// Unregisters a custom command from the developer console.
    /// </summary>
    /// <param name="commandName">The name of the command to unregister.</param>
    /// <returns>True if the command was unregistered, false if it didn't exist.</returns>
    public static bool UnregisterCommand(string commandName)
    {
        return Commands.Remove(commandName.ToLower());
    }


    public static void HandleCommand(string command)
    {
        string[] commandParts = command.Split(' ');
        string commandName = commandParts[0].ToLower();
        string[] commandArgs = commandParts.Length > 1 ? commandParts[1..] : [];

        if (!Commands.ContainsKey(commandName))
        {
            DeveloperConsole.PrintErr($"Unknown command: {commandName}");
            return;
        }

        try
        {
            Commands[commandName].Action.Invoke(commandArgs);
        }
        catch (Exception ex)
        {
            DeveloperConsole.PrintErr($"Error executing command '{commandName}': {ex.Message}");
        }
    }


    // BUILT-IN COMMAND HANDLERS

    private static void CommandTimeScale(string[] args)
    {
        if (args.Length == 0)
        {
            DeveloperConsole.PrintInfo($"Current time scale: {Engine.TimeScale}");
            return;
        }

        if (args.Length == 1 && float.TryParse(args[0], out float timeScale))
        {
            if (timeScale < 0.0f)
            {
                DeveloperConsole.PrintErr("Time scale cannot be negative.");
                return;
            }

            Engine.TimeScale = timeScale;
            DeveloperConsole.PrintSuccess($"Time scale set to {timeScale}");
        }
        else
        {
            DeveloperConsole.PrintInfo($"Usage: timescale {BBCodeHelper.Formatting.Underline("<value>")}\nExample: timescale 0.5");
        }
    }

    private static void CommandClearConsole(string[] args)
    {
        DeveloperConsole.Clear();
    }

    private static void CommandQuitGame(string[] args)
    {
        if (Engine.GetMainLoop() is SceneTree tree)
        {
            DeveloperConsole.PrintInfo("Quitting application...");
            tree.Quit();
        }
        else
        {
            DeveloperConsole.PrintErr("Unable to access the SceneTree to quit the application.");
        }
    }

    private static void CommandHelp(string[] args)
    {
        DeveloperConsole.PrintInfo("Available commands:");

        // Sort commands alphabetically for better readability
        var sortedCommands = new List<KeyValuePair<string, CommandInfo>>(Commands);
        sortedCommands.Sort((a, b) => string.Compare(a.Key, b.Key, StringComparison.Ordinal));

        foreach (KeyValuePair<string, CommandInfo> command in sortedCommands)
        {
            DeveloperConsole.PrintInfo($"  {BBCodeHelper.Formatting.Bold(command.Key)}: {command.Value.Description}");
        }
    }

}

namespace Dimworld.Developer;

using System;
using System.Collections.Generic;
using Godot;


public class DeveloperConsoleCommandHandler
{

    private class CommandInfo(Action<string[]> action, string description)
    {
        public Action<string[]> Action { get; set; } = action;
        public string Description { get; set; } = description;
    }

    private static readonly Dictionary<string, CommandInfo> Commands = new()
    {
        { "timescale", new CommandInfo(CommandTimeScale, "Arguments: [u]<value>[/u]. Sets the time scale of the game.") },
        { "help", new CommandInfo(CommandHelp, "Lists all available commands.") },
    };

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

    private static void CommandTimeScale(string[] args)
    {
        if (args.Length == 1 && float.TryParse(args[0], out float timeScale))
        {
            if (timeScale < 0.0f)
            {
                DeveloperConsole.PrintErr("DeveloperConsole: Time scale cannot be negative.");
                return;
            }

            Engine.TimeScale = timeScale;
        }
        else
        {
            DeveloperConsole.PrintInfo("Usage: timescale <value>");
        }
    }

    private static void CommandHelp(string[] args)
    {
        DeveloperConsole.PrintInfo("Available commands:");
        foreach (KeyValuePair<string, CommandInfo> command in Commands)
        {
            DeveloperConsole.PrintInfo($"\t⦁\t{command.Key}: {command.Value.Description}");
        }
    }

}

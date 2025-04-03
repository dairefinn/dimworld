namespace Dimworld.Developer;

using System;
using System.Collections.Generic;
using Godot;


public class DeveloperConsoleCommandHandler
{

    private static readonly Dictionary<string, Action<string[]>> Commands = new()
    {
        { "timescale", CommandTimeScale },
        { "help", CommandHelp },
    };

    public static void HandleCommand(string command)
    {
        string[] commandParts = command.Split(' ');
        string commandName = commandParts[0].ToLower();
        string[] commandArgs = commandParts.Length > 1 ? commandParts[1..] : [];

        if (!Commands.ContainsKey(commandName))
        {
            DeveloperConsole.Print($"Unknown command: {commandName}");
            return;
        }

        try
        {
            Commands[commandName].Invoke(commandArgs);
        }
        catch (Exception ex)
        {
            DeveloperConsole.Print($"Error executing command '{commandName}': {ex.Message}");
        }
    }

    private static void CommandTimeScale(string[] args)
    {
        if (args.Length == 1 && float.TryParse(args[0], out float timeScale))
        {
            if (timeScale < 0.0f)
            {
                DeveloperConsole.Print("DeveloperConsole: Time scale cannot be negative.");
                return;
            }

            Engine.TimeScale = timeScale;
        }
        else
        {
            DeveloperConsole.Print("Usage: timescale <value>");
        }
    }

    private static void CommandHelp(string[] args)
    {
        Color commandColour = Colors.Yellow;

        DeveloperConsole.Print("Available commands:", commandColour);
        foreach (var command in Commands.Keys)
        {
            DeveloperConsole.Print($"- {command}", commandColour);
            // Get the parameters of the command
            DeveloperConsole.Print($"  Parameters: {string.Join(", ", command)}", commandColour);
        }
    }

}

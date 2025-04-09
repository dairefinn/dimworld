namespace Dimworld.Developer;

using System;
using System.Collections.Generic;
using Dimworld.Agents.Instances;
using Dimworld.Helpers;
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
        {
            "timescale", new CommandInfo(
                CommandTimeScale,
                $"Arguments: {BBCodeHelper.Formatting.Underline("<value>")}. Sets the time scale of the game."
            )
        },
        {
            "teleport", new CommandInfo(
                CommandTeleport,
                $"Arguments: {BBCodeHelper.Formatting.Underline("<x> <y>")}. Teleports the player to the specified coordinates."
            )
        },
        {
            "changelevel", new CommandInfo(
                CommandChangeLevel,
                $"Arguments: {BBCodeHelper.Formatting.Underline("<level_name>")}. Changes the current level to the specified one."
            )
        },
        {
            "clear", new CommandInfo(
                CommandClearConsole,
                "Clears the console."
            )
        },
        {
            "exit", new CommandInfo(
                CommandExitGame,
                "Exits the game."
            )
        },
        {
            "help", new CommandInfo(
                CommandHelp,
                "Lists all available commands."
            )
        }
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


    // COMMAND HANDLERS

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

    private static void CommandTeleport(string[] args)
    {
        if (args.Length == 2 && float.TryParse(args[0], out float x) && float.TryParse(args[1], out float y))
        {
            Player player = Globals.Instance.Player;
            player.GlobalPosition = new Vector2(x, y);
        }
        else
        {
            DeveloperConsole.PrintInfo($"Usage: teleport {BBCodeHelper.Formatting.Underline("<x>")} {BBCodeHelper.Formatting.Underline("<y>")}");
        }
    }

    private static void CommandChangeLevel(string[] args)
    {
        if (args.Length == 1)
        {
            string levelName = args[0];
            string fullLevelPath = $"res://Levels/{levelName}.tscn";
            Globals.Instance.LevelHandler.ChangeLevel(fullLevelPath);
        }
        else
        {
            DeveloperConsole.PrintInfo($"Usage: changelevel {BBCodeHelper.Formatting.Underline("<level_name>")}");
        }
    }

    private static void CommandClearConsole(string[] args)
    {
        DeveloperConsole.Clear();
    }

    private static void CommandExitGame(string[] args)
    {
        if (Engine.GetMainLoop() is SceneTree tree)
        {
            tree.Quit();
        }
        else
        {
            DeveloperConsole.PrintErr("Unable to access the SceneTree to quit the game.");
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

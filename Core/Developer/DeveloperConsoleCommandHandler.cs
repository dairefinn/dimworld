namespace Dimworld.Core.Developer;

using System;
using Dimworld.Characters;
using Dimworld.Core.Utils;
using Dimworld.Core.Developer;
using Godot;
using CommandHandler = DaireFinn.Plugins.DeveloperConsole.DeveloperConsoleCommandHandler;

/// <summary>
/// Registers game-specific commands for the developer console.
/// </summary>
public static class DeveloperConsoleCommandHandler
{
    /// <summary>
    /// Registers all game-specific commands with the developer console.
    /// Should be called during initialization (e.g., in Globals._Ready()).
    /// </summary>
    public static void RegisterCommands()
    {
        CommandHandler.RegisterCommand(
            "teleport",
            CommandTeleport,
            $"Arguments: {BBCodeHelper.Formatting.Underline("<x> <y>")}. Teleports the player to the specified coordinates."
        );

        CommandHandler.RegisterCommand(
            "changelevel",
            CommandChangeLevel,
            $"Arguments: {BBCodeHelper.Formatting.Underline("<level_name>")}. Changes the current level to the specified one."
        );

        CommandHandler.RegisterCommand(
            "exit",
            CommandExitGame,
            "Exits the game."
        );
    }

    // COMMAND HANDLERS

    private static void CommandTeleport(string[] args)
    {
        if (args.Length == 2 && float.TryParse(args[0], out float x) && float.TryParse(args[1], out float y))
        {
            Player player = Globals.Instance.Player;
            if (player == null)
            {
                DeveloperConsole.PrintErr("Player is not available.");
                return;
            }

            player.GlobalPosition = new Vector2(x, y);
            DeveloperConsole.PrintSuccess($"Teleported to ({x}, {y})");
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
            string fullLevelPath = $"res://Game/Levels/{levelName}.tscn";

            if (Globals.Instance.LevelHandler == null)
            {
                DeveloperConsole.PrintErr("LevelHandler is not available.");
                return;
            }

            Globals.Instance.LevelHandler.ChangeLevel(fullLevelPath);
            DeveloperConsole.PrintSuccess($"Changing level to: {levelName}");
        }
        else
        {
            DeveloperConsole.PrintInfo($"Usage: changelevel {BBCodeHelper.Formatting.Underline("<level_name>")}");
        }
    }

    private static void CommandExitGame(string[] args)
    {
        if (Engine.GetMainLoop() is SceneTree tree)
        {
            DeveloperConsole.PrintInfo("Exiting game...");
            tree.Quit();
        }
        else
        {
            DeveloperConsole.PrintErr("Unable to access the SceneTree to quit the game.");
        }
    }
}

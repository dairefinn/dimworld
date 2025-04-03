namespace Dimworld.Developer;

using Godot;


public class DeveloperConsoleCommandHandler
{

    public static void HandleCommand(string command)
    {
        string[] commandParts = command.Split(' ');
        string commandName = commandParts[0].ToLower();
        string[] commandArgs = commandParts.Length > 1 ? commandParts[1..] : [];

        switch (commandName)
        {
            case "timescale":
                CommandTimeScale(commandArgs);
                break;
            default:
                DeveloperConsole.Print($"Unknown command: {commandName}");
                break;
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

}
        
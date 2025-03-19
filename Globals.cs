namespace Dimworld;


public class Globals
{

    public static Globals Instance { get; private set; }

    public static Globals GetInstance()
    {
        if (Instance == null)
        {
            Instance = new();
        }

        return Instance;
    }


    public PlayerController MainPlayer { get; set; }

}
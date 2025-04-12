namespace Dimworld.Factions;

using Dimworld.Core.Factions;
using Godot;


public static class FactionDefaults
{

    public static void Initialize()
    {
        Faction factionTest = ResourceLoader.Load<Faction>("res://Game/Factions/Test.tres");
        Faction factionTest2 = ResourceLoader.Load<Faction>("res://Game/Factions/Test2.tres");
        factionTest.SetAffinity(factionTest2, Affinity.Friendly);
    }

}

namespace Dimworld.Factions;

using Dimworld.Core.Factions;
using Godot;


public static class FactionDefaults
{

    public static void Initialize()
    {
        Faction factionTest = ResourceLoader.Load<Faction>("res://Game/Factions/Test.tres");
        Faction factionEnemy = ResourceLoader.Load<Faction>("res://Game/Factions/TestEnemy.tres");
        Faction factionFriendly = ResourceLoader.Load<Faction>("res://Game/Factions/TestFriendly.tres");
        factionTest.SetAffinity(factionEnemy, Affinity.Hostile);
    }

}

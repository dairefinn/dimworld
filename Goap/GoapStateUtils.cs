namespace Dimworld;

using Godot;
using Godot.Collections;

public class GoapStateUtils
{
    public static bool IsSubsetOf(Dictionary<string, Variant> stateA, Dictionary<string, Variant> stateB)
    {
        foreach (string key in stateA.Keys)
        {
            if (!stateB.TryGetValue(key, out Variant value) || !value.Equals(stateA[key]))
            {
                return false;
            }
        }

        return true;
    }

    public static Dictionary<string, Variant> Add(Dictionary<string, Variant> stateA, Dictionary<string, Variant> stateB)
    {
        foreach (string key in stateB.Keys)
        {
            stateA[key] = stateB[key];
        }

        return stateA;
    }

    public static Dictionary<string, Variant> Duplicate(Dictionary<string, Variant> state)
    {
        Dictionary<string, Variant> newState = [];
        foreach (string key in state.Keys)
        {
            newState[key] = state[key];
        }

        return newState;
    }

    public static Variant GetState(Dictionary<string, Variant> state, string key, Variant defaultValue)
    {
        if (!state.ContainsKey(key))
        {
            return defaultValue;
        }

        return state[key];
    }

    public static Dictionary<string, Variant> SetState(Dictionary<string, Variant> state, string key, Variant value)
    {
        state[key] = value;
        return state;
    }

    public static void PrintState(Dictionary<string, Variant> state, string label = "State")
    {
        GD.Print(label + ": {");
        foreach (string key in state.Keys)
        {
            GD.Print(key + ": " + state[key]);
        }
        GD.Print("}");
    }

}
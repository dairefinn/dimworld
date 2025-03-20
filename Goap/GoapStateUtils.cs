namespace Dimworld;

using Godot;
using Godot.Collections;

public class GoapStateUtils
{

    // TODO: I had { }"is_patrolling": false } as a precondition for the patrol house action but it was breaking because the agent didn't have the property in their state at all. Should I add a way to make "false" properties the same as missing ones?
    public static bool IsSubsetOf(Dictionary<string, Variant> subset, Dictionary<string, Variant> mainState)
    {
        foreach (string key in subset.Keys)
        {
            if (!mainState.ContainsKey(key)) {
                return false; // Key must exist in main state
            }

            // Check if the value can be converted to an array
            Array<string> subsetArray = subset[key].As<Array<string>>();
            Array<string> mainStateArray = mainState[key].As<Array<string>>();

            if (subsetArray != null && mainStateArray != null)
            {
                // The main array must contain all the elements of the subset array but doesn't need to be exactly the same
                foreach (string element in subsetArray)
                {
                    if (!mainStateArray.Contains(element)) return false;
                }

                continue;
            }

            if (!mainState[key].Equals(subset[key])) return false; // Key must have the same value
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
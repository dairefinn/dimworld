namespace Dimworld;

using Godot;
using Godot.Collections;

public class GoapStateUtils
{

    public static bool IsSubsetOf(Dictionary<string, Variant> subset, Dictionary<string, Variant> mainState)
    {
        if (mainState == null) return false;
        if (subset == null) return true;

        foreach (string key in subset.Keys)
        {
            if (!mainState.ContainsKey(key)) {
                return false; // Key must exist in main state
            }

            
            Variant.Type mainStateType = mainState[key].VariantType;
            Variant.Type subsetType = subset[key].VariantType;

            if (mainStateType != subsetType) return false; // Types must match

            switch(mainStateType)
            {
                case Variant.Type.Bool:
                    if (mainState[key].AsBool() != subset[key].AsBool()) return false;
                    break;
                case Variant.Type.Array:
                    // Check if the value can be converted to an array
                    Array<string> subsetArray = subset[key].AsGodotArray<string>();
                    Array<string> mainStateArray = mainState[key].AsGodotArray<string>();

                    if (subsetArray != null && mainStateArray != null)
                    {
                        // The main array must contain all the elements of the subset array but doesn't need to be exactly the same
                        foreach (string element in subsetArray)
                        {
                            if (!mainStateArray.Contains(element)) return false;
                        }

                        continue;
                    }
                    break;
                default:
                    if (!mainState[key].Equals(subset[key])) return false; // Key must have the same value
                    return true;
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
        if (state == null) return defaultValue;

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

    public static string GetAsString(Dictionary<string, Variant> state, string label = "State")
    {
        string output = label + ": { ";

        foreach (string key in state.Keys)
        {
            output += key + ": " + state[key] + ", ";
        }
        output += "}";

        return output;
    }

}
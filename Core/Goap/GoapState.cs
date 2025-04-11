namespace Dimworld.GOAP;

using Godot;
using Godot.Collections;


[GlobalClass]
public partial class GoapState : Resource
{

    public static GoapState Empty => new([]);


    [Export] public Dictionary<string, Variant> State { get; set; }


    public GoapState()
    {
        State = [];
    }

    public GoapState(Dictionary<string, Variant> state)
    {
        State = state;
    }


    public void RemoveKey(string key)
    {
        State.Remove(key);
    }

    public bool ContainsKey(string key)
    {
        return State.ContainsKey(key);
    }

    public Variant GetKey(string key)
    {
        return State[key];
    }

    public void SetKey(string key, Variant value)
    {
        State[key] = value;
    }


    public bool IsSubsetOf(GoapState mainState)
    {
        if (mainState == null) return false;

        Dictionary<string, Variant> mainStateRaw = mainState.State;

        foreach (string key in State.Keys)
        {
            if (!mainState.ContainsKey(key)) {
                return false; // Key must exist in main state
            }

            
            Variant.Type mainStateType = mainStateRaw[key].VariantType;
            Variant.Type subsetType = State[key].VariantType;

            if (mainStateType != subsetType) return false; // Types must match

            switch(mainStateType)
            {
                case Variant.Type.Bool:
                    if (mainStateRaw[key].AsBool() != State[key].AsBool()) return false;
                    break;
                case Variant.Type.Array:
                    // Check if the value can be converted to an array
                    Array<string> subsetArray = State[key].AsGodotArray<string>();
                    Array<string> mainStateArray = mainStateRaw[key].AsGodotArray<string>();

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
                case Variant.Type.String:
                    if (mainStateRaw[key].AsString() != State[key].AsString()) return false; // Key must have the same value
                    break;
                case Variant.Type.NodePath:
                    if (mainStateRaw[key].AsNodePath() != State[key].AsNodePath()) return false; // Key must have the same value
                    break;
                default:
                    if (!mainStateRaw[key].Equals(State[key])) return false; // Key must have the same value
                    return true;
            }
            
        }

        return true;
    }

    public override string ToString()
    {
        string output = "{ ";

        foreach (string key in State.Keys)
        {
            output += key + ": " + State[key] + ", ";
        }

        output += "}";

        return output;
    }

}
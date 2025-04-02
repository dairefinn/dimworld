namespace Dimworld.GOAP.Actions;

using Godot;
using Godot.Collections;
using System.Linq;

public partial class Idle : GoapAction
{

    public override GoapState GetEffects()
    {
        return new GoapState(new Dictionary<string, Variant> {
            {"current_action", "idle"}
        });
    }

}

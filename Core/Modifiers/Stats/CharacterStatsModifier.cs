namespace Dimworld.Core.Modifiers;

using System;
using Dimworld.Core.Characters.Stats;
using Godot;


public abstract partial class CharacterStatsModifier : Modifier
{

    public CharacterStats TargetStats = null;

    public CharacterStatsModifier(string key) : base(key)
    {
    }


    public CharacterStatsModifier SetTargetStats(CharacterStats targetStats)
    {
        TargetStats = targetStats;
        return this;
    }


    public override void OnAdded(ModifierHandler handler)
    {
        base.OnAdded(handler);

        if (!IsInstanceValid(handler))
        {
            GD.PrintErr($"Modifier handler is not valid: {handler}");
            throw new Exception("Failed to add modifier: handler is not valid");
        }

        Node parent = handler.GetParent();

        if (parent is not IHasCharacterStats nodeWithStats)
        {
            GD.PrintErr($"Parent is not a CharacterController: {parent}");
            throw new Exception("Failed to add modifier: parent is not a CharacterController");
        }

        if (nodeWithStats.Stats == null)
        {
            GD.PrintErr($"TargetStats == null for modifier: {this}");
            throw new Exception("Failed to add modifier: TargetStats == null");
        }

        TargetStats = nodeWithStats.Stats;
    }


}

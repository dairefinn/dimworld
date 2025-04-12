namespace Dimworld.Items.Weapons;

using Dimworld.Core;
using Dimworld.Core.Characters;
using Dimworld.Core.Characters.Dialogue;
using Dimworld.Core.Effects;
using Dimworld.Core.Items;
using Dimworld.Effects;
using Godot;
using Godot.Collections;


public partial class Revolver : InventoryItem, ICanBeUsedFromHotbar, IUsesAmmo
{

    [Export] public float BulletRadius { get; set; } = 2.0f;
    [Export] public float BulletLength { get; set; } = 10.0f;
    [Export] public float BulletDamage { get; set; } = -20f;
    [Export] public float BulletSpeed { get; set; } = 200f;


    [Export] public int AmmoCount { get; set; } = 6;
    public int AmmoRemaining { get; set; } = 6;
    public float AmmoReloadTime { get; set; } = 1.0f;


    public Revolver()
    {
        AmmoRemaining = AmmoCount;
    }


    public bool UseFromHotbar(EquipmentHandler equipmentHandler)
    {
        if (AmmoRemaining <= 0)
        {
            TriggerDialogue(equipmentHandler, "Out of ammo!");
            return false;
        }

        // Effects will be placed at <radius> units away in the direction of the cursor
        Vector2 direction = equipmentHandler.GlobalPosition.DirectionTo(Globals.Instance.CursorFollower.GlobalPosition);
        Vector2 effectPosition = equipmentHandler.GlobalPosition + (direction * BulletRadius);

        CircleShape2D effectArea = new()
        {
            Radius = BulletRadius
        };
        Node parent = equipmentHandler.GetParent();
        Array<Node> nodeBlacklist = [parent];

        Effect damageEffect = new AddHealthEffect(effectArea, [1, 2], BulletDamage).SetDuration(10f).SetNodeBlacklist(nodeBlacklist).SetStartPosition(effectPosition).SetVelocity(direction * BulletSpeed);

        Globals.Instance.LevelHandler.CurrentLevel.AddChild(damageEffect);

        // TODO: Simple way of rendering the bullets visually using primitive meshes. Replace with an actual sprite.
        MeshInstance2D mesh = new()
        {
            Mesh = new CapsuleMesh()
            {
                Radius = BulletRadius,
                Height = BulletLength,
            }
        };
        mesh.Rotate(Mathf.DegToRad(90f));
        damageEffect.AddChild(mesh);

        damageEffect.GlobalRotation = direction.Angle();

        AmmoRemaining--;

        return true;
    }

    public bool Reload(EquipmentHandler equipmentHandler)
    {
        if (AmmoRemaining >= AmmoCount) return false;

        AmmoRemaining++;
        TriggerDialogue(equipmentHandler, "Reloading...");

        Timer reloadTimer = new()
        {
            WaitTime = AmmoReloadTime,
            OneShot = true
        };
        reloadTimer.Timeout += () =>
        {
            TriggerDialogue(equipmentHandler, "Reloaded!");
            reloadTimer.QueueFree();
        };
        Globals.Instance.LevelHandler.CurrentLevel.AddChild(reloadTimer);
        reloadTimer.Start();

        return true;
    }

    private void TriggerDialogue(EquipmentHandler equipmentHandler, string message)
    {
        Node parent = equipmentHandler.GetParent();

        if (parent is not ICanSpeak canSpeak) return;

        canSpeak.Say(message);
    }

}

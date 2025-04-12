namespace Dimworld.Items.Weapons;

using Dimworld.Core.Items;


/// <summary>
/// An interface for items that use ammo.
/// </summary>
public interface IUsesAmmo
{

    public int AmmoCount { get; set; }
    public int AmmoRemaining { get; set; }
    public float AmmoReloadTime { get; set; } // TODO: Support different types of reloading (Magazines, one at a time, over time, etc.)

    public bool Reload(EquipmentHandler equipmentHandler);

}
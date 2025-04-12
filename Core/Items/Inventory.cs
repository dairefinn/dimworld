namespace Dimworld.Core.Items;

using Godot;
using Godot.Collections;


/// <summary>
/// Represents an entire inventory which contains a number of slots.
/// Provides a number of methods to query the associated slots and manage items inside of them.
/// </summary>
[GlobalClass]
public partial class Inventory : Resource
{

	[Signal] public delegate void OnUpdatedEventHandler();


	[Export] public string InventoryName = "Inventory";

	[Export] public Array<InventorySlot> Slots {
		get => _slots;
		set {
			_slots = value;
			OnSlotsUpdated();
			OnUpdate();
		}
	}
	private Array<InventorySlot> _slots = [];


	public Inventory()
	{
		_slots = _slots.Duplicate(true);
	}


	private void OnSlotsUpdated()
	{
		for(int i = 0; i < Slots.Count; i++)
		{
			// Make sure that no slot == null
			if (Slots[i] == null)
			{
				Slots[i] = new InventorySlot();
			}

			// Make sure that the OnUpdated signal for the slot also triggers the OnUpdated signal for the inventory
			if (!Slots[i].IsConnected(SignalName.OnUpdated, Callable.From(OnUpdate)))
			{
				Slots[i].OnUpdated += OnUpdate;	
			}
		}
	}

	private void OnUpdate()
	{
		EmitSignal(SignalName.OnUpdated);
	}


	/// <summary>
	/// Add an item to the inventory.
	/// </summary>
	/// <param name="item">The item to add.</param>
	/// <returns>A boolean indicating whether the item was added successfully.</returns>
	public bool AddItem(InventoryItem item, int quantity = 1)
	{
		if (item == null) return false;
		if (!CanAddItem(item, quantity)) return false;

		while (quantity > 0)
		{
			InventorySlot slot = GetFirstSlotWithItem(item, true);
			if (slot == null)
			{
				slot = GetFirstEmptySlot();
				if (slot == null) return false;
			}

			if (!slot.CanAddItem(item)) return false;

			slot.AddItem(item);
			quantity--;
		}

		return true;
	}

	/// <summary>
	/// Remove an item from the inventory.
	/// </summary>
	/// <param name="item">The item to remove.</param>
	/// <returns>A boolean indicating whether the item was removed successfully.</returns>
	public bool RemoveItem(InventoryItem item, int quantity = 1)
	{
		if (item == null) return false;
		if (CountItem(item) < quantity) return false;

		while (quantity > 0)
		{
			InventorySlot slot = GetFirstSlotWithItem(item);
			if (slot == null) return false;

			if (!slot.RemoveItem()) return false;

			quantity--;
		}

		return true;
	}

	/// <summary>
	/// Checks if there is space in the inventory for a given item. It will see if there are any slots that aren't full and if there are any empty slots.
	/// </summary>
	/// <param name="item"></param>
	/// <param name="quantity"></param>
	/// <returns></returns>
	public bool CanAddItem(InventoryItem item, int quantity = 1)
	{
		while(quantity > 0)
		{
			InventorySlot slot = GetFirstSlotWithItem(item, true);
			if (slot == null)
			{
				slot = GetFirstEmptySlot();
				if (slot == null) return false;
			}

			if (!slot.CanAddItem(item)) return false;

			quantity--;
		}
		return true;
	}

	/// <summary>
	/// Check if the inventory contains an item.
	/// </summary>
	/// <param name="item">The item to check for.</param>
	/// <returns>A boolean indicating whether the item is in the inventory.</returns>
	public bool HasItem(InventoryItem item)
	{
		foreach (InventorySlot slot in Slots)
		{
			if (slot.IsEmpty) continue;
			if (slot.Item == item) return true;
		}
		
		return false;
	}

	/// <summary>
	/// Check if the inventory contains an item by its ID.
	/// </summary>
	/// <param name="itemId">The ID of the item to check for.</param>
	/// <returns>A boolean indicating whether the item is in the inventory.</returns>
	public bool HasItem(string itemId)
	{
		foreach (InventorySlot slot in Slots)
		{
			if (slot.IsEmpty) continue;
			if (slot.Item.Id == itemId) return true;
		}
		
		return false;
	}

	public int CountItem(InventoryItem item)
	{
		int count = 0;
		foreach (InventorySlot slot in Slots)
		{
			if (slot.IsEmpty) continue;
			if (slot.Item == item) count += slot.Quantity;
		}
		
		return count;
	}

	/// <summary>
	/// Get the first empty slot in the inventory.
	/// </summary>
	/// <returns>The first empty slot in the inventory.</returns>
	public InventorySlot GetFirstEmptySlot()
	{
		foreach (InventorySlot slot in Slots)
		{
			if (slot.IsEmpty) return slot;
		}

		return null;
	}

	/// <summary>
	/// Get the first slot with an item in the inventory.
	/// </summary>
	/// <param name="item">The item to check for.</param>
	/// <param name="ignoreFull">Whether to ignore full slots.</param>
	/// <returns>The first slot with the item in the inventory.</returns>
	public InventorySlot GetFirstSlotWithItem(InventoryItem item, bool ignoreFull = false)
	{
		foreach (InventorySlot slot in Slots)
		{
			if (slot.IsEmpty) continue;
			if (slot.Item.Id != item.Id) continue;
			if (ignoreFull && slot.IsFull) continue;
			return slot;
		}

		return null;
	}

	/// <summary>
	/// Get the first slot with an item in the inventory by its ID.
	/// </summary>
	/// <param name="itemId">The ID of the item to check for.</param>
	/// <param name="ignoreFull">Whether to ignore full slots.</param>
	/// <returns>The first slot with the item in the inventory.</returns>
	public InventorySlot GetFirstSlotWithItem(string itemId, bool ignoreFull = false)
	{
		foreach (InventorySlot slot in Slots)
		{
			if (slot.IsEmpty) continue;
			if (slot.Item.Id != itemId) continue;
			if (ignoreFull && slot.IsFull) continue;
			return slot;
		}

		return null;
	}

	/// <summary>
	/// Check if the inventory is full.
	/// </summary>
	/// <returns>A boolean indicating whether the inventory is full.</returns>
	public bool IsFull()
	{
		foreach (InventorySlot slot in Slots)
		{
			if (slot.IsEmpty) return false;
		}

		return true;
	}

	/// <summary>
	/// Check if the inventory is empty.
	/// </summary>
	/// <returns>A boolean indicating whether the inventory is empty.</returns>
	public bool IsEmpty()
	{
		foreach (InventorySlot slot in Slots)
		{
			if (!slot.IsEmpty) return false;
		}

		return true;
	}

	/// <summary>
	/// Checks if a given slot is associated with this inventory.
	/// </summary>
	/// <param name="slot">The slot to check.</param>
	/// <returns>A boolean indicating whether the slot is in the inventory.</returns>
	public bool IsSlotInInventory(InventorySlot slot)
	{
		foreach (InventorySlot s in Slots)
		{
			if (s == slot) return true;
		}

		return false;
	}

}

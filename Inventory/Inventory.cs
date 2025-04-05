namespace Dimworld;

using System;
using Dimworld.Developer;
using Godot;
using Godot.Collections;


[GlobalClass]
public partial class Inventory : Resource
{

	[Signal] public delegate void OnUpdatedEventHandler();


	[Export] public string InventoryName = "Inventory";
	[Export] public Array<InventorySlot> Slots {
		get => _slots;
		set => SetSlots(value);
	}
	private Array<InventorySlot> _slots = [];


	public Inventory()
	{
	}

	public Inventory(Inventory inventory)
	{
		InventoryName = inventory.InventoryName;
		foreach (InventorySlot slot in inventory.Slots)
		{
			Slots.Add(new InventorySlot(slot));
		}
	}


	private void SetSlots(Array<InventorySlot> slots)
	{
		_slots = slots;
		PopulateAllSlots();
		CallDeferred(MethodName.OnUpdate);
	}

	public void PopulateAllSlots()
	{
		for(int i = 0; i < Slots.Count; i++)
		{
			if (Slots[i] == null)
			{
				Slots[i] = new InventorySlot();
			}

			if (!Slots[i].IsConnected(SignalName.OnUpdated, Callable.From(OnUpdate)))
			{
				Slots[i].OnUpdated += OnUpdate;
			}
		}
	}


	/// <summary>
	/// Add an item to the inventory.
	/// </summary>
	/// <param name="item">The item to add.</param>
	/// <returns>A boolean indicating whether the item was added successfully.</returns>
	public bool AddItem(InventoryItem item)
	{
		InventorySlot slot = GetFirstSlotWithItem(item, true);
		if (slot == null)
		{
			DeveloperConsole.Print("No slot found for item: " + item.ItemName);
			slot = GetFirstEmptySlot();
		}
		if (slot == null) return false;

		return slot.AddItem(item);
	}

	/// <summary>
	/// Remove an item from the inventory.
	/// </summary>
	/// <param name="item">The item to remove.</param>
	/// <returns>A boolean indicating whether the item was removed successfully.</returns>
	public bool RemoveItem(InventoryItem item)
	{
		if (!HasItem(item)) return false;

		foreach (InventorySlot slot in Slots)
		{
			if (slot.Item == item)
			{
				return slot.RemoveItem();
			}
		}

		return false;
	}

	private void OnUpdate()
	{
		EmitSignal(SignalName.OnUpdated);
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

	public bool HasItem(string itemId)
	{
		foreach (InventorySlot slot in Slots)
		{
			if (slot.IsEmpty) continue;
			if (slot.Item.Id == itemId) return true;
		}
		
		return false;
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

	public bool IsFull()
	{
		foreach (InventorySlot slot in Slots)
		{
			if (slot.IsEmpty) return false;
		}

		return true;
	}

	public bool IsSlotInInventory(InventorySlot slot)
	{
		foreach (InventorySlot s in Slots)
		{
			if (s == slot) return true;
		}

		return false;
	}

}

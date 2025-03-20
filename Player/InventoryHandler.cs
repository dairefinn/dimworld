namespace Dimworld;

using Godot;


public partial class InventoryHandler : Control
{

    [Export] public Inventory MainInventory;
    [Export] public Inventory SecondaryInventory;

    [Export] public InventoryUI primaryInventoryUI;
    [Export] public InventoryUI secondaryInventoryUI;

    public bool IsViewing => GetPrimaryInventoryVisibility() || GetSecondaryInventoryVisibility();


    private InventorySlotUI SelectedSlot {
        get => _selectedSlot;
        set {
            if (_selectedSlot != null)
            {
                _selectedSlot.Modulate = Colors.White;
            }

            _selectedSlot = value;
            if (_selectedSlot != null)
            {
                _selectedSlot.Modulate = Colors.Red;
            } 
        }
    }
    private InventorySlotUI _selectedSlot;


    public override void _Ready()
    {
        SetPrimaryInventory(MainInventory);
        SetSecondaryInventory(SecondaryInventory);

        primaryInventoryUI.SetVisibility(false);
        primaryInventoryUI.OnSlotClicked += OnSlotClicked;

        secondaryInventoryUI.SetVisibility(false);
        secondaryInventoryUI.OnSlotClicked += OnSlotClicked;
    }

    public void SetPrimaryInventory(Inventory inventory)
    {
        MainInventory = inventory;
        primaryInventoryUI.TargetInventory = inventory;
    }

    public void SetSecondaryInventory(Inventory inventory)
    {
        SecondaryInventory = inventory;
        secondaryInventoryUI.TargetInventory = inventory;
    }

    public void OpenSecondaryInventory(Inventory inventory)
    {
        SetSecondaryInventory(inventory);
        primaryInventoryUI.SetVisibility(true);
        secondaryInventoryUI.SetVisibility(true);
    }

    public void CloseSecondaryInventory()
    {
        SetSecondaryInventory(null);
        secondaryInventoryUI.SetVisibility(false);
    }

    public bool GetPrimaryInventoryVisibility()
    {
        if (primaryInventoryUI == null) return false;
        return primaryInventoryUI.Visible;
    }

    public bool GetSecondaryInventoryVisibility()
    {
        if (secondaryInventoryUI == null) return false;
        return secondaryInventoryUI.Visible;
    }

    public void SetPrimaryInventoryVisibility(bool visible)
    {
        primaryInventoryUI.SetVisibility(visible);
    }

    public void SetSecondaryInventoryVisibility(bool visible)
    {
        secondaryInventoryUI.SetVisibility(visible);
    }

    public void OnSlotClicked(InventorySlotUI slotUI)
    {
        GD.Print("Slot clicked: " + slotUI);
        if (slotUI == null) return;

        // If there is nothing selected already and the slot is empty, do nothing
        if (SelectedSlot == null && slotUI.TargetSlot.Item == null) return;

        // If we're re-selecting the same slot, deselect it
        if (SelectedSlot == slotUI)
        {
            SelectedSlot = null;
            return;
        }

        // If nothing has been selected already, select the slot
        if (SelectedSlot == null)
        {
            GD.Print("Slot selected: " + slotUI);
            SelectedSlot = slotUI;
            return;
        }

        // TODO: Might want to use the Inventory.AddItem methods here instead. Probaly want an AddItemAtIndex method so it goes to a specific slot.

        // If something has been selected already and the next slot has something in it, swap the items
        if (slotUI.TargetSlot.Item != null)
        {
            GD.Print("Swapping items: " + SelectedSlot + " and " + slotUI);

            InventoryItem sourceItem = SelectedSlot.TargetSlot.Item;
            int sourceQuantity = SelectedSlot.TargetSlot.Quantity;

            SelectedSlot.TargetSlot.Item = null;

            // InventoryItem targetItem = slotUI.TargetSlot.Item;
            // int targetQuantity = slotUI.TargetSlot.Quantity;

            // SelectedSlot.TargetSlot.Item = null;
            // slotUI.TargetSlot.Item = null;

            // slotUI.TargetSlot.Item = sourceItem;
            // slotUI.TargetSlot.Quantity = sourceQuantity;

            SelectedSlot = null;
            return;
        }

        // If something has been selected already and the next slot is empty, move the item
        GD.Print("Moving item: " + SelectedSlot + " to " + slotUI);
        slotUI.TargetSlot.AddFromExisting(SelectedSlot.TargetSlot);
        SelectedSlot.UpdateUI();
        SelectedSlot = null;
    }

}

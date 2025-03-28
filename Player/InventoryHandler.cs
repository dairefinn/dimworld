namespace Dimworld;

using Godot;


public partial class InventoryHandler : Control
{

    [Export] public Inventory PrimaryInventory {
        get => _primaryInventory;
        set => SetPrimaryInventory(value);
    }
    private Inventory _primaryInventory;

    [Export] public Inventory SecondaryInventory {
        get => _secondaryInventory;
        set => SetSecondaryInventory(value);
    }
    private Inventory _secondaryInventory;

    private InventoryUI primaryInventoryUI {
        get => _primaryInventoryUI;
        set => SetPrimaryInventoryUI(value);
    }
    private InventoryUI _primaryInventoryUI;

    private InventoryUI secondaryInventoryUI {
        get => _secondaryInventoryUI;
        set => SetSecondaryInventoryUI(value);
    }
    private InventoryUI _secondaryInventoryUI;


    private InventoryContextMenuUI ContextMenu;


    public bool IsViewing => GetPrimaryInventoryVisibility() || GetSecondaryInventoryVisibility();


    // LIFECYCLE EVENTS

    public override void _Ready()
    {
        primaryInventoryUI = GetNode<InventoryUI>("%PrimaryInventoryUI");
        secondaryInventoryUI = GetNode<InventoryUI>("%SecondaryInventoryUI");
        ContextMenu = GetNode<InventoryContextMenuUI>("%ContextMenu");

        ContextMenu.Visible = false;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }


    // PROPERTY SETTERS

    private void SetPrimaryInventory(Inventory inventory)
    {
        if (inventory == null){
            GD.Print("Primary inventory is null");
        }

        _primaryInventory = inventory;

        if (primaryInventoryUI != null)
        {
            primaryInventoryUI.TargetInventory = inventory;
        }

        if (inventory == null)
        {
            GD.Print("Opening primary inventory: " + inventory.InventoryName);
            foreach (InventorySlot slot in inventory.Slots)
            {
                GD.Print("Slot: " + (slot.Item != null ? slot.Item.ItemName : "Empty"));
            }
            SetPrimaryInventoryVisibility(false);
        }
    }

    private void SetSecondaryInventory(Inventory value)
    {
        _secondaryInventory = value;
        
        if (secondaryInventoryUI != null)
        {
            secondaryInventoryUI.TargetInventory = value;
        }

        if (value == null)
        {
            SetSecondaryInventoryVisibility(false);
        }
    }

    private void SetPrimaryInventoryUI(InventoryUI value)
    {
        // Unregister old inventory
        if (_primaryInventoryUI != null)
        {
            _primaryInventoryUI.TargetInventory = null;
            _primaryInventoryUI.OnVisibilityChanged -= OnSecondaryVisibilityChanged;
        }

        // Update value
        _primaryInventoryUI = value;

        // Register new inventory
        if (value != null)
        {
            _primaryInventoryUI.TargetInventory = PrimaryInventory;
            _primaryInventoryUI.OnVisibilityChanged += OnPrimaryVisibilityChanged;
        }
    }

    private void SetSecondaryInventoryUI(InventoryUI value)
    {
        // Unregister old inventory
        if (_secondaryInventoryUI != null)
        {
            _secondaryInventoryUI.TargetInventory = null;
            _secondaryInventoryUI.OnVisibilityChanged -= OnSecondaryVisibilityChanged;
        }

        // Update value
        _secondaryInventoryUI = value;

        // Register new inventory
        if (value != null)
        {
            _secondaryInventoryUI.TargetInventory = SecondaryInventory;
            _secondaryInventoryUI.OnVisibilityChanged += OnSecondaryVisibilityChanged;
        }
    }

    // INVENTORY VISIBILITY

    public void OpenSecondaryInventory(Inventory inventory)
    {
        SetSecondaryInventory(inventory);
        SetBothInventoriesVisibility(true);
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

    public void SetBothInventoriesVisibility(bool visible)
    {
        SetPrimaryInventoryVisibility(visible);
        SetSecondaryInventoryVisibility(visible);
    }

    private void OnPrimaryVisibilityChanged(bool visible)
    {
        OnAnyVisibilityChanged(visible);
    }

    private void OnSecondaryVisibilityChanged(bool visible)
    {
        OnAnyVisibilityChanged(visible);
    }

    private void OnAnyVisibilityChanged(bool visible)
    {
        if (!visible)
        {
            ContextMenu.Hide();
        }
    }


    // MOVING ITEMS

    public void MoveItemFromSlotToSlot(InventorySlotUI sourceSlot, InventorySlotUI targetSlot)
    {
        if (sourceSlot == null) return;
        if (targetSlot == null) return;
        if (sourceSlot.TargetSlot.IsEmpty) return;
        if (sourceSlot == targetSlot) return;
        
        bool isChangingInventories = sourceSlot.ParentInventoryUI != targetSlot.ParentInventoryUI;

        // Perform the actual swap
        targetSlot.TargetSlot.SwapWithExisting(sourceSlot.TargetSlot);

        if (isChangingInventories)
        {
            InputHandler.Instance.PlayerAgent.EquipmentHandler.Unequip(sourceSlot.TargetSlot.Item);
            InputHandler.Instance.PlayerAgent.EquipmentHandler.Unequip(targetSlot.TargetSlot.Item);
        }

        sourceSlot.UpdateUI();
        targetSlot.UpdateUI();
    }


    // CONTEXT MENU

    public void RequestContextMenu(InventorySlotUI inventorySlotUI)
    {
        if (inventorySlotUI == null) return;
        if (inventorySlotUI.TargetSlot.IsEmpty) return;

        float slotWidth = inventorySlotUI.GetRect().Size.X;
        Vector2 contextMenuPosition = inventorySlotUI.GlobalPosition + new Vector2(slotWidth, 10);

        if (ContextMenu.GlobalPosition == contextMenuPosition && ContextMenu.Visible)
        {
            ContextMenu.Hide();
            return;
        }

        bool itemIsInParentInventory = inventorySlotUI.ParentInventoryUI == primaryInventoryUI;

        InventoryContextMenuUI.ContextMenuOption[] options = inventorySlotUI.TargetSlot.Item.GetContextMenuOptions(ContextMenu, InputHandler.Instance.PlayerAgent.EquipmentHandler, itemIsInParentInventory);
        if (options == null || options.Length == 0) return; // If an item doesn't provide any context menu options, don't show the context menu

        ContextMenu.OnOptionSelected += () => OnContextMenuOptionSelected(inventorySlotUI);
        ContextMenu.Show(contextMenuPosition, options);
    }

    public void OnContextMenuOptionSelected(InventorySlotUI slotUI)
    {
        if (slotUI == null) return;

        ContextMenu.Hide();
        slotUI.UpdateUI();
    }

}

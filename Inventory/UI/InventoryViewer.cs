namespace Dimworld;

using Dimworld.Developer;
using Godot;


public partial class InventoryViewer : Control
{

    [Export] public Inventory PrimaryInventory {
        get => _primaryInventory;
        set {
            _primaryInventory = value;
            OnUpdateReferences();
        }
    }
    private Inventory _primaryInventory;

    [Export] public Inventory SecondaryInventory {
        get => _secondaryInventory;
        set {
            _secondaryInventory = value;
            OnUpdateReferences();
        }
    }
    private Inventory _secondaryInventory;

    [Export] private InventoryUI PrimaryInventoryUI {
        get => _primaryInventoryUI;
        set {
            if (_primaryInventoryUI != null)
            {
                _primaryInventoryUI.TargetInventory = null;
                _primaryInventoryUI.OnVisibilityChanged -= OnSecondaryVisibilityChanged;
            }
            _primaryInventoryUI = value;
            OnUpdateReferences();
        }
    }
    private InventoryUI _primaryInventoryUI;

    [Export] private InventoryUI SecondaryInventoryUI {
        get => _secondaryInventoryUI;
        set {
            if (_secondaryInventoryUI != null)
            {
                _secondaryInventoryUI.TargetInventory = null;
                _secondaryInventoryUI.OnVisibilityChanged -= OnSecondaryVisibilityChanged;
            }
            _secondaryInventoryUI = value;
            OnUpdateReferences();
        }
    }
    private InventoryUI _secondaryInventoryUI;

    [Export] public InventoryHotbar Hotbar {
        get => _hotbar;
        set {
            if (_hotbar != null)
            {
                _hotbar.Inventory = null;
                _hotbar.ColumnCount = 5;
                _hotbar.HotbarRow = 0;
            }

            _hotbar = value;
            OnUpdateReferences();
        }
    }
    private InventoryHotbar _hotbar;
    [Export] public InventoryContextMenuUI ContextMenu { get; set; }


    public bool IsViewing => GetPrimaryInventoryVisibility() || GetSecondaryInventoryVisibility();


    public void OnUpdateReferences()
    {
        if (PrimaryInventory != null)
        {
            if (PrimaryInventoryUI != null)
            {
                PrimaryInventoryUI.TargetInventory = PrimaryInventory;
            }
        }
        else
        {
            SetPrimaryInventoryVisibility(false);
        }

        if (SecondaryInventory != null)
        {
            if (SecondaryInventoryUI != null)
            {
                SecondaryInventoryUI.TargetInventory = SecondaryInventory;
            }
        }
        else
        {
            SetSecondaryInventoryVisibility(false);
        }

        if (IsInstanceValid(PrimaryInventoryUI))
        {
            PrimaryInventoryUI.OnVisibilityChanged -= OnPrimaryVisibilityChanged;
            PrimaryInventoryUI.OnVisibilityChanged += OnPrimaryVisibilityChanged;
        }

        if (IsInstanceValid(SecondaryInventoryUI))
        {
            SecondaryInventoryUI.OnVisibilityChanged -= OnSecondaryVisibilityChanged;
            SecondaryInventoryUI.OnVisibilityChanged += OnSecondaryVisibilityChanged;
        }
        
        if (IsInstanceValid(Hotbar))
        {
            Hotbar.Inventory = PrimaryInventory;

            if (IsInstanceValid(PrimaryInventoryUI))
            {
                Hotbar.ColumnCount = PrimaryInventoryUI.SlotsGrid.Columns;
                Hotbar.HotbarRow = PrimaryInventoryUI.RowsDisplayed; // Hotbar is the last row
            }
        }
    }


    // INVENTORY VISIBILITY

    public void OpenSecondaryInventory(Inventory inventory)
    {
        SecondaryInventory = inventory;
        SetBothInventoriesVisibility(true);
    }

    public bool GetPrimaryInventoryVisibility()
    {
        if (PrimaryInventoryUI == null) return false;
        return PrimaryInventoryUI.Visible;
    }

    public bool GetSecondaryInventoryVisibility()
    {
        if (SecondaryInventoryUI == null) return false;
        return SecondaryInventoryUI.Visible;
    }

    public void SetPrimaryInventoryVisibility(bool visible)
    {
        PrimaryInventoryUI?.SetVisibility(visible);
    }

    public void SetSecondaryInventoryVisibility(bool visible)
    {
        SecondaryInventoryUI?.SetVisibility(visible);
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
            ContextMenu?.Hide();
        }

        Hotbar?.SetSelectable(visible);
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
            Globals.Instance.Player.EquipmentHandler.Unequip(sourceSlot.TargetSlot.Item);
            Globals.Instance.Player.EquipmentHandler.Unequip(targetSlot.TargetSlot.Item);
        }

        sourceSlot.UpdateUI();
        targetSlot.UpdateUI();
    }


    // CONTEXT MENU

    public void RequestContextMenu(InventorySlotUI inventorySlotUI)
    {
        if (!IsInstanceValid(inventorySlotUI)) return;
        if (!IsInstanceValid(ContextMenu)) return;
        if (inventorySlotUI.TargetSlot.IsEmpty) return;

        float slotWidth = inventorySlotUI.GetRect().Size.X;
        Vector2 contextMenuPosition = inventorySlotUI.GlobalPosition + new Vector2(slotWidth, 10);

        if (ContextMenu.GlobalPosition == contextMenuPosition && ContextMenu.Visible)
        {
            ContextMenu.Hide();
            return;
        }

        bool itemIsInParentInventory = inventorySlotUI.ParentInventoryUI == PrimaryInventoryUI;

        InventoryContextMenuUI.ContextMenuOption[] options = inventorySlotUI.TargetSlot.Item.GetContextMenuOptions(ContextMenu, Globals.Instance.Player.EquipmentHandler, itemIsInParentInventory);
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

    public void TryUseSelectedItem()
    {
        if (!IsInstanceValid(Hotbar)) return;
        if (!IsInstanceValid(Hotbar.SelectedSlotUI)) return;
        if (Hotbar.SelectedSlotUI.TargetSlot == null) return;
        if (Hotbar.SelectedSlotUI.TargetSlot.IsEmpty) return;

        InventoryItem item = Hotbar.SelectedSlotUI.TargetSlot.Item;

        if (item != null)
        {
            GD.Print("Using item: " + item.ItemName);
            // TODO: Implement using items. Might want to use the EquipmentHandler for this.
            // item.Use();
        }
    }

}

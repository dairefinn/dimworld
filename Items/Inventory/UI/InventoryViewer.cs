namespace Dimworld.Items.UI;

using Dimworld.Items.Weapons;
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
            PrimaryInventoryUI.OnSlotClicked += OnPrimaryInventorySlotClicked;
        }

        if (IsInstanceValid(SecondaryInventoryUI))
        {
            SecondaryInventoryUI.OnVisibilityChanged -= OnSecondaryVisibilityChanged;
            SecondaryInventoryUI.OnVisibilityChanged += OnSecondaryVisibilityChanged;
            SecondaryInventoryUI.OnSlotClicked += OnSecondaryInventorySlotClicked;
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

    
    // QUICK TRANSFERRING

    private void OnPrimaryInventorySlotClicked(InventorySlotUI slotUI)
    {
        // Move the item from the primary inventory to the secondary inventory
        MoveItemFromSourceToDestination(PrimaryInventoryUI, SecondaryInventoryUI, slotUI, null);
    }

    private void OnSecondaryInventorySlotClicked(InventorySlotUI slotUI)
    {
        // Move the item from the secondary inventory to the primary inventory
        MoveItemFromSourceToDestination(SecondaryInventoryUI, PrimaryInventoryUI, slotUI, null);
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

    public void MoveItemFromSourceToDestination(InventoryUI sourceInventory, InventoryUI destinationInventory, InventorySlotUI sourceSlot, InventorySlotUI destinationSlot)
    {
        if (sourceSlot == null) return;
        if (sourceSlot.TargetSlot.IsEmpty) return;
        if (sourceSlot == destinationSlot) return;
        if (destinationSlot == null)
        {
            destinationSlot = destinationInventory.GetFirstEmptySlot();
            if (destinationSlot == null) return;
        }

        bool isChangingInventories = sourceInventory != destinationInventory;

        // Perform the actual swap
        destinationSlot.TargetSlot.SwapWithExisting(sourceSlot.TargetSlot);

        if (isChangingInventories)
        {
            if (sourceSlot.TargetSlot.Item is ICanBeEquipped sourceItemCanBeEquipped)
            {
                Globals.Instance.Player.EquipmentHandler.Unequip(sourceItemCanBeEquipped);
            }

            if (destinationSlot.TargetSlot.Item is ICanBeEquipped targetItemCanBeEquipped)
            {
                Globals.Instance.Player.EquipmentHandler.Unequip(targetItemCanBeEquipped);
            }
        }

        // TODO: InventorySlot.OnUpdate signal should handle this
        // sourceSlot.UpdateUI();
        // destinationSlot.UpdateUI();
    }


    // CONTEXT MENU

    public void RequestContextMenu(InventorySlotUI inventorySlotUI)
    {
        if (!IsInstanceValid(inventorySlotUI)) return;
        if (!IsInstanceValid(ContextMenu)) return;

        float slotWidth = inventorySlotUI.GetRect().Size.X;
        Vector2 contextMenuPosition = inventorySlotUI.GlobalPosition + new Vector2(slotWidth, 10);

        // If the context menu is already open and the position is the same, hide it
        if ((ContextMenu.GlobalPosition == contextMenuPosition && ContextMenu.Visible))
        {
            ContextMenu.Hide();
            return;
        }

        // If the target slot is empty or doesn't have a context menu, hide the context menu
        if (inventorySlotUI.TargetSlot.IsEmpty || inventorySlotUI.TargetSlot.Item is not IHasContextMenu)
        {
            ContextMenu.Hide();
            return;
        }

        if (inventorySlotUI.TargetSlot.IsEmpty) return;
        if (inventorySlotUI.TargetSlot.Item is not IHasContextMenu itemWithContextMenu) return;

        bool itemIsInParentInventory = inventorySlotUI.ParentInventoryUI == PrimaryInventoryUI;

        InventoryContextMenuUI.ContextMenuOption[] options = itemWithContextMenu.GetContextMenuOptions(ContextMenu, Globals.Instance.Player.EquipmentHandler, itemIsInParentInventory);
        if (options == null || options.Length == 0) return; // If an item doesn't provide any context menu options, don't show the context menu

        ContextMenu.OnOptionSelected += () => OnContextMenuOptionSelected(inventorySlotUI);
        ContextMenu.Show(contextMenuPosition, options);
    }

    public void OnContextMenuOptionSelected(InventorySlotUI slotUI)
    {
        if (slotUI == null) return;

        ContextMenu.Hide();
        // slotUI.UpdateUI();
    }

    public void TryUseSelectedItem()
    {
        InventorySlot selectedSlot = GetSelectedSlotWithItem();
        if (selectedSlot == null) return;

        InventoryItem item = selectedSlot.Item;

        if (item is ICanBeUsedFromHotbar itemCanBeUsedFromHotbar)
        {
            itemCanBeUsedFromHotbar.UseFromHotbar(Globals.Instance.Player.EquipmentHandler);
            selectedSlot.EmitSignal(InventorySlot.SignalName.OnUpdated);
        }
    }

    public void TryReloadSelectedItem()
    {
        InventorySlot selectedSlot = GetSelectedSlotWithItem();
        if (selectedSlot == null) return;

        InventoryItem item = selectedSlot.Item;

        if (item is IUsesAmmo itemUsesAmmo)
        {
            itemUsesAmmo.Reload(Globals.Instance.Player.EquipmentHandler);
            selectedSlot.EmitSignal(InventorySlot.SignalName.OnUpdated);
        }
    }

    private InventorySlot GetSelectedSlotWithItem()
    {   
        if (!IsInstanceValid(Hotbar)) return null;
        if (Hotbar.SelectedSlotUI == null) return null;
        if (Hotbar.SelectedSlotUI.TargetSlot == null) return null;
        if (Hotbar.SelectedSlotUI.TargetSlot.IsEmpty) return null;

        return Hotbar.SelectedSlotUI.TargetSlot;
    }

}

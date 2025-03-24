namespace Dimworld;

using Godot;


public partial class InventoryHandler : Control
{

    [Export] public Inventory PrimaryInventory {
        get => _primaryInventory;
        set => SetPrimaryInventory(value);
    }
    private Inventory _primaryInventory;
    [Export] public EquipmentHandler PrimaryEquipmentHandler {
        get => _primaryEquipmentHandler;
        set => SetPrimaryEquipmentHandler(value);
    }
    private EquipmentHandler _primaryEquipmentHandler;

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

    private InventorySlotUI SelectedSlot {
        get => _selectedSlot;
        set {
            // TODO: Improve selected slot UI
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

    private InventoryContextMenuUI ContextMenu;


    public bool IsViewing => GetPrimaryInventoryVisibility() || GetSecondaryInventoryVisibility();


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


    private void SetPrimaryInventory(Inventory value)
    {
        _primaryInventory = value;

        if (primaryInventoryUI != null)
        {
            primaryInventoryUI.TargetInventory = value;
        }

        if (value == null)
        {
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
        _primaryInventoryUI = value;

        if (value != null)
        {
            _primaryInventoryUI.TargetInventory = PrimaryInventory;
            _primaryInventoryUI.OnSlotClicked += OnSlotClicked;
            _primaryInventoryUI.OnSlotClickedAlternate += OnSlotClickedAlternate;
            _primaryInventoryUI.OnVisibilityChanged += OnPrimaryVisibilityChanged;
        }
    }

    private void SetSecondaryInventoryUI(InventoryUI value)
    {
        _secondaryInventoryUI = value;

        if (value != null)
        {
            _secondaryInventoryUI.TargetInventory = SecondaryInventory;
            _secondaryInventoryUI.OnSlotClicked += OnSlotClicked;
            _secondaryInventoryUI.OnSlotClickedAlternate += OnSlotClickedAlternate;
            _secondaryInventoryUI.OnVisibilityChanged += OnSecondaryVisibilityChanged;
        }
    }

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

    private void OnSlotClicked(InventorySlotUI slotUI)
    {
        if (slotUI == null) return;

        // If there is nothing selected already and the slot is empty, do nothing
        if (SelectedSlot == null && slotUI.TargetSlot.Item == null) return;

        // If we're re-selecting the same slot, deselect it. If the item is usable, use it.
        if (SelectedSlot == slotUI)
        {
            SelectedSlot = null;
            return;
        }

        // If nothing has been selected already, select the slot
        if (SelectedSlot == null)
        {
            SelectedSlot = slotUI;
            return;
        }

        // If something has been selected already and the next slot has something in it, swap the items
        if (slotUI.TargetSlot.Item != null)
        {
            slotUI.TargetSlot.SwapWithExisting(SelectedSlot.TargetSlot);

            SelectedSlot = null;
            return;
        }

        // If something has been selected already and the next slot is empty, move the item
        slotUI.TargetSlot.AddFromExisting(SelectedSlot.TargetSlot);
        SelectedSlot.UpdateUI();
        SelectedSlot = null;
    }

    private void OnSlotClickedAlternate(InventorySlotUI slotUI)
    {
        if (slotUI == null) return;
        if (slotUI.TargetSlot.IsEmpty) return;

        float slotWidth = slotUI.GetRect().Size.X;
        Vector2 contextMenuPosition = slotUI.GlobalPosition + new Vector2(slotWidth, 10);

        InventoryContextMenuUI.ContextMenuOption[] options = slotUI.TargetSlot.Item.GetContextMenuOptions(ContextMenu, PrimaryEquipmentHandler);

        ContextMenu.Show(contextMenuPosition, options);
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
            SelectedSlot = null;
            ContextMenu.Hide();
        }
    }


    public void SetPrimaryEquipmentHandler(EquipmentHandler value)
    {
        _primaryEquipmentHandler = value;
    }

}

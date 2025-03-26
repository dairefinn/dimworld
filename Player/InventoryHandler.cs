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
            _primaryInventoryUI.OnVisibilityChanged += OnPrimaryVisibilityChanged;
        }
    }

    private void SetSecondaryInventoryUI(InventoryUI value)
    {
        _secondaryInventoryUI = value;

        if (value != null)
        {
            _secondaryInventoryUI.TargetInventory = SecondaryInventory;
            _secondaryInventoryUI.OnVisibilityChanged += OnSecondaryVisibilityChanged;
        }
    }

    public void SetPrimaryEquipmentHandler(EquipmentHandler value)
    {
        _primaryEquipmentHandler = value;
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


    // CONTEXT MENU
    // TODO: Re-implement with state machines

    private void OnSlotClickedAlternate(InventorySlotUI slotUI)
    {
        if (slotUI == null) return;
        if (slotUI.TargetSlot.IsEmpty) return;

        float slotWidth = slotUI.GetRect().Size.X;
        Vector2 contextMenuPosition = slotUI.GlobalPosition + new Vector2(slotWidth, 10);

        if (ContextMenu.GlobalPosition == contextMenuPosition && ContextMenu.Visible)
        {
            ContextMenu.Hide();
            return;
        }

        InventoryContextMenuUI.ContextMenuOption[] options = slotUI.TargetSlot.Item.GetContextMenuOptions(ContextMenu, PrimaryEquipmentHandler);
        if (options == null) return; // If item returns null for options, don't show the context menu

        // ContextMenu.OnOptionSelected += () => OnContextMenuOptionSelected(slotUI);
        ContextMenu.Show(contextMenuPosition, options);
    }

    // public void OnContextMenuOptionSelected(InventorySlotUI slotUI)
    // {
    //     if (slotUI == null) return;

    //     ContextMenu.Hide();

    //     // SELECTED

    //     if (slotUI == SelectedSlot)
    //     {
    //         GD.Print("Selected slot context menu option selected");
    //         // slotUI.SetTheme(InventorySlotUI.InventorySlotTheme.Selected);
    //         return;
    //     }

    //     // DEFAULT

    //     GD.Print("Default slot context menu option selected");
    //     // slotUI.SetTheme();
    // }

    // #endregion
    

}

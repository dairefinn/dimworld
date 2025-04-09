namespace Dimworld.Items.UI;

using Dimworld.Items.Weapons;
using Godot;


public partial class InventorySlotUI : Panel
{

    private static readonly StyleBox STYLEBOX_DEFAULT = GD.Load<StyleBox>("res://Items/Inventory/UI/Slot/Styles/InventorySlotUI_Default.tres");
    private static readonly StyleBox STYLEBOX_ACTIVE = GD.Load<StyleBox>("res://Items/Inventory/UI/Slot/Styles/InventorySlotUI_Active.tres");
    private static readonly StyleBox STYLEBOX_SELECTED = GD.Load<StyleBox>("res://Items/Inventory/UI/Slot/Styles/InventorySlotUI_Selected.tres");


    [Signal] public delegate void OnSlotClickedEventHandler(InventorySlotUI slotUI);


    [Export] public InventorySlot TargetSlot {
        get => _targetSlot;
        set {
            _targetSlot = value;
            OnUpdateTargetSlot();
        }
    }
    private InventorySlot _targetSlot;
    [Export] public int SlotIndex {
        get => _slotIndex;
        set {
            _slotIndex = value;
            UpdateUI();
        }
    }
    private int _slotIndex = -1;
    [Export] public bool CanBeSelected { get; set; } = true;

    public InventoryUI ParentInventoryUI { get; set; }

    [Export] public TextureRect ItemIcon;
    [Export] public Label QuantityLabel;
    [Export] public Label ItemLabel;
    [Export] public Label IndexLabel;
    [Export] public Panel HoverOverlay;


    public InventorySlotDragArea DragArea;


    private InventorySlotStateMachine StateMachine;


    // LIFECYCLE EVENTS
    
    public override void _Ready()
    {
        StateMachine = GetNode<InventorySlotStateMachine>("%StateMachine");
        DragArea = GetNode<InventorySlotDragArea>("%DragArea");
        DragArea.ParentSlot = this;

        StateMachine?.Init(this);

        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;

        UpdateUI();
    }

    public override void _Input(InputEvent @event)
    {
		if (IsInstanceValid(StateMachine))
        {
            StateMachine.OnInput(@event);
        }
    }

    public override void _GuiInput(InputEvent @event)
    {
		if (IsInstanceValid(StateMachine))
        {
            StateMachine.OnGuiInput(@event);
        }
    }

    // SIGNAL HANDLERS

	public void OnMouseEntered()
	{
		if (IsInstanceValid(HoverOverlay))
        {
            HoverOverlay.Show();
        }
		if (IsInstanceValid(StateMachine))
        {
            StateMachine.OnMouseEntered();
        }
	}

	public void OnMouseExited()
	{
        if (IsInstanceValid(HoverOverlay))
        {
            HoverOverlay.Hide();
        }
        if (IsInstanceValid(StateMachine))
        {
            StateMachine.OnMouseExited();
        }
	}


    // SETTERS

    public void OnUpdateTargetSlot()
    {
        UpdateUI();

        if (_targetSlot == null) return;
        _targetSlot.OnUpdated += UpdateUI;
    }

    public void UpdateUI()
    {
        if (!IsInstanceValid(this)) return;

        string itemText = "";
        bool canHoldMultiple = false;
        int itemQuantity = 0;
        Texture2D itemIcon = null;
        bool IsEquipped = false;

        if (_targetSlot != null && _targetSlot.Item != null)
        {
            itemText = GetLabelText(_targetSlot.Item);
            canHoldMultiple = _targetSlot.Item.MaxStackSize > 1;
            itemQuantity = _targetSlot.Quantity;
            itemIcon = _targetSlot.Item.Icon;
            IsEquipped = _targetSlot.Item.IsEquipped;
        }

        if (IsInstanceValid(ItemLabel))
        {
            ItemLabel.Text = itemText;
        }

        if (IsInstanceValid(QuantityLabel))
        {
            QuantityLabel.Text = itemQuantity.ToString();
            QuantityLabel.Visible = canHoldMultiple && itemQuantity > 0;
        }

        if (IsInstanceValid(ItemIcon))
        {
            ItemIcon.Texture = itemIcon;
        }

        if (IsInstanceValid(IndexLabel))
        {
            IndexLabel.Text = _slotIndex.ToString();
            IndexLabel.Visible = SlotIndex >= 0;
        }
        
        if(IsEquipped)
		{
            SetStyle(StyleType.Active);
		}
		else
		{
            SetStyle(StyleType.Default);
		}
    }
    
    public void SetStyle(StyleType type)
    {
        if (!IsInstanceValid(this)) return;

        switch (type)
        {
            case StyleType.Default:
                Set("theme_override_styles/panel", STYLEBOX_DEFAULT);
                break;
            case StyleType.Active:
                Set("theme_override_styles/panel", STYLEBOX_ACTIVE);
                break;
            case StyleType.Selected:
                Set("theme_override_styles/panel", STYLEBOX_SELECTED);
                break;
        }
    }

    public StyleType GetStyle()
    {
        if (!IsInstanceValid(this)) return StyleType.Default;
        if (Get("theme_override_styles/panel").Equals(null)) return StyleType.Default;

        if (Get("theme_override_styles/panel").Equals(STYLEBOX_DEFAULT))
        {
            return StyleType.Default;
        }
        
        if (Get("theme_override_styles/panel").Equals(STYLEBOX_ACTIVE))
        {
            return StyleType.Active;
        }
        
        if (Get("theme_override_styles/panel").Equals(STYLEBOX_SELECTED))
        {
            return StyleType.Selected;
        }

        return StyleType.Default;
    }

    private string GetLabelText(InventoryItem item)
    {
        if (item == null) return "";

        if (item is IUsesAmmo usesAmmo)
        {
            return $"{usesAmmo.AmmoRemaining}/{usesAmmo.AmmoCount}";
        }
        
        return "";
    }

    public enum StyleType
    {
        Default,
        Active,
        Selected
    }

}

namespace Dimworld.Items.UI;

using Godot;


public partial class InventorySlotUI : Panel
{

    public static readonly StyleBox STYLEBOX_DEFAULT = GD.Load<StyleBox>("res://Items/Inventory/UI/Slot/Styles/InventorySlotUI_Default.tres");
    public static readonly StyleBox STYLEBOX_ACTIVE = GD.Load<StyleBox>("res://Items/Inventory/UI/Slot/Styles/InventorySlotUI_Active.tres");
    public static readonly StyleBox STYLEBOX_SELECTED = GD.Load<StyleBox>("res://Items/Inventory/UI/Slot/Styles/InventorySlotUI_Selected.tres");


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

        string itemName = "";
        int itemQuantity = 0;
        Texture2D itemIcon = null;
        bool IsEquipped = false;

        if (_targetSlot != null && _targetSlot.Item != null)
        {
            itemName = _targetSlot.Item.ItemName;
            itemQuantity = _targetSlot.Quantity;
            itemIcon = _targetSlot.Item.Icon;
            IsEquipped = _targetSlot.Item.IsEquipped;
        }

        if (IsInstanceValid(ItemLabel))
        {
            ItemLabel.Text = itemName;
        }

        if (IsInstanceValid(QuantityLabel))
        {
            QuantityLabel.Text = itemQuantity.ToString();
            QuantityLabel.Visible = itemQuantity > 0;
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
			Set("theme_override_styles/panel", STYLEBOX_ACTIVE);
		}
		else
		{
			Set("theme_override_styles/panel", STYLEBOX_DEFAULT);
		}
    }

}

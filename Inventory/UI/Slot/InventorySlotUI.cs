namespace Dimworld;

using Godot;


public partial class InventorySlotUI : Panel
{

    public static readonly StyleBox STYLEBOX_DEFAULT = GD.Load<StyleBox>("res://Inventory/UI/Slot/Styles/InventorySlotUI_Default.tres");
    public static readonly StyleBox STYLEBOX_ACTIVE = GD.Load<StyleBox>("res://Inventory/UI/Slot/Styles/InventorySlotUI_Active.tres");
    public static readonly StyleBox STYLEBOX_SELECTED = GD.Load<StyleBox>("res://Inventory/UI/Slot/Styles/InventorySlotUI_Selected.tres");


    [Export] public InventorySlot TargetSlot {
        get => _targetSlot;
        set => SetTargetSlot(value);
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

    public TextureRect ItemIcon;
    public Label QuantityLabel;
    public Label ItemLabel;
    public Label IndexLabel;
    public InventorySlotDragArea DragArea;
    public Panel HoverOverlay;


    private InventorySlotStateMachine StateMachine;


    // LIFECYCLE EVENTS
    
    public override void _Ready()
    {
        QuantityLabel = GetNode<Label>("%QuantityLabel");
        ItemLabel = GetNode<Label>("%ItemLabel");
        ItemIcon = GetNode<TextureRect>("%ItemIcon");
        IndexLabel = GetNode<Label>("%IndexLabel");
        StateMachine = GetNode<InventorySlotStateMachine>("%StateMachine");
        HoverOverlay = GetNode<Panel>("%HoverOverlay");
        DragArea = GetNode<InventorySlotDragArea>("%DragArea");
        DragArea.ParentSlot = this;

        ItemLabel.Text = "";
        QuantityLabel.Text = "0";
        ItemIcon.Texture = null;

        UpdateUI();
        StateMachine.Init(this);

        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
    }
 
    public override void _Input(InputEvent @event)
    {
		StateMachine.OnInput(@event);
    }

    public override void _GuiInput(InputEvent @event)
    {
        StateMachine.OnGuiInput(@event);
    }

    // SIGNAL HANDLERS

	public void OnMouseEntered()
	{
		HoverOverlay.Show();
		StateMachine.OnMouseEntered();
	}

	public void OnMouseExited()
	{
        HoverOverlay.Hide();
		StateMachine.OnMouseExited();
	}


    // SETTERS

    public void SetTargetSlot(InventorySlot slot)
    {
        _targetSlot = slot;
        UpdateUI();

        if (_targetSlot == null) return;

        _targetSlot.OnUpdated += UpdateUI;
    }

    public void UpdateUI()
    {
        if (_targetSlot == null) return;
        if (!IsInstanceValid(this)) return;

        string itemName = "";
        int itemQuantity = 0;
        Texture2D itemIcon = null;

        if (_targetSlot.Item != null)
        {
            itemName = _targetSlot.Item.ItemName;
            itemQuantity = _targetSlot.Quantity;
            itemIcon = _targetSlot.Item.Icon;
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
        
        if(TargetSlot.Item != null && TargetSlot.Item.IsEquipped)
		{
			Set("theme_override_styles/panel", STYLEBOX_ACTIVE);
		}
		else
		{
			Set("theme_override_styles/panel", STYLEBOX_DEFAULT);
		}
    }

}

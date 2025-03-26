namespace Dimworld;

using Godot;
using Godot.Collections;

public partial class InventorySlotUI : Panel
{

    public static readonly StyleBox STYLEBOX_DEFAULT = GD.Load<StyleBox>("res://Inventory/UI/Slot/Styles/InventorySlotUI_Default.tres");
    public static readonly StyleBox STYLEBOX_ACTIVE = GD.Load<StyleBox>("res://Inventory/UI/Slot/Styles/InventorySlotUI_Active.tres");
    public static readonly StyleBox STYLEBOX_SELECTED = GD.Load<StyleBox>("res://Inventory/UI/Slot/Styles/InventorySlotUI_Selected.tres");
    public static readonly StyleBox STYLEBOX_HOVER = GD.Load<StyleBox>("res://Inventory/UI/Slot/Styles/InventorySlotUI_Hover.tres");


    [Export] public InventorySlot TargetSlot {
        get => _targetSlot;
        set => SetTargetSlot(value);
    }
    private InventorySlot _targetSlot;

    public InventoryUI ParentInventory { get; set; }
    public TextureRect ItemIcon;
    public Label QuantityLabel;
    public Label ItemLabel;
    public InventorySlotDragArea DragArea;


    private InventorySlotStateMachine StateMachine;


    // LIFECYCLE EVENTS
    
    public override void _Ready()
    {
        QuantityLabel = GetNode<Label>("%QuantityLabel");
        ItemLabel = GetNode<Label>("%ItemLabel");
        ItemIcon = GetNode<TextureRect>("%ItemIcon");
        StateMachine = GetNode<InventorySlotStateMachine>("%StateMachine");
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
		StateMachine.OnMouseEntered();
	}

	public void OnMouseExited()
	{
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
        }

        if (IsInstanceValid(ItemIcon))
        {
            ItemIcon.Texture = itemIcon;
        }
    }

}

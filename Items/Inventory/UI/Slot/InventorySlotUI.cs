namespace Dimworld.Items.UI;

using Dimworld.Items.Weapons;
using Dimworld.States;
using Godot;
using Godot.Collections;

public partial class InventorySlotUI : Panel
{

    public enum States
    {
        BASE,
        CLICKED,
        DRAGGING
    }

    public enum StyleType
    {
        Default,
        Active,
        Selected
    }


    private static readonly StyleBox STYLEBOX_DEFAULT = GD.Load<StyleBox>("res://Items/Inventory/UI/Slot/Styles/InventorySlotUI_Default.tres");
    private static readonly StyleBox STYLEBOX_ACTIVE = GD.Load<StyleBox>("res://Items/Inventory/UI/Slot/Styles/InventorySlotUI_Active.tres");
    private static readonly StyleBox STYLEBOX_SELECTED = GD.Load<StyleBox>("res://Items/Inventory/UI/Slot/Styles/InventorySlotUI_Selected.tres");


    [Signal] public delegate void OnSlotClickedEventHandler(InventorySlotUI slotUI);
    [Signal] public delegate void OnSlotAlternateClickedEventHandler(InventorySlotUI slotUI);


    [Export] public InventorySlot TargetSlot {
        get => _targetSlot;
        set => SetTargetSlot(value);
    }
    private InventorySlot _targetSlot;
    [Export] public int SlotIndex {
        get => _slotIndex;
        set => SetSlotIndex(value);
    }
    private int _slotIndex = -1;
    [Export] public bool CanBeSelected { get; set; } = true;

    public InventoryUI ParentInventoryUI { get; set; }

    [Export] public TextureRect ItemIcon;
    [Export] public Label QuantityLabel;
    [Export] public Label ItemLabel;
    [Export] public Label IndexLabel;
    [Export] public Panel HoverOverlay;
    [Export] public InventorySlotDragArea DragArea;
    

    public StyleType CurrentStyle = StyleType.Default;    


    private StateMachine<InventorySlotUI> _stateMachine;
    private Dictionary<string, State<InventorySlotUI>> _states = new()
    {
        {States.BASE.ToString(), new InventorySlotBaseState()},
        {States.CLICKED.ToString(), new InventorySlotClickedState()},
        {States.DRAGGING.ToString(), new InventorySlotDraggingState()}
    };


    private void SetTargetSlot(InventorySlot value)
    {
        if (_targetSlot != null)
        {
            _targetSlot.OnUpdated -= UpdateUI;
        }

        _targetSlot = value;

        if (_targetSlot != null)
        {
            _targetSlot.OnUpdated += UpdateUI;
        }

        UpdateUI();
    }

    private void SetSlotIndex(int value)
    {
        _slotIndex = value;
        
        if (IsInstanceValid(IndexLabel))
        {
            IndexLabel.Text = _slotIndex.ToString();
            IndexLabel.Visible = _slotIndex >= 0;
        }
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

    private void UpdateUI()
    {
        if (!IsInstanceValid(this)) return;

        string itemText = "";
        bool canHoldMultiple = false;
        int itemQuantity = 0;
        Texture2D itemIcon = null;
        bool isEquipped = false;

        if (_targetSlot != null && _targetSlot.Item != null)
        {
            itemText = GetLabelText(_targetSlot.Item);
            canHoldMultiple = _targetSlot.Item.MaxStackSize > 1;
            itemQuantity = _targetSlot.Quantity;
            itemIcon = _targetSlot.Item.Icon;
            isEquipped = _targetSlot.Item.IsEquipped;
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
        
        if(isEquipped)
		{
            SetStyle(StyleType.Active);
		}
		else
		{
            SetStyle(StyleType.Default);
		}
    }


    public override void _Ready()
    {
        DragArea.ParentSlot = this;

        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;

        _stateMachine = new StateMachine<InventorySlotUI>(this, _states, States.BASE.ToString());
    }

    public override void _Input(InputEvent @event)
    {
        _stateMachine?.OnInput(@event);
    }

    public override void _GuiInput(InputEvent @event)
    {
        _stateMachine?.OnGuiInput(@event);
    }

	public void OnMouseEntered()
	{
		if (IsInstanceValid(HoverOverlay))
        {
            HoverOverlay.Show();
        }

        _stateMachine?.OnMouseEntered();
	}

	public void OnMouseExited()
	{
        if (IsInstanceValid(HoverOverlay))
        {
            HoverOverlay.Hide();
        }

        _stateMachine?.OnMouseExited();
	}

    public void SetStyle(StyleType type)
    {
        if (!IsInstanceValid(this)) return;
        if (CurrentStyle == type) return;

        switch (type)
        {
            case StyleType.Default:
                Set("theme_override_styles/panel", STYLEBOX_DEFAULT);
                CurrentStyle = StyleType.Default;
                break;
            case StyleType.Active:
                Set("theme_override_styles/panel", STYLEBOX_ACTIVE);
                CurrentStyle = StyleType.Active;
                break;
            case StyleType.Selected:
                Set("theme_override_styles/panel", STYLEBOX_SELECTED);
                CurrentStyle = StyleType.Selected;
                break;
        }
    }

}

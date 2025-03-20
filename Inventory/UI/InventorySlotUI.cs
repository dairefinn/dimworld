namespace Dimworld;

using Godot;

public partial class InventorySlotUI : Panel
{

    [Signal] public delegate void OnClickedEventHandler();


    [Export] public InventorySlot TargetSlot {
        get => _targetSlot;
        set => SetTargetSlot(value);
    }
    private InventorySlot _targetSlot;


    private Label QuantityLabel;
    private Label ItemLabel;
    private TextureRect ItemIcon;


    public override void _Ready()
    {
        base._Ready();

        QuantityLabel = GetNode<Label>("%QuantityLabel");
        ItemLabel = GetNode<Label>("%ItemLabel");
        ItemIcon = GetNode<TextureRect>("%ItemIcon");

        ItemLabel.Text = "";
        QuantityLabel.Text = "0";
        ItemIcon.Texture = null;

        UpdateUI();
    }

    public override void _GuiInput(InputEvent @event)
    {
        base._GuiInput(@event);
        
        if (@event is InputEventMouseButton mouseButtonEvent)
        {
            // TODO: Should I be hardcoding inputs here or using an input action?
            if (mouseButtonEvent.ButtonIndex == MouseButton.Left && mouseButtonEvent.Pressed)
            {
                EmitSignal(SignalName.OnClicked);
            }
        }
    }


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

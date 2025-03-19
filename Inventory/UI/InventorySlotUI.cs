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
    }

    public override void _GuiInput(InputEvent @event)
    {
        base._GuiInput(@event);
        
        if (@event is InputEventMouseButton mouseButtonEvent)
        {
            if (mouseButtonEvent.ButtonIndex == MouseButton.Left)
            {
                EmitSignal(SignalName.OnClicked);
            }
        }
    }

    public void SetTargetSlot(InventorySlot slot)
    {
        _targetSlot = slot;

        if (_targetSlot == null) return;

        _targetSlot.OnUpdated += UpdateUI;
    }

    private void UpdateUI()
    {
        if (_targetSlot == null) return;
        if (_targetSlot.IsEmpty) return;

        ItemLabel.Text = _targetSlot.Item.ItemName;
        QuantityLabel.Text = _targetSlot.Quantity.ToString();
        ItemIcon.Texture = _targetSlot.Item.Icon;
    }

}

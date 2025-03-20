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
    [Export] public bool IsSelected {
        get => _isSelected;
        set => SetSelected(value);
    }
    private bool _isSelected;

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

    private void SetSelected(bool isSelected)
    {
        GD.Print("Setting selected to " + isSelected);
        _isSelected = isSelected;
        // TODO: Show something in the UI to indicate that the slot is selected
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

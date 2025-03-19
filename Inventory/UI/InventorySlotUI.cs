namespace Dimworld;

using Godot;

public partial class InventorySlotUI : Panel
{

    [Signal] public delegate void OnClickedEventHandler();

    [Export] public InventorySlot TargetSlot;

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
        
        TargetSlot.OnUpdated += UpdateUI;
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


    public void UpdateUI()
    {
        if (TargetSlot == null) return;
        if (TargetSlot.IsEmpty) return;

        ItemLabel.Text = TargetSlot.Item.ItemName;
        QuantityLabel.Text = TargetSlot.Quantity.ToString();
        ItemIcon.Texture = TargetSlot.Item.Icon;
    }

}

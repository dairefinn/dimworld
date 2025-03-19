namespace Dimworld;

using Godot;

public partial class InventorySlotUI : Panel
{

    [Export] public InventorySlot TargetSlot;

    private Label QuantityLabel;
    private Label ItemLabel;


    public override void _Ready()
    {
        base._Ready();

        QuantityLabel = GetNode<Label>("%QuantityLabel");
        ItemLabel = GetNode<Label>("%ItemLabel");

        ItemLabel.Text = "";
        QuantityLabel.Text = "0";

        UpdateUI();
        
        TargetSlot.OnUpdated += UpdateUI;
    }

    public void UpdateUI()
    {
        if (TargetSlot == null) return;
        if (TargetSlot.IsEmpty) return;

        ItemLabel.Text = TargetSlot.Item.ItemName;
        QuantityLabel.Text = TargetSlot.Quantity.ToString();
    }

}

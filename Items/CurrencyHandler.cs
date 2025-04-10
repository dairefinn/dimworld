namespace Dimworld.Items;

using Godot;


public partial class CurrencyHandler : Node
{
    public static CurrencyHandler Instance { get; private set; }

    [Export] public int Money { get; private set; } = 0;
    [Export] public int MaxMoney { get; private set; } = 10000;

    public override void _Ready()
    {
        Instance = this;
    }

    public void AddMoney(int amount)
    {
        if (amount < 0) return;
        Money += amount;
        if (Money > MaxMoney) Money = MaxMoney;
    }

    public void RemoveMoney(int amount)
    {
        if (amount < 0) return;
        Money -= amount;
        if (Money < 0) Money = 0;
    }

    public bool HasMoney(int amount)
    {
        return Money >= amount;
    }

}
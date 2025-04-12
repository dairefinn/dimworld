namespace Dimworld.Core.Currency;

using Godot;


// TODO: Each agent should have their own count of currency instead of a global one
/// <summary>
/// This class is responsible for managing the currency in the game.
/// It's just a placeholder for now so that I can test trading.
/// </summary>
public partial class CurrencyController : Node
{

    [Export] public int Money { get; private set; } = 0;
    [Export] public int MaxMoney { get; private set; } = 10000;


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
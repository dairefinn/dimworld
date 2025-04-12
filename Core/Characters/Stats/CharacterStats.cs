namespace Dimworld.Core.Characters.Stats;

using Dimworld.Core.Developer;
using Godot;


/// <summary>
/// A class that handles the stats of an agent.
/// </summary>
[GlobalClass]
public partial class CharacterStats : Resource
{

    [Signal] public delegate void HealthChangedEventHandler();
    [Signal] public delegate void StaminaChangedEventHandler();


    [Export] public float MaxHealth {
        get => _maxHealth;
        set => SetMaxHealth(value);
    }
    private float _maxHealth = 100;
    [Export] public float MaxStamina {
        get => _maxStamina;
        set => SetMaxStamina(value);
    }
    private float _maxStamina = 100;


    public float Health {
        get => _health;
        set => SetHealth(value);
    }
    private float _health;
    public float Stamina {
        get => _stamina;
        set => SetStamina(value);
    }
    private float _stamina;


    public CharacterStats()
    {

    }
    
    public CharacterStats(CharacterStats source)
    {
        _maxHealth = source._maxHealth;
        _maxStamina = source._maxStamina;
        _health = source._maxHealth;
        _stamina = source._maxStamina;
    }


    // SETTERS

    public void SetHealth(float value)
    {
        _health = Mathf.Clamp(value, 0, MaxHealth);
        EmitSignal(SignalName.HealthChanged);
    }

    public void SetStamina(float value)
    {
        _stamina = Mathf.Clamp(value, 0, MaxStamina);
        EmitSignal(SignalName.StaminaChanged);
    }

    public void SetMaxHealth(float value)
    {
        _maxHealth = Mathf.Max(0, value);
        SetHealth(Health);
    }

    public void SetMaxStamina(float value)
    {
        _maxStamina = Mathf.Max(0, value);
        SetStamina(Stamina);
    }


    // METHODS

    public float GetHealthPercent()
    {
        if (MaxHealth <= 0) return 0;
        if (Health <= 0) return 0;
        if (Health == MaxHealth) return 100;

        return Health / MaxHealth * 100;
    }

    public float GetStaminaPercent()
    {
        if (MaxStamina <= 0) return 0;
        if (Stamina <= 0) return 0;
        if (Stamina == MaxStamina) return 100;

        return Stamina / MaxStamina * 100;
    }

    public void Heal(float amount = 0)
    {
        if (amount <= 0) return;

        DeveloperConsole.Print($"Healing {amount} health");
        SetHealth(Health + amount);
    }

    public void RecoverStamina(float amount = 0)
    {
        if (amount < 0) return;
        if (amount == 0)
        {
            amount = MaxStamina - Stamina;
        }

        SetStamina(Stamina + amount);
    }

    public void TakeDamage(float amount = 0)
    {
        if (amount <= 0) return;

        DeveloperConsole.Print($"Taking {amount} damage");
        SetHealth(Health - amount);
    }

    public void UseStamina(float amount = 0)
    {
        if (amount < 0) return;
        if (amount == 0)
        {
            amount = MaxStamina - Stamina;
        }

        SetStamina(Stamina - amount);
    }

}

namespace Dimworld;

using Godot;


[GlobalClass]
public partial class AgentStats : Resource
{
    // TODO: Hunger? Thirst?

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


    public AgentStats()
    {

    }
    
    public AgentStats(AgentStats source)
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
        if (Health == MaxHealth) return 1;

        return Health / MaxHealth;
    }

    public float GetStaminaPercent()
    {
        if (MaxStamina <= 0) return 0;
        if (Stamina <= 0) return 0;
        if (Stamina == MaxStamina) return 1;

        return Stamina / MaxStamina;
    }

}

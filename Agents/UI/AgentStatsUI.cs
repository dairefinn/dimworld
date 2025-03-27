namespace Dimworld;

using Godot;


public partial class AgentStatsUI : PanelContainer
{

    [Export] public AgentStats Stats {
        get => _stats;
        set => SetStats(value);
    }
    private AgentStats _stats;

    private ColorRect barHealth;
    private ColorRect barStamina;

    private float barMaxWidth = 20f;


    public override void _Ready()
    {
        barHealth = GetNode<ColorRect>("%BarHealth");
        barStamina = GetNode<ColorRect>("%BarStamina");

        barMaxWidth = barHealth.GetParent<VBoxContainer>().Size.X;

        UpdateUI();
    }


    public void SetStats(AgentStats value)
    {
        // Unregister old stats
        if (_stats != null)
        {
            _stats.HealthChanged -= UpdateUI;
            _stats.StaminaChanged -= UpdateUI;
        }

        // Update value
        _stats = value;

        // Register new stats
        if (_stats != null)
        {
            _stats.HealthChanged += UpdateUI;
            _stats.StaminaChanged += UpdateUI;
        }

        UpdateUI();
    }


    public void UpdateUI()
    {
        if (Stats == null) return;

        if (IsInstanceValid(barHealth))
        {
            float healthWidth = Stats.GetHealthPercent() * barMaxWidth;
            barHealth.Size = new Vector2(healthWidth, barHealth.Size.Y);
        }

        if (IsInstanceValid(barStamina))
        {
            float staminaWidth = Stats.GetStaminaPercent() * barMaxWidth;
            barStamina.Size = new Vector2(staminaWidth, barStamina.Size.Y);
        }
    }


}

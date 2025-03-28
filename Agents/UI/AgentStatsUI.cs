namespace Dimworld;

using Godot;


public partial class AgentStatsUI : PanelContainer
{

    [Export] public AgentStats Stats {
        get => _stats;
        set => SetStats(value);
    }
    private AgentStats _stats;

    private ProgressBar barHealth;
    private ProgressBar barStamina;

    private float barMaxWidth = 20f;


    public override void _Ready()
    {
        barHealth = GetNode<ProgressBar>("%BarHealth");
        barStamina = GetNode<ProgressBar>("%BarStamina");

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
            GD.Print($"Health: {Stats.GetHealthPercent()}");
            barHealth.Value = Stats.GetHealthPercent();
        }

        if (IsInstanceValid(barStamina))
        {
            barStamina.Value = Stats.GetStaminaPercent();
        }
    }


}

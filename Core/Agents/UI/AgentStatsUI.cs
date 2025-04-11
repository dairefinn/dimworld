namespace Dimworld;

using Godot;

public partial class AgentStatsUI : PanelContainer
{
    [Export] public AgentStats Stats {
        get => _stats;
        set => SetStats(value);
    }
    private AgentStats _stats;
    [Export] public float FadeOutTime = 2.5f;

    private ProgressBar barHealth;
    private ProgressBar barStamina;

    private SceneTreeTimer fadeTimer;

    private Tween tweenHealth;
    private Tween tweenStamina;
    private Tween tweenVisibility;
    private bool initialized = false;


    // LIFECYCLE EVENTS

    public override void _Ready()
    {
        barHealth = GetNode<ProgressBar>("%BarHealth");
        barStamina = GetNode<ProgressBar>("%BarStamina");

        // This is a bit of a hack but it prevents the initial stats update from showing the stats UI
        GetTree().CreateTimer(0.5f).Timeout += () =>
        {
            initialized = true;
        };
    }


    // SETTERS

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


    // UI UPDATES

    public async void UpdateUI()
    {
        if (!IsNodeReady())
        {
            await ToSignal(this, "ready");
        }

        if (Stats == null) return;

        if (IsInstanceValid(barHealth))
        {
            tweenHealth?.Kill();
            tweenHealth = CreateTween().SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.InOut);
            tweenHealth.TweenProperty(barHealth, "value", Stats.GetHealthPercent(), 0.5f);
        }

        if (IsInstanceValid(barStamina))
        {
            tweenStamina?.Kill();
            tweenStamina = CreateTween().SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.InOut);
            tweenStamina.TweenProperty(barStamina, "value", Stats.GetStaminaPercent(), 0.5f);
        }

        // Card shows and then fades out over time when the stats change
        if (!initialized) return;

        Show();
        Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 1);
        
        if (fadeTimer != null)
        {
            fadeTimer = null;
        }

        fadeTimer = GetTree().CreateTimer(FadeOutTime);
        fadeTimer.Timeout += () => StartFadeOut();
    }

    private void StartFadeOut()
    {
        tweenVisibility?.Kill();
        tweenVisibility = CreateTween().SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
        tweenVisibility.TweenProperty(this, "modulate", new Color(Modulate.R, Modulate.G, Modulate.B, 0), 0.5f);
        tweenVisibility.Finished += () => Hide(); // Hide the UI after the fade-out completes
    }
}
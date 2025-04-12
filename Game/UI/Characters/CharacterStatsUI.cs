namespace Dimworld.UI.Characters;

using Dimworld.Core.Characters.Stats;
using Dimworld.Core.Factions;
using Godot;


public partial class CharacterStatsUI : Control
{

    [Export] public NodePath StatsSource {
        get => _statsSource;
        set {
            _statsSource = value;
            OnSourceChanged();
        }
    }
    private NodePath _statsSource;
    [Export] public float FadeOutTime = 2.5f;


    private CharacterStats _stats;
    private Faction _faction;
    private ProgressBar _barHealth;
    private ProgressBar _barStamina;
    private TextureRect _factionIcon;
    private SceneTreeTimer _fadeTimer;
    private Tween _tweenHealth;
    private Tween _tweenStamina;
    private Tween _tweenVisibility;
    private bool _initialized = false;


    // LIFECYCLE EVENTS

    public override void _Ready()
    {
        _barHealth = GetNode<ProgressBar>("%BarHealth");
        _barStamina = GetNode<ProgressBar>("%BarStamina");
        _factionIcon = GetNode<TextureRect>("%FactionIcon");

        // This is a bit of a hack but it prevents the initial stats update from showing the stats UI
        GetTree().CreateTimer(0.5f).Timeout += () =>
        {
            _initialized = true;
        };

        CallDeferred(MethodName.OnSourceChanged);
    }


    // SETTERS

    public void OnSourceChanged()
    {
        if (StatsSource == null) return;

        Node statsNode = GetNodeOrNull<Node>(StatsSource);
        if (statsNode == null) return;

        CharacterStats statsFromNode = CharacterStats.GetStatsFor(statsNode);
        Faction factionFromNode = Faction.GetAffiliationFor(statsNode);

        SetStats(statsFromNode);
        SetFaction(factionFromNode);
    }

    public void SetStats(CharacterStats value)
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

    public void SetFaction(Faction value)
    {
        // Unregister old faction
        if (_faction != null)
        {
            _faction.Changed -= UpdateUI;
        }

        // Update value
        _faction = value;

        // Register new faction
        if (_faction != null)
        {
            _faction.Changed += UpdateUI;
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

        if (_stats == null)
        {
            Hide();
            return;
        }

        if (IsInstanceValid(_barHealth))
        {
            _tweenHealth?.Kill();
            _tweenHealth = CreateTween().SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.InOut);
            _tweenHealth.TweenProperty(_barHealth, "value", _stats.GetHealthPercent(), 0.5f);
        }

        if (IsInstanceValid(_barStamina))
        {
            _tweenStamina?.Kill();
            _tweenStamina = CreateTween().SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.InOut);
            _tweenStamina.TweenProperty(_barStamina, "value", _stats.GetStaminaPercent(), 0.5f);
        }

        if (IsInstanceValid(_factionIcon))
        {
            if (_faction != null)
            {
                _factionIcon.Texture = _faction?.Icon;
                _factionIcon.Show();
            }
            else
            {
                _factionIcon.Texture = null;
                _factionIcon.Hide();
            }
        }

        // Card shows and then fades out over time when the stats change
        if (!_initialized) return;

        Show();
        Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 1);
        
        if (_fadeTimer != null)
        {
            _fadeTimer = null;
        }

        _fadeTimer = GetTree().CreateTimer(FadeOutTime);
        _fadeTimer.Timeout += () => StartFadeOut();
    }

    private void StartFadeOut()
    {
        _tweenVisibility?.Kill();
        _tweenVisibility = CreateTween().SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
        _tweenVisibility.TweenProperty(this, "modulate", new Color(Modulate.R, Modulate.G, Modulate.B, 0), 0.5f);
        _tweenVisibility.Finished += () => Hide(); // Hide the UI after the fade-out completes
    }
}
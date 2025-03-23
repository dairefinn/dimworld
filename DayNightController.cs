namespace Dimworld;

using System;
using Godot;


// TODO: Most of this is ripped from our FarmH& project, need to adapt it to this one.
public partial class DayNightController : DirectionalLight2D
{
    
	public static DayNightController Instance { get; private set; }


	[Export] public Color NightColor { get; set; } = new Color(0.11f, 0, 0.36f);

	[Export] public bool EnableDayNightCycle { get; set; } = true;
	[Export] public float TimePassedPerTick { get; set; } = 0.1f;

	[Export] public int CurrentDay { get; set; } = 1;

	// 0 = Dawn
	// 25 = Midday
	// 50 = Dusk
	// 75 = Midnight
	// 100 = Dawn
	[Export(PropertyHint.Range, "0,100")] public float PercentageOfDay {
        get => _percentageOfDay;
        set => SetPercentageOfDay(value);
    }
    private float _percentageOfDay = 0;

	public DayNightController() {
		Instance = this;
	}

	public override void _Ready() {
		SetPercentageOfDay(PercentageOfDay);
	}

	public override void _Process(double delta) {
		base._Process(delta);
		UpdateTime(delta);
	}

	public void UpdateTime(double delta) {
		if (EnableDayNightCycle) {
			IncrementTime(PercentageOfDay, TimePassedPerTick * (float)delta);
		}
	}

	private void IncrementTime(float startPercentageOfDay, float timePassedPerTick) {
		float newPerentageOfDay = startPercentageOfDay;
		newPerentageOfDay += timePassedPerTick;
		newPerentageOfDay = Math.Clamp(newPerentageOfDay, 0, 125);
		if (newPerentageOfDay >= 100)
		{
			newPerentageOfDay -= 100;
			CurrentDay++;
		}

        PercentageOfDay = newPerentageOfDay;
	}

    // TODO: Since this is a 2D game, we don't need to rotate the sun but this might set the shadow direction?
	private void UpdateSunPosition(float percentageOfDay) {
		// 0 is start of day, 25 is midday. 50 is end of day. 75 is midnight. 100 is start of day.
		// float percentageOfDayDecimal = percentageOfDay / 100;
		// float percentageOfRotation = percentageOfDayDecimal * 360;
		// Sun.Transform.Rotation = new Vector3(percentageOfRotation, 0, 0);
	}

	private void UpdateSkyColour(float percentageOfDay) {
		float lerpAdjusted;
		if (percentageOfDay < 25) { // 0.5 to 1
			lerpAdjusted = Utils.Lerp(0.5f, 1, percentageOfDay / 25);
		} else if (percentageOfDay < 50) { // 1 to 0.5
			lerpAdjusted = Utils.Lerp(1, 0.5f, (percentageOfDay - 25) / 25);
		} else if (percentageOfDay < 75) { // 0.5 to 0
			lerpAdjusted = Utils.Lerp(0.5f, 0, (percentageOfDay - 50) / 25);
		} else { // 0 to 0.5
			lerpAdjusted = Utils.Lerp(0, 0.5f, (percentageOfDay - 75) / 25);
		}

        // Color = Utils.Lerp(NightColor, NightColor, lerpAdjusted);

        // 0.1 at day (25%)
        // 1 at night (75%)
        
        Energy = Utils.Lerp(0.0f, 1.0f, 1 - lerpAdjusted);
	}
    
    private void SetPercentageOfDay(float value) {
        _percentageOfDay = value;
        UpdateSunPosition(_percentageOfDay);
        UpdateSkyColour(_percentageOfDay);
    }

	// public SaveData GetSaveData()
	// {
	// 	return new DayNightCycleSaveData(this);
    // }

	// public void LoadFromSaveData(DayNightCycleSaveData dayNightCycleSaveData) {
	// 	CurrentDay = dayNightCycleSaveData.CurrentDay;
	// 	PercentageOfDay = dayNightCycleSaveData.PercentageOfDay;
    // }

}

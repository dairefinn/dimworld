namespace Dimworld.Modifiers;

using Godot;
using Godot.Collections;


public partial class ModifierHandler : Node2D
{

	[Export] public Array<Modifier> Modifiers { get; set; } = [];


	// PROCESSING

	public void ProcessConditions(double delta)
	{
		ProcessModifiersFor(Modifier.ProcessingType.Frame, delta);
	}

	public void PhysicsProcessConditions(double delta)
	{
		ProcessModifiersFor(Modifier.ProcessingType.Physics, delta);
	}

	private void ProcessModifiersFor(Modifier.ProcessingType processingType, double delta)
	{
		Array<Modifier> modifiersProcessing = [..Modifiers];

		foreach (Modifier modifier in modifiersProcessing)
		{
			if (modifier.ProcessOn == processingType)
			{
				bool isStillActive = modifier.Process(delta);

				if (!isStillActive)
				{
					Remove(modifier);
				}
			}
		}
	}


	// ADDING/REMOVING/GETTING

	public void Add(Modifier modifier)
	{
		Modifier existingModifier = GetByKey(modifier.Key);
		if (existingModifier != null)
		{
			Modifiers.Remove(existingModifier);
		}

		Modifiers.Add(modifier);
		modifier.OnAdded(this);
	}

	public void Remove(Modifier modifier)
	{
		if (!Modifiers.Contains(modifier))
		{
			return;
		}

		Modifiers.Remove(modifier);
		modifier.OnRemoved(this);
	}

	public Modifier GetByKey(string key)
	{
		foreach (var modifier in Modifiers)
		{
			if (modifier.Key == key)
			{
				return modifier;
			}
		}

		return null;
	}

	public void RemoveByKey(string key)
	{
		Modifier modifier = GetByKey(key);
		if (modifier != null)
		{
			Modifiers.Remove(modifier);
		}
	}

	public Array<T> GetAllByType<[MustBeVariant] T>() where T : Modifier
	{
		Array<T> modifiers = [];

		foreach (var modifier in Modifiers)
		{
			if (modifier is T typedModifier)
			{
				modifiers.Add(typedModifier);
			}
		}

		return modifiers;
	}

}

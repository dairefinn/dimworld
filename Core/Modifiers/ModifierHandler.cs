namespace Dimworld.Core.Modifiers;

using Dimworld.Core.Developer;
using Godot;
using Godot.Collections;


/// <summary>
/// A default instance of the modifier handler. This is used to manage the different modifiers that are applied to an entity.
/// It doesn't make any actual changes to the entity, it just adds/removes them and passes lifecycle events on to any that are applied.
/// </summary>
public partial class ModifierHandler : Node2D
{

	public Array<Modifier> Modifiers { get; set; } = [];


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


	/// <summary>
	/// Processes all modifiers that are set to process on the frame.
	/// This should be called from the _Process method of the node that this is attached to.
	/// </summary>
	/// <param name="delta">The delta time since the last frame.</param>
	public void ProcessConditions(double delta)
	{
		ProcessModifiersFor(Modifier.ProcessingType.Frame, delta);
	}

	/// <summary>
	/// Processes all modifiers that are set to process on the physics frame.
	/// This should be called from the _PhysicsProcess method of the node that this is attached to.
	/// </summary>
	/// <param name="delta">The delta time since the last physics frame.</param>
	public void PhysicsProcessConditions(double delta)
	{
		ProcessModifiersFor(Modifier.ProcessingType.Physics, delta);
	}

	/// <summary>
	/// Adds a modifier to the handler. If a modifier with the same key already exists, it will be removed first.
	/// </summary>
	/// <param name="modifier">The modifier to add.</param>
	public void Add(Modifier modifier)
	{
		Modifier existingModifier = GetByKey(modifier.Key);
		if (existingModifier != null)
		{
			Modifiers.Remove(existingModifier);
		}

		DeveloperConsole.Print($"Adding modifier {modifier} to {this} with key {modifier.Key}");
		Modifiers.Add(modifier);
		modifier.OnAdded(this);
	}

	/// <summary>
	/// Removes a modifier from the handler, if it exists.
	/// </summary>
	/// <param name="modifier">The modifier to remove.</param>
	public void Remove(Modifier modifier)
	{
		if (!Modifiers.Contains(modifier))
		{
			return;
		}

		Modifiers.Remove(modifier);
		modifier.OnRemoved(this);
	}

	/// <summary>
	/// Gets a modifier by its key.
	/// </summary>
	/// <param name="key">The key of the modifier to get.</param>
	/// <returns>The modifier with the specified key, or null if it doesn't exist.</returns>
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

	/// <summary>
	/// Removes a modifier by its key, if it exists.
	/// </summary>
	/// <param name="key">The key of the modifier to remove.</param>
	public void RemoveByKey(string key)
	{
		Modifier modifier = GetByKey(key);
		if (modifier != null)
		{
			Modifiers.Remove(modifier);
		}
	}

	/// <summary>
	/// Gets all modifiers of a specific type.
	/// </summary>
	/// <typeparam name="T">The type of modifier to get.</typeparam>
	/// <returns>An array of modifiers of the specified type.</returns>
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

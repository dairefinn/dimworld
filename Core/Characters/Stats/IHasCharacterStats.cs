namespace Dimworld.Core.Characters.Stats;


/// <summary>
/// Any agent that has stats should implement this interface.
/// </summary>
public interface IHasCharacterStats
{

    public CharacterStats Stats { get; set; }

    public void OnDeath();

}
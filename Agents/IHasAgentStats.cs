namespace Dimworld;

using Godot;


public interface IHasAgentStats
{

    public AgentStats Stats { get; set; }

    public void OnDeath();

}
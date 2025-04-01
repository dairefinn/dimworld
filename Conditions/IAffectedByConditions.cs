using Godot.Collections;

namespace Dimworld;


public interface IAffectedByConditions
{

    // public Array<Condition> Conditions { get; set; }

    public bool AddCondition(Condition condition);

    public bool RemoveCondition(Condition condition);

    public bool HasCondition(Condition condition);

    public void ProcessConditions(double delta);

    public void PhysicsProcessConditions(double delta);

}

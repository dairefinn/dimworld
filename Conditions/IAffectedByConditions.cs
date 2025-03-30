using Godot.Collections;

namespace Dimworld;


public interface IAffectedByConditions
{

    public Array<Condition> Conditions { get; set; }

    public void AddCondition(Condition condition);

    public void RemoveCondition(Condition condition);

    public void ProcessConditions(double delta);

}

namespace Dimworld;


public interface IConditionHandler
{

    public bool AddCondition(Condition condition);

    public bool RemoveCondition(Condition condition);

	public bool HasCondition(Condition condition);

    public void ProcessConditions(IAffectedByConditions parent, double delta);

	public void PhysicsProcessConditions(IAffectedByConditions parent, double delta);

}

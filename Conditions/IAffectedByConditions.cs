namespace Dimworld;


public interface IAffectedByConditions
{

    /// <summary>
    /// Returns the condition handler for this object. If you don't feel the need to keep the logic in a separate class, return `this`.
    /// </summary>
    /// <returns></returns>
    public IConditionHandler GetConditionHandler();

}

namespace Dimworld.Core.Characters;

using Godot;


public interface IHasNavigation
{

    public void NavigateTo(Vector2 target);

    public bool IsTargetReached();

    public bool CanReachPoint(Vector2 targetPoint);

    public bool IsTargetingPoint(Vector2 targetPoint);

    public Vector2 GetTargetPosition();

}

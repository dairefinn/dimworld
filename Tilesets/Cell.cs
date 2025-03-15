namespace Dimworld;

using Godot;

/// <summary>
/// Abstract class for any logic common to all cells in the world grid.
/// </summary>
public abstract partial class Cell : Node2D
{

    public CellType CellType { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }

    
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        Vector2 position = Position / 16;
        Row = (int)position.Y;
        Column = (int)position.X;
    }

    /// <summary>
    /// Texture is 16x16 so we can divide the coordinates by 16 to get the cell coordinates.
    /// </summary>
    /// <returns></returns>
    public Vector2 GetCoodinates()
    {
        return new Vector2(Position.X / 16, Position.Y / 16);
    }

}

namespace Dimworld;

using Godot;
using System;
using System.Linq;

public partial class MapHandler : Node2D
{

	// FIXME: All tilemap layers are now children of the map handler, so I will need to add a layer to the properties of each method.

	/// <summary>
	/// Get a specific cell by row and column.
	/// </summary>
	/// <param name="row">The row the cell is in</param>
	/// <param name="column">The column the cell is in</param>
	/// <returns>The cell at the specified row and column or null if no cell is found.</returns>
	public Cell GetCell(int row, int column)
	{
		return GetAllCells().FirstOrDefault(cell => cell.Row == row && cell.Column == column);
	}

	/// <summary>
	/// Get a specific cell by it's coordinates.
	/// </summary>
	/// <param name="position">A Vector2 where X is the column and Y is the row</param>
	/// <returns>The cell at the specified position or null if no cell is found.</returns>
	public Cell GetCell(Vector2 position)
	{
		return GetCell((int)position.Y, (int)position.X);
	}

	/// <summary>
	/// Get all cells in the world grid.
	/// </summary>
	/// <returns>An array of all cells in the world grid.</returns>
	public Cell[] GetAllCells()
	{
		return GetChildren().OfType<Cell>().ToArray();
	}

	/// <summary>
	/// Get all cells in a specific row.
	/// </summary>
	/// <param name="row">The row to get cells from</param>
	/// <returns>An array of all cells in the specified row.</returns>
	public Cell[] GetAllCellsInRow(int row)
	{
		return GetAllCells().Where(cell => cell.Row == row).ToArray();
	}

	/// <summary>
	/// Get all cells in a specific column.
	/// </summary>
	/// <param name="column">The column to get cells from</param>
	/// <returns>An array of all cells in the specified column.</returns>
	public Cell[] GetAllCellsInColumn(int column)
	{
		return GetAllCells().Where(cell => cell.Column == column).ToArray();
	}

	/// <summary>
	/// Get all cells adjacent to a specific cell.
	/// </summary>
	/// <param name="cell">The cell to get neighbours for</param>
	/// <returns>An array of all cells adjacent to the specified cell.</returns>
	public Cell[] GetNeighbours(Cell cell)
	{
		return GetAllCells().Where(c => Math.Abs(c.Row - cell.Row) <= 1 && Math.Abs(c.Column - cell.Column) <= 1).ToArray();
	}

}

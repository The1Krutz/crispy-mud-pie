using Godot;
using Godot.Collections;

/// <summary>
/// Draws the unit's path using an autotile
/// </summary>
public class UnitPath : TileMap {
  // Exports
  [Export]
  public Grid grid;

  // Public Fields
  public Array<Vector2> CurrentPath;

  // Private Fields
  private Pathfinder _pathfinder;

  // Public Functions

  /// <summary>
  /// Creates a new PathFinder that uses the AStar algorithm we use to find a path between two cells
  /// among the `walkable_cells`. We'll call this function every time the player selects a unit
  /// </summary>
  public void Initialize(Array<Vector2> walkableCells) {
    _pathfinder = new Pathfinder(grid, walkableCells);
  }

  /// <summary>
  /// Finds and draws the path between `cell_start` and `cell_end`.
  /// </summary>
  public void Draw(Vector2 cellStart, Vector2 cellEnd) {
    Clear();
    CurrentPath = _pathfinder.CalculatePointPath(cellStart, cellEnd);

    foreach (var cell in CurrentPath) {
      SetCellv(cell, 0);
    }

    UpdateBitmaskRegion();
  }

  /// <summary>
  /// Stops drawing, clearing the drawn path and the `_pathfinder`.
  /// </summary>
  public void Stop() {
    _pathfinder = null;
    Clear();
  }
}
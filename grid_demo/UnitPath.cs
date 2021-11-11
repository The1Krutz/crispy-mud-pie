using System.Linq;
using Godot;
using Godot.Collections;

/// <summary>
/// Draws the unit's path using an autotile
/// </summary>
public class UnitPath : TileMap {
  // Exports
  [Export]
  public Grid grid;

  // Private Fields
  private Pathfinder _pathfinder;
  private Array<Vector2> currentPath;

  // Lifecycle Hooks
  public override void _Ready() {
    // debug!
    Vector2 rectStart = new Vector2(4, 4);
    Vector2 rectEnd = new Vector2(10, 8);

    Array<Vector2> points = new Array<Vector2>();

    for (int x = 0; x < rectEnd.x - rectStart.x + 1; x++) {
      for (int y = 0; y < rectEnd.y - rectStart.y + 1; y++) {
        points.Add(rectStart + new Vector2(x, y));
      }
    }

    Initialize(points);
    Draw(rectStart, new Vector2(8, 7));
    // !debug
  }

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
    currentPath = _pathfinder.CalculatePointPath(cellStart, cellEnd);

    foreach (var cell in currentPath) {
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
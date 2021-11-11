using Godot;

/// <summary>
/// Represents a grid with its size, the size of each cell in pixels, and some helper functions to
/// calculate and convert coordinates. It is meant to be shared between game objects that need
/// access to those values.
/// </summary>
public class Grid : Resource {
  // grid size in rows and columns
  [Export]
  public Vector2 size = new Vector2(20, 20);

  // size of each cell in pixels
  [Export]
  public Vector2 cellSize = new Vector2(80, 80);

  // half a cell size. This gets used often enough to be worth precalculating
  private Vector2 _halfCellSize;

  public Grid() {
    _halfCellSize = cellSize / 2;
  }

  /// <summary>
  /// Returns the position of a cell's center in pixels. We'll place units and have them move
  /// through cells using this function.
  /// </summary>
  public Vector2 CalculateMapPosition(Vector2 gridPosition) {
    return (gridPosition * cellSize) + _halfCellSize;
  }

  /// <summary>
  /// Returns the coordinates of the cell on the grid given a position on the map. This is the
  /// complementary of `calculate_map_position()` above. When designing a level, you'll place units
  /// visually in the editor. We'll use this function to find the grid coordinates they're placed
  /// on, and call `calculate_map_position()` to snap them to the cell's center.
  /// </summary>
  public Vector2 CalculateGridCoordinates(Vector2 mapPosition) {
    return (mapPosition / cellSize).Floor();
  }

  /// <summary>
  /// returns true if cellCoordinates are within the grid. This method (together with Clamp) allows
  /// us to ensure the cursor or units can never go outside the map's limits
  /// </summary>
  public bool IsWithinBounds(Vector2 cellCoordinates) {
    return cellCoordinates.x >= 0 && cellCoordinates.x < size.x && cellCoordinates.y >= 0 && cellCoordinates.y < size.y;
  }

  /// <summary>
  /// Clamps gridPosition to fit within the grid's boundaries.
  /// This is a clamp function designed specifically for our grid coordinates.
  /// The Vector2 class comes with its `Vector2.clamp()` method, but it doesn't work the same way:
  /// it limits the vector's length instead of clamping each of the vector's components individually.
  /// That's why we need to code a new method.
  /// </summary>
  public Vector2 Clamp(Vector2 gridPosition) {
    Vector2 temp = new Vector2(gridPosition.x, gridPosition.y);
    temp.x = Mathf.Clamp(temp.x, 0, size.x - 1);
    temp.y = Mathf.Clamp(temp.y, 0, size.y - 1);
    return temp;
  }

  /// <summary>
  /// Given Vector2 coordinates, calculates and returns the corresponding integer index.
  /// You can use this function to convert 2D coordinates to a 1D array's indices.
  /// There are two cases where you need to convert coordinates like so:
  /// 1. We'll need it for the AStar algorithm, which requires a unique index for each point on the graph it uses to find a path.
  /// 2. You can use it for performance. More on that below.
  /// </summary>
  public int AsIndex(Vector2 cell) {
    return (int)(cell.x + (size.x * cell.y));
  }
}

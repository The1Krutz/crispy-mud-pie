using Godot;

/**
 * Represents a grid with its size, the size of each cell in pixels, and some helper functions to
 * calculate and convert coordinates. It is meant to be shared between game objects that need access
 * to those values.
 */
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

  /**
   * Returns the position of a cell's center in pixels. We'll place units and have them move through
   * cells using this function.
   */
  public Vector2 CalculateMapPosition(Vector2 gridPosition) {
    return (gridPosition * cellSize) + _halfCellSize;
  }

  /**
   * Returns the coordinates of the cell on the grid given a position on the map. This is the
   * complementary of `calculate_map_position()` above. When designing a level, you'll place units
   * visually in the editor. We'll use this function to find the grid coordinates they're placed on,
   * and call `calculate_map_position()` to snap them to the cell's center.
   */
  public Vector2 CalculateGridCoordinates(Vector2 mapPosition) {
    return (mapPosition / cellSize).Floor();
  }

  // TODO: continue here
}

/**
 * template
 */

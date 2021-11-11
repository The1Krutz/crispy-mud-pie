using Godot;
using Godot.Collections;

/// <summary>
/// Finds the path between two points among walkable cells using A*
/// </summary>
public class Pathfinder : Reference {
  // Private Fields
  private Vector2[] Directions = { Vector2.Left, Vector2.Right, Vector2.Up, Vector2.Down };
  private Grid _grid;
  private AStar2D _astar = new AStar2D();

  // Constructor
  public Pathfinder(Grid grid, Array<Vector2> walkableCells) {
    _grid = grid;

    Dictionary<Vector2, int> cellMappings = new Dictionary<Vector2, int>();
    foreach (Vector2 cell in walkableCells) {
      cellMappings[cell] = _grid.AsIndex(cell);
    }
    AddAndConnectPoints(cellMappings);
  }

  // Public Funtions

  /// <summary>
  /// Returns the path found between `start` and `end` as an array of Vector2 coordinates.
  /// </summary>
  public Vector2[] CalculatePointPath(Vector2 start, Vector2 end) {
    int startIndex = _grid.AsIndex(start);
    int endIndex = _grid.AsIndex(end);

    if (_astar.HasPoint(startIndex) && _astar.HasPoint(endIndex)) {
      return _astar.GetPointPath(startIndex, endIndex);
    } else {
      return new Vector2[0];
    }
  }

  // Private Functions

  /// <summary>
  /// Adds and connects the walkable cells to the Astar2D object.
  /// </summary>
  private void AddAndConnectPoints(Dictionary<Vector2, int> cellMappings) {
    foreach (var point in cellMappings) {
      _astar.AddPoint(point.Value, point.Key);
    }
    foreach (var point in cellMappings) {
      foreach (int neighborIndex in FindNeighborIndices(point.Key, cellMappings)) {
        _astar.ConnectPoints(point.Value, neighborIndex);
      }
    }
  }

  /// <summary>
  /// Returns an array of the cell's connectable neighbors.
  /// </summary>
  private Array<int> FindNeighborIndices(Vector2 cell, Dictionary<Vector2, int> cellMappings) {
    Array<int> neighborIndices = new Array<int>();
    foreach (Vector2 direction in Directions) {
      Vector2 neighbor = cell + direction;

      if (!cellMappings.ContainsKey(neighbor)) {
        continue;
      }

      if (!_astar.ArePointsConnected(cellMappings[cell], cellMappings[neighbor])) {
        neighborIndices.Add(cellMappings[neighbor]);
      }
    }

    return neighborIndices;
  }
}
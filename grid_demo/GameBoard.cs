using System.Linq;
using Godot;
using Godot.Collections;

/// <summary>
/// Represents and manages the game board. Stores references to entities that are in each cell and
/// tells whether cells are occupied or not. Units can only move around the grid one at a time.
/// </summary>
public class GameBoard : Node2D {
  // Signals

  // Exports

  // Public Fields

  // Backing Fields

  // Private Fields
  private Vector2[] Directions = { Vector2.Left, Vector2.Right, Vector2.Up, Vector2.Down };
  private Grid _grid;
  private Dictionary<Vector2, Unit> _units = new Dictionary<Vector2, Unit>(); // key: position on grid, value: unit reference

  // Constructor

  // Lifecycle Hooks
  public override void _Ready() {
    // preload / onready
    _grid = GD.Load<Grid>("res://Grid.tres");

    Reinitialize();
    GD.Print(_units);
  }

  // Public Functions
  public bool IsOccupied(Vector2 cell) {
    return _units.ContainsKey(cell);
  }

  /// <summary>
  /// Returns an array of cells a given unit can walk using the flood fill algorithm
  /// </summary>
  public Array<Vector2> GetWalkableCells(Unit unit) {
    return FloodFill(unit.Cell, unit.MoveRange);
  }

  // Private Functions

  /// <summary>
  /// Clears, and refills the `_units` dictionary with game objects that are on the board.
  /// </summary>
  private void Reinitialize() {
    _units.Clear();

    foreach (var child in GetChildren()) {
      if (child is Unit unit) {
        _units[unit.Cell] = unit;
      }
    }
  }

  /// <summary>
  /// Returns an array with all the coordinates of walkable cells based on the `max_distance`.
  /// </summary>
  private Array<Vector2> FloodFill(Vector2 cell, int maxDistance) {
    var arr = new Array<Vector2>();
    var stack = new Array<Vector2>() { cell };

    while (stack.Count > 0) {
      // bullshit hack around .Pop()
      Vector2 current = stack[0];
      stack.RemoveAt(0);

      if (!_grid.IsWithinBounds(current)) {
        continue;
      }
      if (arr.Contains(current)) {
        continue;
      }

      Vector2 difference = (current - cell).Abs();
      int distance = (int)(difference.x + difference.y);

      if (distance > maxDistance) {
        continue;
      }

      arr.Add(current);

      foreach (Vector2 direction in Directions) {
        Vector2 coordinates = current + direction;

        if (IsOccupied(coordinates)) {
          continue;
        }
        if (arr.Contains(coordinates)) {
          continue;
        }
        stack.Add(coordinates);
      }
    }
    return arr;
  }
}
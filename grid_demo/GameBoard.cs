using System;
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
}
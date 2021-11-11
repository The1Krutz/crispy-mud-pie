using Godot;
using Godot.Collections;

/// <summary>
/// Represents and manages the game board. Stores references to entities that are in each cell and
/// tells whether cells are occupied or not. Units can only move around the grid one at a time.
/// </summary>
public class GameBoard : Node2D {
  // Private Fields
  private Grid _grid;
  private UnitOverlay _unitOverlay;
  private UnitPath _unitPath;
  private Vector2[] Directions = { Vector2.Left, Vector2.Right, Vector2.Up, Vector2.Down };
  private Dictionary<Vector2, Unit> _units = new Dictionary<Vector2, Unit>(); // key: position on grid, value: unit reference
  private Unit _activeUnit;
  private Array<Vector2> _walkableCells = new Array<Vector2>();

  // Lifecycle Hooks
  public override void _Ready() {
    // preload / onready
    _grid = GD.Load<Grid>("res://Grid.tres");
    _unitOverlay = GetNode<UnitOverlay>("UnitOverlay");
    _unitPath = GetNode<UnitPath>("UnitPath");

    Reinitialize();
    GD.Print(_units);
  }

  public override void _UnhandledInput(InputEvent @event) {
    if (_activeUnit != null && @event.IsActionPressed("ui_cancel")) {
      DeselectActiveUnit();
      ClearActiveUnit();
    }
  }

  // Public Functions

  /// <summary>
  /// Returns `true` if the cell is occupied by a unit.
  /// </summary>
  public bool IsOccupied(Vector2 cell) {
    return _units.ContainsKey(cell);
  }

  /// <summary>
  /// Updates the interactive path's drawing if there's an active and selected unit.
  /// </summary>
  public void OnCursorMoved(Vector2 newCell) {
    if (_activeUnit?.IsSelected == true) {
      _unitPath.Draw(_activeUnit.Cell, newCell);
    }
  }

  /// <summary>
  /// Selects or moves a unit based on where the cursor is.
  /// </summary>
  public void OnCursorAcceptPressed(Vector2 cell) {
    if (_activeUnit == null) {
      SelectUnit(cell);
    } else {
      MoveActiveUnit(cell);
    }
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

  /// <summary>
  /// Selects the unit in the `cell` if there's one there. Sets it as the `_active_unit` and draws
  /// its walkable cells and interactive move path. The board reacts to the signals emitted by the
  /// cursor. And it does so by calling functions that select and move a unit.
  /// </summary>
  private void SelectUnit(Vector2 cell) {
    if (!_units.ContainsKey(cell)) {
      return;
    }

    _activeUnit = _units[cell];
    _activeUnit.IsSelected = true;
    _walkableCells = GetWalkableCells(_activeUnit);
    _unitOverlay.Draw(_walkableCells);
    _unitPath.Initialize(_walkableCells);
  }

  /// <summary>
  /// Deselects the active unit, clearing the cells overlay and interactive path drawing.
  /// </summary>
  private void DeselectActiveUnit() {
    _activeUnit.IsSelected = false;
    _unitOverlay.Clear();
    _unitPath.Stop();
  }

  /// <summary>
  /// Clears the reference to the _active_unit and the corresponding walkable cells
  /// </summary>
  private void ClearActiveUnit() {
    _activeUnit = null;
    _walkableCells.Clear();
  }

  /// <summary>
  /// Updates the _units dictionary with the target position for the unit and asks the _active_unit to walk to it.
  /// </summary>
  private async void MoveActiveUnit(Vector2 newCell) {
    if (IsOccupied(newCell) || !_walkableCells.Contains(newCell)) {
      return;
    }

    _units.Remove(_activeUnit.Cell);
    _units[newCell] = _activeUnit;
    DeselectActiveUnit();
    _activeUnit.WalkAlong(_unitPath.CurrentPath);

    await ToSignal(_activeUnit, nameof(Unit.WalkFinished));
    ClearActiveUnit();
  }
}
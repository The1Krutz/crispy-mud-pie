using Godot;
using Godot.Collections;

/// <summary>
/// Draws an overlay over an array of cells
/// </summary>
public class UnitOverlay : TileMap {
  // Signals

  // Exports

  // Public Fields

  // Backing Fields

  // Private Fields

  // Constructor

  // Lifecycle Hooks

  // Public Functions
  public void Draw(Array<Vector2> cells) {
    Clear();
    foreach (Vector2 cell in cells) {
      SetCellv(cell, 0);
    }
  }

  // Private Functions
}
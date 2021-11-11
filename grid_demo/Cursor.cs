using Godot;

/// <summary>
/// Player-controlled cursor. Allows them to navigate the game grid, select units, and move them.
/// Supports both keyboard and mouse/touch input
/// </summary>
public class Cursor : Node2D {
  // Signals
  [Signal]
  public delegate void AcceptPressed(Vector2 cell);
  [Signal]
  public delegate void Moved(Vector2 new_cell);

  // Exports
  [Export]
  public Grid grid;
  [Export]
  public float UiCooldown = 0.1f;

  // Public Fields
  public Vector2 Cell {
    get => _cell;
    set {
      Vector2 newCell = grid.Clamp(value);
      if (newCell.IsEqualApprox(Cell)) {
        return;
      }

      _cell = newCell;
      Position = grid.CalculateMapPosition(_cell);

      EmitSignal(nameof(Moved), _cell);
      _timer.Start();
    }
  }

  // Backing Fields
  private Vector2 _cell = Vector2.Zero;

  // Private Fields
  private Timer _timer;

  // Lifecycle Hooks
  public override void _Ready() {
    // preload / onready
    grid = GD.Load<Grid>("res://Grid.tres");
    _timer = GetNode<Timer>("Timer");

    _timer.WaitTime = UiCooldown;
    Position = grid.CalculateMapPosition(Cell);
  }

  public override void _UnhandledInput(InputEvent @event) {
    if (@event is InputEventMouseMotion mouseMotion) {
      Cell = grid.CalculateGridCoordinates(mouseMotion.Position);
    } else if (@event.IsActionPressed("click") || @event.IsActionPressed("ui_accept")) {
      EmitSignal(nameof(AcceptPressed), Cell);
      GetTree().SetInputAsHandled();
    }

    var shouldMove = @event.IsPressed();
    if (@event.IsEcho()) {
      shouldMove = shouldMove && _timer.IsStopped();
    }

    if (!shouldMove) {
      return;
    }

    if (@event.IsAction("ui_right")) {
      Cell += Vector2.Right;
    } else if (@event.IsAction("ui_up")) {
      Cell += Vector2.Up;
    } else if (@event.IsAction("ui_left")) {
      Cell += Vector2.Left;
    } else if (@event.IsAction("ui_down")) {
      Cell += Vector2.Down;
    }
  }

  public override void _Draw() {
    DrawRect(new Rect2(-grid.cellSize / 2, grid.cellSize), Color.ColorN("aliceblue"), false, 2.0f);
  }
}

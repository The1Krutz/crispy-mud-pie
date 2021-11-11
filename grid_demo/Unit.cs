using System.Linq;
using Godot;
using Godot.Collections;

/// <summary>
/// Represents a unit on the game board.
/// The board manages the unit's position inside the game grid.
/// The unit itself is only a visual representation that moves smoothly in the game world.
/// </summary>
public class Unit : Path2D {
  // Signals
  [Signal]
  public delegate void WalkFinished();

  // Exports
  [Export]
  public Grid grid;
  [Export]
  public int MoveRange = 6;
  [Export]
  public Texture Skin {
    get => _skin;
    set => SetSkin(value); // keep separate so the async still works
  }
  [Export]
  public Vector2 SkinOffset {
    get => _skinOffset;
    set => SetSkinOffset(value); // keep separate so the async still works
  }
  [Export]
  public float MoveSpeed = 600.0f;

  // Public Fields
  public Vector2 Cell {
    get => _cell;
    set => _cell = grid.Clamp(value);
  }
  public bool IsSelected {
    get => _isSelected;
    set {
      _isSelected = value;
      if (IsSelected) {
        _animationPlayer.Play("selected");
      } else {
        _animationPlayer.Play("idle");
      }
    }
  }
  public bool IsWalking {
    get => _isWalking;
    set {
      _isWalking = value;
      SetProcess(IsWalking);
    }
  }

  // Backing Fields
  private Texture _skin;
  private Vector2 _skinOffset;
  private Vector2 _cell;
  private bool _isSelected;
  private bool _isWalking;

  // Private Fields
  private Sprite _sprite;
  private AnimationPlayer _animationPlayer;
  private PathFollow2D _pathFollow;

  // Lifecycle Hooks
  public override void _Ready() {
    grid = GD.Load<Grid>("res://Grid.tres");
    _sprite = GetNode<Sprite>("PathFollow2D/Sprite");
    _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    _pathFollow = GetNode<PathFollow2D>("PathFollow2D");

    SetProcess(false);

    Cell = grid.CalculateGridCoordinates(Position);
    Position = grid.CalculateMapPosition(Cell);

    if (!Engine.EditorHint) {
      Curve = new Curve2D();
    }
  }

  public override void _Process(float delta) {
    _pathFollow.Offset += MoveSpeed * delta;

    if (_pathFollow.UnitOffset >= 1.0) {
      IsWalking = false;

      _pathFollow.Offset = 0.0f;
      Position = grid.CalculateMapPosition(Cell);
      Curve.ClearPoints();

      EmitSignal(nameof(WalkFinished));
    }
  }

  // Private Functions

  /// <summary>
  /// Update the sprite texture
  /// </summary>
  private async void SetSkin(Texture value) {
    _skin = value;
    if (_sprite == null) {
      await ToSignal(this, "ready");
    }
    _sprite.Texture = value;
  }

  /// <summary>
  /// Set the skin offset so the sprite lines up with the shadow
  /// </summary>
  private async void SetSkinOffset(Vector2 value) {
    _skinOffset = value;
    if (_sprite == null) {
      await ToSignal(this, "ready");
    }
    _sprite.Position = value;
  }

  /// <summary>
  /// Starts walking along a path
  /// </summary>
  /// <param name="path">Grid coordinates that this function converts into map coordinates</param>
  public void WalkAlong(Array<Vector2> path) {
    if (path.Count == 0) {
      return;
    }

    Curve.AddPoint(Vector2.Zero);
    foreach (var point in path) {
      Curve.AddPoint(grid.CalculateMapPosition(point) - Position);
    }
    Cell = path.Last();

    IsWalking = true;
  }
}

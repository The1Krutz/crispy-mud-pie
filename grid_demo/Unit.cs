using System.Linq;
using Godot;

/// <summary>
/// Represents a unit on the game board. The board manages the unit's position inside the game grid.
/// The unit itself is only a visual representation that moves smoothly in the game world. We use
/// the tool mode so the skin and skin_offset below update in the editor.
/// </summary>
public class Unit : Path2D {
  [Signal]
  public delegate void WalkFinished();

  [Export]
  public Grid grid;

  [Export]
  public int MoveRange = 6;

  private Texture _skin;
  [Export]
  public Texture Skin {
    get {
      return _skin;
    }
    set {
      SetSkin(value);
    }
  }

  private Vector2 _skinOffset;
  [Export]
  public Vector2 SkinOffset {
    get {
      return _skinOffset;
    }
    set {
      SetSkinOffset(value);
    }
  }

  [Export]
  public float MoveSpeed = 600.0f;

  private Vector2 _cell;
  public Vector2 Cell {
    get {
      return _cell;
    }
    set {
      SetCell(value);
    }
  }

  private bool _isSelected = false;
  public bool IsSelected {
    get {
      return _isSelected;
    }
    set {
      SetIsSelected(value);
    }
  }

  private bool _isWalking;
  public bool IsWalking {
    get {
      return _isWalking;
    }
    set {
      SetIsWalking(value);
    }
  }

  private Sprite _sprite; private AnimationPlayer _animationPlayer; private PathFollow2D _pathFollow;
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

    // debug!
    var points = new Godot.Collections.Array<Vector2> {
      new Vector2(2, 2),
      new Vector2(2, 5),
      new Vector2(8, 5),
      new Vector2(8, 7)
    };
    WalkAlong(points);
    // !debug
  }

  /// <summary>
  /// When active, move the unit along its curve with the help of the PathFollow2d node
  /// </summary>
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

  /// <summary>
  /// When changing the cell value, we don't want to allow coordinates outside the grid, so we clamp them
  /// </summary>
  private void SetCell(Vector2 value) {
    _cell = grid.Clamp(value);
  }

  /// <summary>
  /// This property toggles playback of the 'selected' animation
  /// </summary>
  private void SetIsSelected(bool value) {
    IsSelected = value;
    if (IsSelected) {
      _animationPlayer.Play("selected");
    } else {
      _animationPlayer.Play("idle");
    }
  }

  /// <summary>
  /// Both setters here manipulate the unit's Sprite node.
  /// Here we update the sprite's texture
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
  /// controls whether this unit gets processed or not
  /// </summary>
  private void SetIsWalking(bool value) {
    _isWalking = value;
    SetProcess(IsWalking);
  }

  public void WalkAlong(Godot.Collections.Array<Vector2> path) {
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

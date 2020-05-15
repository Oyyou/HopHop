using HopHop.GUI;
using HopHop.MapStuff;
using HopHop.Sprites;
using HopHop.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HopHop.Lib;

namespace HopHop.Managers
{
  public class UnitManager
  {
    public enum States
    {
      Selected,
      Moving,
    }

    private List<Unit> _units = new List<Unit>();

    private Unit _previousTarget;

    private Unit _selectedUnit = null;

    private MapManager _mapManager;

    private Point _previousMapPoint;
    private Point _currentMapPoint;

    private Engine.Sprite _unitPointerTile;

    private Engine.Sprite _targetPointerTile;

    private UnitPointer _unitPointer;

    public States State { get; set; } = States.Selected;

    public bool UpdateUnitIndex = false;

    public UnitManager(List<Unit> units, MapManager mapDrawer)
    {
      _units = units;

      _mapManager = mapDrawer;
    }

    public void LoadContent(ContentManager content)
    {
      _unitPointerTile = new Engine.Sprite(content.Load<Texture2D>("Units/Misc/PointerTile"))
      {
        Layer = 0.20f,
      };

      _targetPointerTile = new Engine.Sprite(content.Load<Texture2D>("Units/Misc/PointerTile"))
      {
        Layer = 0.20f,
      };

      _unitPointer = new UnitPointer(content.Load<Texture2D>("Units/Misc/Pointer"));
    }

    public void UnloadContent()
    {

    }

    public bool FinisedMoving = false;

    public void Update(GameTime gameTime, BattleGUI gui, Unit selectedTarget)
    {
      _selectedUnit = _units[gui.SelectedUnitIndex];

      FinisedMoving = false;

      switch (State)
      {
        case States.Selected:

          _previousMapPoint = _currentMapPoint;
          _currentMapPoint = Map.Vector2ToPoint(Game1.GameMouse.Position_WithCamera);

          if (Game1.GameMouse.HasRightClicked && _selectedUnit != null && Game1.GameMouse.ClickableObjects.Count == 0)
          {
            State = States.Moving;
          }

          _mapManager.SetUnit(_selectedUnit);

          if (_previousTarget != selectedTarget || selectedTarget == null)
          {
            _previousTarget = selectedTarget;
            SetUnitPath(selectedTarget);
          }

          break;
        case States.Moving:

          _selectedUnit.Move();

          if (_selectedUnit.MovementPositions.Count == 0)
          {
            FinisedMoving = true;

            _selectedUnit.UpdateStamina();
            _mapManager.Refresh();

            if (_selectedUnit.UnitModel.Stamina <= 0)
              UpdateUnitIndex = true;


            //_selectedUnit = null;
            State = States.Selected;
          }

          break;
        default:
          break;
      }

      _unitPointerTile.Layer = _selectedUnit.Layer -= 0.001f;
      _unitPointerTile.Position = _selectedUnit.TilePosition - new Vector2(4, 4);

      if (selectedTarget != null)
      {
        _targetPointerTile.Layer = selectedTarget.Layer -= 0.001f;
        _targetPointerTile.Position = selectedTarget.TilePosition - new Vector2(4, 4);
      }
      else
      {
        _targetPointerTile.Layer = -1;
      }


      _unitPointer.Update(gameTime, _selectedUnit.Position);
    }

    private void SetUnitPath(Unit target)
    {
      if (_selectedUnit == null)
        return;

      if (target == null && _previousMapPoint == _currentMapPoint)
        return;

      var mapPoint = Map.Vector2ToPoint(_selectedUnit.TilePosition);
      var endPoint = _currentMapPoint;

      if (target != null)
      {
        var targetPoint = Map.Vector2ToPoint(target.TilePosition);

        var points = new List<Point>()
        {
          targetPoint + new Point(0, -1),   // Top
          targetPoint + new Point(1, -1),   // Top-Right
          targetPoint + new Point(1, 0),    // Right
          targetPoint + new Point(1, 1),    // Bottom-Right
          targetPoint + new Point(0, 1),    // Bottom
          targetPoint + new Point(-1, 1),   // Bottom-Left
          targetPoint + new Point(-1, 0),   // Left
          targetPoint + new Point(-1, -1),  // Top-Left
        };

        int length = 100;

        foreach (var p in points)
        {
          var result = PathFinder.Find(_mapManager.Map.Get(), mapPoint, p);

          if (result.Status == PathStatus.Valid)
          {
            if (result.Path.Count < length)
            {
              endPoint = p;
              length = result.Path.Count;
            }
          }
        }
      }

      var pfResult = PathFinder.Find(_mapManager.Map.Get(), mapPoint, endPoint);
      _selectedUnit.SetPath(pfResult.Path);
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      _unitPointerTile.Draw(gameTime, spriteBatch);
      _targetPointerTile.Draw(gameTime, spriteBatch);
      _unitPointer.Draw(gameTime, spriteBatch);

      foreach (var unit in _units)
        unit.Draw(gameTime, spriteBatch);
    }
  }
}

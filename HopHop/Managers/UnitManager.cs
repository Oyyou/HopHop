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
using HopHop.Lib.Models;
using Engine;

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
    private AbilityModel _previousAbility;

    private Unit _selectedUnit = null;

    private MapManager _mapManager;

    /// <summary>
    /// Where the cursor was on the map last frame
    /// </summary>
    private Point _previousMapPoint;

    /// <summary>
    /// Where the cursor is on the map
    /// </summary>
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

    private Vector2 _previousPosition;

    private Vector2 _currentPosition;

    public void Update(GameTime gameTime, BattleGUI gui, Unit selectedTarget)
    {
      _selectedUnit = _units[gui.SelectedUnitIndex];

      FinisedMoving = false;

      switch (State)
      {
        case States.Selected:

          _previousMapPoint = _currentMapPoint;
          _currentMapPoint = Map.Vector2ToPoint(Game1.GameMouse.Position_WithCamera);
          _currentMapPoint = Helpers.Clamp(_currentMapPoint, new Point(0, 0), new Point(_mapManager.Map.GetWidth(), _mapManager.Map.GetHeight()));

          if (Game1.GameMouse.HasRightClicked && _selectedUnit != null && Game1.GameMouse.ClickableObjects.Where(c => !(c is Unit)).Count() == 0)
          {
            State = States.Moving;
          }

          _mapManager.SetUnit(_selectedUnit);

          _previousPosition = _currentPosition;
          _currentPosition = _selectedUnit.Position;

          if (_previousPosition != _currentPosition)
          {
            _selectedUnit.PotentialPoints = new List<Tuple<int, Point>>();

            var unitPoint = Map.Vector2ToPoint(_selectedUnit.TilePosition);

            var distance = (_selectedUnit.UnitModel.Speed * _selectedUnit.UnitModel.Stamina);

            var top = (int)MathHelper.Max(unitPoint.Y - (distance + 1), 0);
            var left = (int)MathHelper.Max(unitPoint.X - (distance + 1), 0);

            var right = (int)MathHelper.Min(unitPoint.X + (distance + 1), _mapManager.Map.GetWidth());
            var bottom = (int)MathHelper.Min(unitPoint.Y + (distance + 1), _mapManager.Map.GetHeight());

            for (var y = top; y < bottom; y++)
            {
              for (var x = left; x < right; x++)
              {
                var endPoint = new Point(x, y);

                if (unitPoint == endPoint)
                  continue;

                var pfResult = PathFinder.Find(_mapManager.Map.Get(), unitPoint, endPoint);

                if (pfResult.Status == PathStatus.Valid && pfResult.Path.Count() <= distance)
                {
                  _selectedUnit.PotentialPoints.Add(new Tuple<int, Point>(pfResult.Path.Count, pfResult.Path.Last()));
                }
              }
            }
          }


          //// if there isn't a target
          //if (selectedTarget == null)
          //{
          //  SetUnitPath();
          //}
          //// if the target has changed
          //else if (_previousTarget != selectedTarget)
          //{
          //  SetUnitPath(selectedTarget);
          //}
          //// If the mouse has changed tile
          //else if (_previousMapPoint != _currentMapPoint)
          //{
          //  SetUnitPath(selectedTarget);
          //}

          //_previousTarget = selectedTarget;

          if ((_previousTarget != selectedTarget) ||
              (selectedTarget == null) ||
              (_previousMapPoint != _currentMapPoint) ||
              (_previousAbility != _selectedUnit.UnitModel.Abilities.Get(gui.SelectedAbilityIndex)))
          {
            _previousAbility = _selectedUnit.UnitModel.Abilities.Get(gui.SelectedAbilityIndex);

            var ability = _selectedUnit.UnitModel.Abilities.Get(gui.SelectedAbilityIndex);

            if (_previousTarget != selectedTarget)
            {
              _previousTarget = selectedTarget;
              _selectedUnit.PotentialPaths = new List<List<Point>>();
            }

            if (ability != null)
            {
              if (ability.AbilityType == Lib.Models.AbilityModel.AbilityTypes.Close)
                SetUnitPath(selectedTarget);
            }
            else
            {
              SetUnitPath(selectedTarget);
            }
          }

          break;
        case States.Moving:

          _selectedUnit.Move();

          if (_selectedUnit.MovementPath.Count == 0)
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

    private void SetUnitPath(Unit target = null)
    {
      if (_selectedUnit == null)
        return;

      if (target == null && _previousMapPoint == _currentMapPoint)
        return;

      var mapPoint = Map.Vector2ToPoint(_selectedUnit.TilePosition);

      if (target != null)
      {
        //if (_selectedUnit.UnitModel.Equals(target.UnitModel))
        //{

        //}
        //else 
        if (_selectedUnit.PotentialPaths.Count > 0)
        {
          var path = _selectedUnit.PotentialPaths.FirstOrDefault(c => c.Count() > 0 && c.Last() == _currentMapPoint);

          if (path == null)
          {
            if (_selectedUnit.PotentialPaths.Any(c => c.Count() == 0) && _currentMapPoint == mapPoint)
            {
              _selectedUnit.MovementPath = new List<Point>()
              {
                mapPoint,
              };
            }
          }
          else
          {
            _selectedUnit.SetPath(path);
          }
        }
        else
        {
          var potentialPaths = new List<List<Point>>();

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

          int length = _selectedUnit.UnitModel.Speed * _selectedUnit.UnitModel.Stamina;

          foreach (var p in points)
          {
            var result = PathFinder.Find(_mapManager.Map.Get(), mapPoint, p);

            if (result.Status == PathStatus.Valid)
            {

              if (result.Path.Count <= length)
              {
                potentialPaths.Add(result.Path);
              }
            }
          }

          potentialPaths = potentialPaths.OrderBy(c => c.Count).ToList();

          if (potentialPaths.Count > 0)
            _selectedUnit.SetPath(potentialPaths);
        }
      }
      else
      {
        var pfResult = PathFinder.Find(_mapManager.Map.Get(), mapPoint, _currentMapPoint);
        _selectedUnit.SetPath(pfResult.Path);
      }
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

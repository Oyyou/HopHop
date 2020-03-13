using HopHop.MapStuff;
using HopHop.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopHop.Managers
{
  public class UnitManager
  {
    public enum States
    {
      Selecting,
      Selected,
      Moving,
    }

    private List<Unit> _units = new List<Unit>();

    private Unit _selectedUnit = null;

    private MapManager _mapManager;

    private Point _previousMapPoint;
    private Point _currentMapPoint;

    public States State { get; private set; } = States.Selecting;

    public UnitManager(List<Unit> units, MapManager mapDrawer)
    {
      _units = units;

      _mapManager = mapDrawer;
    }

    public void UnloadContent()
    {

    }

    public void Update(GameTime gameTime)
    {

      switch (State)
      {
        case States.Selecting:

          foreach (var sprite in _units)
          {
            sprite.TilesMoved = 0;
            if (Game1.GameMouse.HasLeftClicked)
            {
              if (Game1.GameMouse.ValidObject == sprite)
              {
                _selectedUnit = sprite;
                State = States.Selected;
              }
            }

            sprite.Update(gameTime);
          }

          _mapManager.SetUnit(_selectedUnit);

          break;
        case States.Selected:

          _previousMapPoint = _currentMapPoint;
          _currentMapPoint = Map.Vector2ToPoint(Game1.GameMouse.Position_WithCamera);

          if (Game1.GameKeyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
          {
            _selectedUnit?.SetPath(new List<Point>());
            _selectedUnit = null;
            State = States.Selecting;
          }

          if (Game1.GameMouse.HasLeftClicked)
          {
            _selectedUnit?.SetPath(new List<Point>());
            _selectedUnit = null;
            State = States.Selecting;
          }

          if (Game1.GameMouse.HasRightClicked && _selectedUnit != null)
          {
            State = States.Moving;
          }

          _mapManager.SetUnit(_selectedUnit);

          SetUnitPath();

          break;
        case States.Moving:

          _selectedUnit.Move();

          if (_selectedUnit.MovementPositions.Count == 0)
          {
            _selectedUnit.UpdateStamina();
            _mapManager.Refresh();
            _selectedUnit = null;
            State = States.Selecting;
          }

          break;
        default:
          break;
      }
    }

    private void SetUnitPath()
    {
      if (_selectedUnit == null)
        return;

      if (_previousMapPoint == _currentMapPoint)
        return;

      var mapPoint = Map.Vector2ToPoint(_selectedUnit.TilePosition);
      var pfResult = PathFinder.Find(_mapManager.Map.Get(), mapPoint, _currentMapPoint);
      _selectedUnit.SetPath(pfResult.Path);
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {

      foreach (var sprite in _units)
        sprite.Draw(gameTime, spriteBatch);
    }
  }
}

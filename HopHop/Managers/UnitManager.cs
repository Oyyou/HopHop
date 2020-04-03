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

    private Unit _selectedUnit = null;

    private MapManager _mapManager;

    private Point _previousMapPoint;
    private Point _currentMapPoint;

    private Engine.Sprite _unitPointerTile;

    private UnitPointer _unitPointer;

    public States State { get; private set; } = States.Selected;

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

      _unitPointer = new UnitPointer(content.Load<Texture2D>("Units/Misc/Pointer"));
    }

    public void UnloadContent()
    {

    }

    public void Update(GameTime gameTime, BattleGUI gui)
    {
      _selectedUnit = _units[gui.SelectedUnitIndex];

      switch (State)
      {
        case States.Selected:

          _previousMapPoint = _currentMapPoint;
          _currentMapPoint = Map.Vector2ToPoint(Game1.GameMouse.Position_WithCamera);

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

            if (_selectedUnit.Stamina <= 0)
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
      _unitPointer.Update(gameTime, _selectedUnit.Position);
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
      _unitPointerTile.Draw(gameTime, spriteBatch);
      _unitPointer.Draw(gameTime, spriteBatch);

      foreach (var sprite in _units)
        sprite.Draw(gameTime, spriteBatch);
    }
  }
}

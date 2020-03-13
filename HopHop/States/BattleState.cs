using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Models;
using HopHop.GUI;
using HopHop.Lib;
using HopHop.Managers;
using HopHop.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static HopHop.Lib.Enums;

namespace HopHop.States
{
  public class BattleState : State
  {
    private MapManager _mapManager;

    private UnitManager _unitManager;

    private readonly List<Unit> _units;

    private BattleGUI _gui;

    private Camera _camera;

    public BattleStates State
    {
      get { return _gui.State; }
      set
      {
        _gui.State = value;
      }
    }

    private List<Vector2> _tests;

    public BattleState(GameModel gameModel, List<Unit> units)
      : base(gameModel)
    {
      _units = units;
    }

    public override void LoadContent()
    {
      _mapManager = new MapManager(Content);

      _mapManager.Refresh = () =>
      {
        _mapManager.Map.Clear();

        foreach (var sprite in _units)
          _mapManager.Map.AddItem(sprite.TileRectangle);

        _mapManager.Map.Write();
      };

      _mapManager.Refresh();

      _unitManager = new UnitManager(_units, _mapManager);

      _gui = new BattleGUI(_gameModel);

      _camera = new Camera();
    }

    public override void UnloadContent()
    {
      _unitManager.UnloadContent();
      _mapManager.UnloadContent();
    }

    private int _unitIndex = 0;

    public override void Update(GameTime gameTime)
    {
      BaseGame.GameMouse.AddCamera(_camera.Transform);

      if (BaseGame.GameKeyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Tab))
      {
        _camera.GoTo(_units[_unitIndex].Rectangle);
        _unitIndex++;

        if (_unitIndex >= _units.Count)
          _unitIndex = 0;
      }


      _gui.Update(gameTime);
      _camera.Update(gameTime);
      _unitManager.Update(gameTime);
      _mapManager.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
      SpriteBatch.Begin(SpriteSortMode.FrontToBack, transformMatrix: _camera.Transform);

      _mapManager.Draw(gameTime, SpriteBatch);
      _unitManager.Draw(gameTime, SpriteBatch);

      SpriteBatch.End();

      _gui.Draw(gameTime, SpriteBatch);
    }
  }
}

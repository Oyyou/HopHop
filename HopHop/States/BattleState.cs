﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine;
using Engine.Models;
using HopHop.GUI;
using HopHop.Lib;
using HopHop.Managers;
using HopHop.MapStuff;
using HopHop.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static HopHop.Lib.Enums;
using Microsoft.Xna.Framework.Input;

namespace HopHop.States
{
  public class BattleState : State
  {
    private MapManager _mapManager;

    private UnitManager _unitManager;

    private List<Sprites.Sprite> _backgroundTiles;

    private readonly List<Unit> _units;

    private List<Unit> _targets = new List<Unit>();

    private List<Unit> _enemies;

    private List<Sprites.Sprite> _walls;

    private BattleGUI _gui;

    private Camera _camera;

    private SpriteFont _font;

    private float _enemyTimer = 5f;

    public BattleStates NextState
    {
      get { return _gui.NextState; }
      set
      {
        _gui.NextState = value;
      }
    }

    public BattleStates State
    {
      get { return _gui.State; }
      set
      {
        _gui.State = value;
      }
    }

    private bool _abilitySelected = false;

    public BattleState(GameModel gameModel, List<Unit> units)
      : base(gameModel)
    {
      _units = units;
    }

    public override void LoadContent()
    {
      _walls = new List<Sprites.Sprite>()
      {
        new Sprites.Sprite(Content.Load<Texture2D>("Walls/Wall_001"))
        {
          TilePosition = Map.PointToVector2(5, 0),
        },
        new Sprites.Sprite(Content.Load<Texture2D>("Cover/Crate"))
        {
          TilePosition = Map.PointToVector2(3, 6),
        },
        new Sprites.Sprite(Content.Load<Texture2D>("Cover/Crate"))
        {
          TilePosition = Map.PointToVector2(5, 7),
        },
      };

      _enemies = new List<Unit>()
      {
        new Enemy(Content.Load<Texture2D>("Units/Enemies/Egg"))
        {
          TilePosition = Map.PointToVector2(7, 7),
        },
        new Enemy(Content.Load<Texture2D>("Units/Enemies/Egg"))
        {
          TilePosition = Map.PointToVector2(3, 7),
        },
        new Enemy(Content.Load<Texture2D>("Units/Enemies/Egg"))
        {
          TilePosition = Map.PointToVector2(9, 6),
        },
      };

      _mapManager = new MapManager(Content);

      _mapManager.Refresh = () =>
      {
        _mapManager.Map.Clear();

        foreach (var sprite in _units)
          _mapManager.Map.AddItem(sprite.TileRectangle);

        foreach (var sprite in _enemies)
          _mapManager.Map.AddItem(sprite.TileRectangle);

        foreach (var sprite in _walls)
          _mapManager.Map.AddItem(sprite.TileRectangle);

        _mapManager.Map.Write();
      };

      var images = new List<string>()
      {
        "Stone_01",
        "Stone_02",
        "Stone_03",
        "Stone_04",
      };

      _backgroundTiles = new List<Sprites.Sprite>();

      for (int y = 0; y < _mapManager.Map.GetHeight(); y++)
      {
        for (int x = 0; x < _mapManager.Map.GetWidth(); x++)
        {
          //_backgroundTiles.Add(new Sprites.Sprite(Content.Load<Texture2D>($"Tiles/Stone/{images[BaseGame.Random.Next(0, images.Count)]}"))
          _backgroundTiles.Add(new Sprites.Sprite(Content.Load<Texture2D>($"Tiles/Floor"))
          {
            HasFixedLayer = true,
            Layer = 0,
            TilePosition = Map.PointToVector2(x, y)
          });
        }
      }

      _mapManager.Refresh();

      _unitManager = new UnitManager(_units, _mapManager);
      _unitManager.LoadContent(Content);

      _gui = new BattleGUI(_gameModel, _units.Select(c => c.UnitModel).ToList());

      _camera = new Camera()
      {
        Position = new Vector2(BaseGame.ScreenWidth / 2, BaseGame.ScreenHeight / 2),
      };

      _font = Content.Load<SpriteFont>("Fonts/Font");
    }

    public override void UnloadContent()
    {
      _unitManager.UnloadContent();
      _mapManager.UnloadContent();
    }

    public override void Update(GameTime gameTime)
    {
      if (State != NextState)
      {
        if (NextState == BattleStates.PlayerTurn)
        {
          _units.ForEach(c =>
          {
            c.Stamina = 2;
            c.TilesMoved = 0;
          });
        }

        State = NextState;
      }

      BaseGame.GameMouse.AddCamera(_camera.Transform);

      switch (State)
      {
        case BattleStates.PlayerTurn:

          CheckInput();

          _gui.Update(gameTime);
          _camera.Update(gameTime);
          _unitManager.Update(gameTime, _gui, (_selectedTargetIndex > -1 && _selectedTargetIndex < _targets.Count) ? _targets[_selectedTargetIndex] : null);
          if (_unitManager.UpdateUnitIndex)
          {
            _unitManager.UpdateUnitIndex = false;
            SelectNextUnit();
          }

          _mapManager.Update(gameTime);

          if (_units.All(c => c.Stamina <= 0))
          {
            NextState = BattleStates.EnemyTurn;
          }

          break;

        case BattleStates.EnemyTurn:

          _enemyTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

          if (_enemyTimer <= 0)
          {
            _enemyTimer = 5f;
            NextState = BattleStates.PlayerTurn;
          }

          break;
        default:
          break;
      }
    }

    private void CheckInput()
    {
      if (!_abilitySelected)
      {
        if (BaseGame.GameKeyboard.IsKeyPressed(Keys.Tab))
        {
          SelectNextUnit();
        }
        else if (BaseGame.GameKeyboard.IsKeyPressed(Keys.LeftShift))
        {
          SelectPreviousUnit();
        }

        if (BaseGame.GameKeyboard.IsKeyPressed(Keys.D1))
        {
          _gui.SelectedAbilityIndex = 0;
          _abilitySelected = true;
        }
        else if (BaseGame.GameKeyboard.IsKeyPressed(Keys.D2))
        {
          _gui.SelectedAbilityIndex = 1;
          _abilitySelected = true;
        }
        else if (BaseGame.GameKeyboard.IsKeyPressed(Keys.D3))
        {
          _gui.SelectedAbilityIndex = 2;
          _abilitySelected = true;
        }
        else if (BaseGame.GameKeyboard.IsKeyPressed(Keys.D4))
        {
          _gui.SelectedAbilityIndex = 3;
          _abilitySelected = true;
        }

        if (_abilitySelected)
        {
          var unit = _units[_gui.SelectedUnitIndex];
          var ability = unit.UnitModel.Abilities.Get(_gui.SelectedAbilityIndex);

          switch (ability.TargetType)
          {
            case Lib.Models.AbilityModel.TargetTypes.Enemies:
              _targets = _enemies;
              break;
            case Lib.Models.AbilityModel.TargetTypes.Friendlies:
              _targets = _units;
              break;
            case Lib.Models.AbilityModel.TargetTypes.Self:
              _targets = new List<Unit>() { unit };
              break;
            default:
              break;
          }

          _selectedTargetIndex = 0;
          SelectNextTarget();
        }
      }
      else // If we've got an ability selected
      {
        if (BaseGame.GameKeyboard.IsKeyPressed(Keys.Escape))
        {
          _abilitySelected = false;
          _gui.SelectedAbilityIndex = -1;
          _targets = new List<Unit>();
          return;
        }

        if (BaseGame.GameKeyboard.IsKeyPressed(Keys.Tab))
        {
            SelectNextTarget();
        }
        else if (BaseGame.GameKeyboard.IsKeyPressed(Keys.LeftShift))
        {
          SelectPreviousTarget();
        }

        var newAbilityIndex = -1;

        if (BaseGame.GameKeyboard.IsKeyPressed(Keys.D1))
        {
          newAbilityIndex = 0;
        }
        else if (BaseGame.GameKeyboard.IsKeyPressed(Keys.D2))
        {
          newAbilityIndex = 1;
        }
        else if (BaseGame.GameKeyboard.IsKeyPressed(Keys.D3))
        {
          newAbilityIndex = 2;
        }
        else if (BaseGame.GameKeyboard.IsKeyPressed(Keys.D4))
        {
          newAbilityIndex = 3;
        }

        // Cast ability
        if (newAbilityIndex == _gui.SelectedAbilityIndex)
        {

        }
        else
        {
          if (newAbilityIndex > -1)
          {
            _gui.SelectedAbilityIndex = newAbilityIndex;

            var unit = _units[_gui.SelectedUnitIndex];
            var ability = unit.UnitModel.Abilities.Get(_gui.SelectedAbilityIndex);

            switch (ability.TargetType)
            {
              case Lib.Models.AbilityModel.TargetTypes.Enemies:
                _targets = _enemies;
                break;
              case Lib.Models.AbilityModel.TargetTypes.Friendlies:
                _targets = _units;
                break;
              case Lib.Models.AbilityModel.TargetTypes.Self:
                _targets = new List<Unit>() { unit };
                break;
              default:
                break;
            }

            _selectedTargetIndex = 0;
            SelectNextTarget();
          }
        }
      }
    }

    private void SelectNextUnit()
    {
      _gui.SelectedUnitIndex++;

      if (_gui.SelectedUnitIndex >= _units.Count)
        _gui.SelectedUnitIndex = 0;

      for (int i = 0; i < _units.Count; i++)
      {
        if (_units[_gui.SelectedUnitIndex].Stamina > 0)
          break;

        _gui.SelectedUnitIndex++;

        if (_gui.SelectedUnitIndex >= _units.Count)
          _gui.SelectedUnitIndex = 0;
      }

      if (_gui.SelectedUnitIndex < _units.Count)
      {
        _camera.GoTo(_units[_gui.SelectedUnitIndex].TileRectangle);
      }
      else
      {
        _gui.SelectedUnitIndex = 0;
      }
    }

    private void SelectPreviousUnit()
    {
      _gui.SelectedUnitIndex--;

      if (_gui.SelectedUnitIndex < 0)
        _gui.SelectedUnitIndex = _units.Count - 1;

      for (int i = _units.Count - 1; i > -1; i--)
      {
        if (_units[_gui.SelectedUnitIndex].Stamina > 0)
          break;

        _gui.SelectedUnitIndex--;

        if (_gui.SelectedUnitIndex < 0)
          _gui.SelectedUnitIndex = _units.Count - 1;
      }

      if (_gui.SelectedUnitIndex < _units.Count)
      {
        _camera.GoTo(_units[_gui.SelectedUnitIndex].TileRectangle);
      }
      else
      {
        _gui.SelectedUnitIndex = 0;
      }
    }

    private int _selectedTargetIndex = -1;
    private void SelectNextTarget()
    {
      _selectedTargetIndex++;

      if (_selectedTargetIndex >= _targets.Count)
        _selectedTargetIndex = 0;

      for (int i = 0; i < _targets.Count; i++)
      {
        if (_targets[_selectedTargetIndex].Stamina > 0)
          break;

        _selectedTargetIndex++;

        if (_selectedTargetIndex >= _targets.Count)
          _selectedTargetIndex = 0;
      }

      if (_selectedTargetIndex < _targets.Count)
      {
        _camera.GoTo(_targets[_selectedTargetIndex].TileRectangle);
      }
      else
      {
        _selectedTargetIndex = 0;
      }
    }

    private void SelectPreviousTarget()
    {

    }

    public override void Draw(GameTime gameTime)
    {
      SpriteBatch.Begin(SpriteSortMode.FrontToBack, transformMatrix: _camera.Transform);

      foreach (var tile in _backgroundTiles)
        tile.Draw(gameTime, SpriteBatch);

      _mapManager.Draw(gameTime, SpriteBatch);
      _unitManager.Draw(gameTime, SpriteBatch);

      foreach (var enemy in _enemies)
        enemy.Draw(gameTime, SpriteBatch);

      foreach (var wall in _walls)
        wall.Draw(gameTime, SpriteBatch);

      SpriteBatch.End();

      switch (State)
      {
        case BattleStates.PlayerTurn:
          _gui.Draw(gameTime, SpriteBatch);
          break;
        case BattleStates.EnemyTurn:
          SpriteBatch.Begin();

          var text = $"Enemy Turn: {_enemyTimer:00.00}";
          var position = new Vector2(BaseGame.ScreenWidth / 2, BaseGame.ScreenHeight / 2) -
            new Vector2(_font.MeasureString(text).X / 2, _font.MeasureString(text).Y / 2);

          SpriteBatch.DrawString(_font, text, position, Color.Red);
          SpriteBatch.End();
          break;
      }
    }
  }
}

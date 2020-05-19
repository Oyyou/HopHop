using System;
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
using HopHop.Lib.Models;

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

      int id = _units.Select(c => c.UnitModel.Id).OrderBy(c => c).Last() + 1;

      _enemies = new List<Unit>()
      {
        new Enemy(Content.Load<Texture2D>("Units/Enemies/Egg"))
        {
          TilePosition = Map.PointToVector2(7, 7),
          UnitModel = new Lib.Models.UnitModel()
          {
            Id = id++,
            Armour = 0,
            Health = 5,
            Speed = 2,
            Name = "Stu",
            UnitType = Lib.Models.UnitModel.UnitTypes.Enemy,
          },
        },
        new Enemy(Content.Load<Texture2D>("Units/Enemies/Egg"))
        {
          TilePosition = Map.PointToVector2(3, 7),
          UnitModel = new Lib.Models.UnitModel()
          {
            Id = id++,
            Armour = 0,
            Health = 5,
            Speed = 2,
            Name = "Frank",
            UnitType = Lib.Models.UnitModel.UnitTypes.Enemy,
          },
        },
        new Enemy(Content.Load<Texture2D>("Units/Enemies/Egg"))
        {
          TilePosition = Map.PointToVector2(9, 6),
          UnitModel = new Lib.Models.UnitModel()
          {
            Id = id++,
            Armour = 0,
            Health = 5,
            Speed = 2,
            Name = "Jim",
            UnitType = Lib.Models.UnitModel.UnitTypes.Enemy,
          },
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
            TilePosition = Map.PointToVector2(x, y + 1) // TODO: Figure out why I've added +1
          });
        }
      }

      _mapManager.Refresh();

      _unitManager = new UnitManager(_units, _mapManager);
      _unitManager.LoadContent(Content);

      _gui = new BattleGUI(_gameModel, _units.Select(c => c.UnitModel).ToList());
      _gui.UnitsGroup.OnButtonChange = OnUnitSelect;
      _gui.AbilitiesGroup.OnButtonChange = OnAbilitySelect;
      _gui.TargetsGroup.OnButtonChange = OnTargetSelect;

      _camera = new Camera()
      {
        Position = new Vector2(BaseGame.ScreenWidth / 2, BaseGame.ScreenHeight / 2),
      };

      _font = Content.Load<SpriteFont>("Fonts/Font");

      OnUnitSelect();
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
            c.UnitModel.Stamina = 2;
            c.TilesMoved = 0;
          });

          _gui.UnitsGroup.CurrentIndex = 0;
          OnUnitSelect();
        }

        State = NextState;
      }

      BaseGame.GameMouse.AddCamera(_camera.Transform);

      switch (State)
      {
        case BattleStates.PlayerTurn:

          CheckInput();

          _gui.Update(gameTime, _targets.Select(c => c.UnitModel).ToList());

          if (_unitManager.State == UnitManager.States.Moving)
            _camera.GoTo(_units[_gui.SelectedUnitIndex].TileRectangle);

          var selectedTargetIndex = _gui.TargetsGroup.CurrentIndex;

          _camera.Update(gameTime);
          _unitManager.Update(gameTime, _gui, (selectedTargetIndex > -1 && selectedTargetIndex < _targets.Count) ? _targets[selectedTargetIndex] : null);

          if (_unitManager.FinisedMoving)
            OnUnitSelect();

          foreach (var unit in _units)
            unit.Update(gameTime);

          foreach (var unit in _enemies)
            unit.Update(gameTime);

          if (_gui.HoveringTargetIndex > -1)
          {
            var target = _targets[_gui.HoveringTargetIndex];

            switch (target.UnitModel.UnitType)
            {
              case Lib.Models.UnitModel.UnitTypes.Friendly:
                target.Colour = Color.Blue;
                break;
              case Lib.Models.UnitModel.UnitTypes.Enemy:
                target.Colour = Color.Red;
                break;
              default:
                break;
            }
          }

          if (_unitManager.UpdateUnitIndex)
          {
            _unitManager.UpdateUnitIndex = false;

            _gui.UnitsGroup.Increment();

            _camera.GoTo(_units[_gui.SelectedUnitIndex].TileRectangle);
          }

          _mapManager.Update(gameTime);

          if (_units.All(c => c.UnitModel.Stamina <= 0) && _unitManager.State == UnitManager.States.Selected)
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
          _gui.UnitsGroup.Increment();
          _camera.GoTo(_units[_gui.SelectedUnitIndex].TileRectangle);

        }
        else if (BaseGame.GameKeyboard.IsKeyPressed(Keys.LeftShift))
        {
          _gui.UnitsGroup.Decrement();
          _camera.GoTo(_units[_gui.SelectedUnitIndex].TileRectangle);
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
          OnAbilitySelect();
        }
      }
      else // If we've got an ability selected
      {
        if (BaseGame.GameKeyboard.IsKeyPressed(Keys.Escape))
        {
          _abilitySelected = false;
          _gui.SelectedAbilityIndex = -1;
          _gui.SelectedTargetIndex = -1;
          _targets = new List<Unit>();
          return;
        }

        if (BaseGame.GameKeyboard.IsKeyPressed(Keys.Tab))
        {
          _gui.TargetsGroup.Increment();
          _camera.GoTo(_targets[_gui.TargetsGroup.CurrentIndex].TileRectangle);
        }
        else if (BaseGame.GameKeyboard.IsKeyPressed(Keys.LeftShift))
        {
          _gui.TargetsGroup.Decrement();
          _camera.GoTo(_targets[_gui.TargetsGroup.CurrentIndex].TileRectangle);
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

        if (BaseGame.GameKeyboard.IsKeyPressed(Keys.Space) ||
            BaseGame.GameKeyboard.IsKeyPressed(Keys.Enter))
        {
          newAbilityIndex = _gui.SelectedAbilityIndex;
        }

        // Cast ability
        if (newAbilityIndex == _gui.SelectedAbilityIndex)
        {
          var unit = _units[_gui.SelectedUnitIndex];

          unit.UnitModel.Stamina -= unit.UnitModel.Abilities.Get(_gui.SelectedAbilityIndex).StaminaCost;

          _unitManager.State = UnitManager.States.Moving;
          _targets = new List<Unit>();
          _abilitySelected = false;
          _gui.TargetsGroup.CurrentIndex = -1;
          //_gui.HoveringTargetIndex = -1;
        }
        else
        {
          if (newAbilityIndex > -1)
          {
            _gui.SelectedAbilityIndex = newAbilityIndex;

            OnAbilitySelect();
          }
        }
      }
    }

    private void OnAbilitySelect()
    {
      _abilitySelected = true;

      var unit = _units[_gui.SelectedUnitIndex];
      var unitDistance = unit.UnitModel.Stamina * unit.UnitModel.Speed;
      var ability = unit.UnitModel.Abilities.Get(_gui.SelectedAbilityIndex);

      _targets = new List<Unit>();

      foreach (var target in ability.Targets)
      {
        _targets.AddRange(_units.Where(c => c.UnitModel.Id == target.Id));
        _targets.AddRange(_enemies.Where(c => c.UnitModel.Id == target.Id));
      }

      if (_targets.Count == 0)
        return;

      //_gui.TargetsGroup.Increment();
      _gui.SelectedTargetIndex = 0;
      _camera.GoTo(_targets[_gui.TargetsGroup.CurrentIndex].TileRectangle);
    }

    /// <summary>
    /// Todo: Optimise
    /// </summary>
    private void OnUnitSelect()
    {
      _abilitySelected = false;
      _targets = new List<Unit>();
      _gui.TargetsGroup.CurrentIndex = -1;

      var unit = _units[_gui.SelectedUnitIndex];
      var unitDistance = unit.UnitModel.Stamina * unit.UnitModel.Speed;
      var unitPoint = Map.Vector2ToPoint(unit.TilePosition);

      _camera.GoTo(unit.TileRectangle);

      var targets = new List<Unit>();

      Action<List<Unit>> setCloseTargets = (units) =>
      {
        var tempUnits = new List<Tuple<int, Unit>>();

        foreach (var u in units)
        {
          var uPoint = Map.Vector2ToPoint(u.TilePosition);

          var uPoints = new List<Point>()
          {
            uPoint + new Point(0, -1),   // Top
            uPoint + new Point(1, -1),   // Top-Right
            uPoint + new Point(1, 0),    // Right
            uPoint + new Point(1, 1),    // Bottom-Right
            uPoint + new Point(0, 1),    // Bottom
            uPoint + new Point(-1, 1),   // Bottom-Left
            uPoint + new Point(-1, 0),   // Left
            uPoint + new Point(-1, -1),  // Top-Left
          };

          int distance = unitDistance;
          bool isValid = false;
          foreach (var p in uPoints)
          {
            var result = PathFinder.Find(_mapManager.Map.Get(), unitPoint, p);

            if (result.Status == PathStatus.Valid)
            {
              if (result.Path.Count <= distance)
              {
                distance = result.Path.Count;
                isValid = true;
              }
            }
          }

          if (isValid)
            tempUnits.Add(new Tuple<int, Unit>(distance, u));
        }

        targets.AddRange(tempUnits.OrderBy(c => c.Item1).Select(c => c.Item2));
      };

      Action<List<Unit>> setRangedTargets = (cTargets) =>
      {
        var tempUnits = new List<Tuple<float, Unit>>();

        foreach (var target in cTargets)
        {
          var targetPoint = Map.Vector2ToPoint(target.TilePosition);
          var distance = Vector2.Distance(unitPoint.ToVector2(), targetPoint.ToVector2());

          if (distance < 10)
            tempUnits.Add(new Tuple<float, Unit>(distance, target));

          // if the player 'can see' the unit
          //  then add the unit
        }

        targets.AddRange(tempUnits.OrderBy(c => c.Item1).Select(c => c.Item2));
      };

      foreach (var ability in unit.UnitModel.Abilities.Get())
      {
        targets = new List<Unit>();

        ability.IsEnabled = false;

        switch (ability.AbilityType)
        {
          case Lib.Models.AbilityModel.AbilityTypes.Close when (ability.TargetType == Lib.Models.AbilityModel.TargetTypes.Enemies):
            setCloseTargets(_enemies);
            break;

          case Lib.Models.AbilityModel.AbilityTypes.Close when (ability.TargetType == Lib.Models.AbilityModel.TargetTypes.Friendlies):
            setCloseTargets(_units);
            break;

          case Lib.Models.AbilityModel.AbilityTypes.Close when (ability.TargetType == Lib.Models.AbilityModel.TargetTypes.All):
            setCloseTargets(_enemies);
            setCloseTargets(_units);
            break;

          case Lib.Models.AbilityModel.AbilityTypes.Ranged when (ability.TargetType == Lib.Models.AbilityModel.TargetTypes.Enemies):
            setRangedTargets(_enemies);
            break;

          case Lib.Models.AbilityModel.AbilityTypes.Ranged when (ability.TargetType == Lib.Models.AbilityModel.TargetTypes.Friendlies):
            setRangedTargets(_units);
            break;

          case Lib.Models.AbilityModel.AbilityTypes.Ranged when (ability.TargetType == Lib.Models.AbilityModel.TargetTypes.All):
            setRangedTargets(_enemies);
            setRangedTargets(_units);
            break;

          case Lib.Models.AbilityModel.AbilityTypes.Self:

            targets.Add(unit);
            break;

          default:
            throw new Exception($"Unexpected ability/target combonation: {ability.AbilityType}/{ability.TargetType}");
        }

        ability.Targets = targets.Select(c => c.UnitModel).ToList();

        if (ability.Targets.Count > 0)
          ability.IsEnabled = true;
      }
    }

    private void OnTargetSelect()
    {
      if (_gui.TargetsGroup.PreviousIndex == _gui.TargetsGroup.CurrentIndex)
      {
        var unit = _units[_gui.SelectedUnitIndex];

        unit.UnitModel.Stamina -= unit.UnitModel.Abilities.Get(_gui.SelectedAbilityIndex).StaminaCost;

        _unitManager.State = UnitManager.States.Moving;
        _targets = new List<Unit>();
        _gui.HoveringTargetIndex = -1;

        _abilitySelected = false;

        return;
      }

      _camera.GoTo(_targets[_gui.SelectedTargetIndex].TileRectangle);
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

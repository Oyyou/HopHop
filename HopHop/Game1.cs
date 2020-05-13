using Engine.Input;
using Engine.Models;
using HopHop.Lib;
using HopHop.Lib.Models;
using HopHop.Managers;
using HopHop.MapStuff;
using HopHop.States;
using HopHop.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace HopHop
{
  /// <summary>
  /// This is the main type for your game.
  /// </summary>
  public class Game1 : BaseGame
  {
    private List<Unit> _units;

    private State _state;

    public Game1()
    {
      _graphics = new GraphicsDeviceManager(this);
      Content.RootDirectory = "Content";
    }

    /// <summary>
    /// Allows the game to perform any initialization it needs to before starting to run.
    /// This is where it can query for any required services and load any non-graphic
    /// related content.  Calling base.Initialize will enumerate through any components
    /// and initialize them as well.
    /// </summary>
    protected override void Initialize()
    {
      IsMouseVisible = true;

      Random = new Random();
      GameMouse = new GameMouse();
      GameKeyboard = new GameKeyboard();

      _graphics.PreferredBackBufferWidth = 1280;
      _graphics.PreferredBackBufferHeight = 720;
      _graphics.ApplyChanges();

      ScreenWidth = _graphics.PreferredBackBufferWidth;
      ScreenHeight = _graphics.PreferredBackBufferHeight;

      base.Initialize();
    }

    /// <summary>
    /// LoadContent will be called once per game and is the place to load
    /// all of your content.
    /// </summary>
    protected override void LoadContent()
    {
      // Create a new SpriteBatch, which can be used to draw textures.
      _spriteBatch = new SpriteBatch(GraphicsDevice);

      GameModel = new GameModel(Content, _graphics, _spriteBatch);

      var testTexture = Content.Load<Texture2D>("Enemy");

      _units = new List<Unit>()
      {
        new Unit(testTexture)
        {
          TilePosition = Map.PointToVector2(1, 2),
          UnitModel = new UnitModel()
          {
            Name = "Fred",
            Armour = 0,
            Health = 4,
            Speed = 5,
            UnitType = Lib.Models.UnitModel.UnitTypes.Friendly,
            Abilities = new AbilitiesModel()
            {
              Ability1 = new AbilityModel("Ability 1", "Slash", 2, AbilityModel.TargetTypes.Enemies, AbilityModel.AbilityTypes.Close),
              Ability2 = new AbilityModel("Ability 2", "Heal", 1, AbilityModel.TargetTypes.Friendlies, AbilityModel.AbilityTypes.Close),
              Ability3 = new AbilityModel("Ability 3", "Sling", 2, AbilityModel.TargetTypes.Enemies, AbilityModel.AbilityTypes.Ranged),
              Ability4 = new AbilityModel("Ability 4", "Meditate", 1, AbilityModel.TargetTypes.Self, AbilityModel.AbilityTypes.Self),
            }
          },
          Layer = 0.6f,
        },
        new Unit(testTexture)
        {
          TilePosition = Map.PointToVector2(4, 2),
          UnitModel = new UnitModel()
          {
            Armour = 0,
            Health = 4,
            Speed = 5,
            UnitType = Lib.Models.UnitModel.UnitTypes.Friendly,
            Abilities = new AbilitiesModel()
            {
              Ability1 = new AbilityModel("Ability 1", "Slash", 2, AbilityModel.TargetTypes.Enemies, AbilityModel.AbilityTypes.Close),
              Ability2 = new AbilityModel("Ability 2", "Heal", 1, AbilityModel.TargetTypes.Friendlies, AbilityModel.AbilityTypes.Close),
              Ability3 = new AbilityModel("Ability 3", "Sling", 2, AbilityModel.TargetTypes.Enemies, AbilityModel.AbilityTypes.Ranged),
              Ability4 = new AbilityModel("Ability 4", "Meditate", 1, AbilityModel.TargetTypes.Self, AbilityModel.AbilityTypes.Self),
            }
          },
          Layer = 0.6f,
        },
        new Unit(testTexture)
        {
          TilePosition = Map.PointToVector2(2, 3),
          UnitModel = new UnitModel()
          {
            Armour = 0,
            Health = 4,
            Speed = 5,
            UnitType = Lib.Models.UnitModel.UnitTypes.Friendly,
            Abilities = new AbilitiesModel()
            {
              Ability1 = new AbilityModel("Ability 1", "Slash", 2, AbilityModel.TargetTypes.Enemies, AbilityModel.AbilityTypes.Close),
              Ability2 = new AbilityModel("Ability 2", "Heal", 1, AbilityModel.TargetTypes.Friendlies, AbilityModel.AbilityTypes.Close),
              Ability3 = new AbilityModel("Ability 3", "Sling", 2, AbilityModel.TargetTypes.Enemies, AbilityModel.AbilityTypes.Ranged),
              Ability4 = new AbilityModel("Ability 4", "Meditate", 1, AbilityModel.TargetTypes.Self, AbilityModel.AbilityTypes.Self),
            }
          },
          Layer = 0.6f,
        },
        new Unit(testTexture)
        {
          TilePosition = Map.PointToVector2(0, 5),
          UnitModel = new UnitModel()
          {
            Armour = 0,
            Health = 4,
            Speed = 5,
            UnitType = Lib.Models.UnitModel.UnitTypes.Friendly,
            Abilities = new AbilitiesModel()
            {
              Ability1 = new AbilityModel("Ability 1", "Slash", 2, AbilityModel.TargetTypes.Enemies, AbilityModel.AbilityTypes.Close),
              Ability2 = new AbilityModel("Ability 2", "Heal", 1, AbilityModel.TargetTypes.Friendlies, AbilityModel.AbilityTypes.Close),
              Ability3 = new AbilityModel("Ability 3", "Sling", 2, AbilityModel.TargetTypes.Enemies, AbilityModel.AbilityTypes.Ranged),
              Ability4 = new AbilityModel("Ability 4", "Meditate", 1, AbilityModel.TargetTypes.Self, AbilityModel.AbilityTypes.Self),
            }
          },
          Layer = 0.6f,
        },
      };

      _state = new BattleState(GameModel, _units);
      _state.LoadContent();
    }

    /// <summary>
    /// UnloadContent will be called once per game and is the place to unload
    /// game-specific content.
    /// </summary>
    protected override void UnloadContent()
    {
      _units.Clear();
      _state.UnloadContent();
    }

    /// <summary>
    /// Allows the game to run logic such as updating the world,
    /// checking for collisions, gathering input, and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Update(GameTime gameTime)
    {
      GameMouse.Update();
      GameKeyboard.Update();

      _state.Update(gameTime);

      base.Update(gameTime);
    }

    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
      GraphicsDevice.Clear(Color.CornflowerBlue);

      _state.Draw(gameTime);

      base.Draw(gameTime);
    }
  }
}

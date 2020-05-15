using Engine.Input;
using Engine.Models;
using HopHop.Lib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace HopHop.GUI
{
  /// <summary>
  /// This is the main type for your game.
  /// </summary>
  public class GUIGame1 : BaseGame
  {
    private BattleGUI _battleGUI;

    public GUIGame1()
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

      _battleGUI = new BattleGUI(GameModel, new System.Collections.Generic.List<Lib.Models.UnitModel>());
    }

    /// <summary>
    /// UnloadContent will be called once per game and is the place to unload
    /// game-specific content.
    /// </summary>
    protected override void UnloadContent()
    {
      // TODO: Unload any non ContentManager content here
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

      _battleGUI.Update(gameTime, new System.Collections.Generic.List<Lib.Models.UnitModel>());

      base.Update(gameTime);
    }

    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
      GraphicsDevice.Clear(Color.CornflowerBlue);

      _battleGUI.Draw(gameTime, _spriteBatch);

      base.Draw(gameTime);
    }
  }
}

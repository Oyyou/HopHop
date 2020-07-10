using System;
using Engine.Models;
using Microsoft.Xna.Framework;
using HopHop.Sprites.Home;
using Microsoft.Xna.Framework.Graphics;
using HopHop.Managers;
using HopHop.Lib;
using HopHop.Sprites;
using HopHop.MapStuff;

namespace HopHop.States
{
  public class HomeState : State
  {
    public enum States
    {
      Outside,
      EnteringBuilding,
      Inside,
    }

    private Sprite _insideBackground;

    private Player _player;

    private Building _house;

    private MapManager _mapManager;

    public States State = States.Outside;

    public HomeState(GameModel gameModel)
      : base(gameModel)
    {
    }

    public override void LoadContent()
    {
      _mapManager = new MapManager(Content);

      _player = new Player(Content.Load<Texture2D>("Enemy"), _mapManager)
      {
        Speed = 4,
        TilePosition = new Vector2(40, 40),
      };

      _house = new Building(Content.Load<Texture2D>("Buildings/House_01"))
      {
        TilePosition = new Vector2(200, 200),
        TileRectangleTopOffset = 120,
      };

      var pixel = new Texture2D(GraphicsDeviceManager.GraphicsDevice, 1, 1);
      pixel.SetData(new Color[] { Color.Black });

      _insideBackground = new Sprite(pixel)
      {
        Opacity = 0.8f,
        SourceRectangle = new Rectangle(0, 0, BaseGame.ScreenWidth, BaseGame.ScreenHeight),
        HasFixedLayer = true,
        Layer = 0.9f,
        TilePositionOffset = 0,
      };

      _mapManager.Refresh = () =>
      {
        _mapManager.Map.Clear();

        _mapManager.Map.AddItem(_house.TileRectangle);
        _mapManager.Map.RemoveItem(_house.EntranceRectangle);

        _mapManager.Map.AddItem(_player.TileRectangle);

        //_mapManager.Map.Write();
      };

      _mapManager.Refresh();
    }

    public override void UnloadContent()
    {
      _mapManager.UnloadContent();
    }

    public override void Update(GameTime gameTime)
    {
      switch (State)
      {
        case States.Outside:

          _player.Update(gameTime);
          _mapManager.Update(gameTime);

          if (_house.EntranceRectangle.Intersects(_player.NextRectangleTile))
          {
            State = States.EnteringBuilding;
            _insideBackground.Opacity = 0.0f;
          }

          break;

        case States.EnteringBuilding:

          _player.Update(gameTime);
          _mapManager.Update(gameTime);

          var diff = Map.TileHeight / _player.Speed;
          _insideBackground.Opacity += 0.8f / diff;

          if (_player.TileRectangle == _house.EntranceRectangle)
          {
            _insideBackground.Opacity = 0.8f;
            State = States.Inside;
          }

          break;

        case States.Inside:

          if (BaseGame.GameKeyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
          {
            State = States.Outside;
            _mapManager.Map.RemoveItem(_player.TileRectangle);
            _player.Move(Directions.Down);
          }

          break;

        default:
          break;
      }
    }

    public override void Draw(GameTime gameTime)
    {
      SpriteBatch.Begin(SpriteSortMode.FrontToBack);

      switch (State)
      {
        case States.Outside:

          _mapManager.Draw(gameTime, SpriteBatch);
          _house.Draw(gameTime, SpriteBatch);
          _player.Draw(gameTime, SpriteBatch);

          break;
        case States.EnteringBuilding:

          _mapManager.Draw(gameTime, SpriteBatch);
          _house.Draw(gameTime, SpriteBatch);
          _player.Draw(gameTime, SpriteBatch);

          _insideBackground.Draw(gameTime, SpriteBatch);

          break;
        case States.Inside:

          _mapManager.Draw(gameTime, SpriteBatch);
          _house.Draw(gameTime, SpriteBatch);

          _insideBackground.Draw(gameTime, SpriteBatch);

          break;
      }

      SpriteBatch.End();
    }
  }
}

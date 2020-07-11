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

    private Sprite _insideFog;

    private Sprite _insideBackground;

    private Player _playerOutside;

    private Player _playerInside;

    private Building _house;

    private MapManager _mapManagerOutside;

    private MapManager _mapManagerInside;

    public States State = States.Outside;

    public HomeState(GameModel gameModel)
      : base(gameModel)
    {
    }

    public override void LoadContent()
    {
      _mapManagerOutside = new MapManager(Content);
      _mapManagerInside = new MapManager(Content, BaseGame.ScreenWidth / Map.TileWidth, BaseGame.ScreenHeight / Map.TileHeight);

      _playerOutside = new Player(Content.Load<Texture2D>("Enemy"), _mapManagerOutside)
      {
        Speed = 4,
        TilePosition = new Vector2(40, 40),
      };

      _playerInside = new Player(Content.Load<Texture2D>("Enemy"), _mapManagerInside)
      {
        Speed = 4,
        TilePosition = new Vector2(240, 240),
      };

      var pixel = new Texture2D(GraphicsDeviceManager.GraphicsDevice, 1, 1);
      pixel.SetData(new Color[] { Color.Black });

      _insideFog = new Sprite(pixel)
      {
        Opacity = 0.8f,
        SourceRectangle = new Rectangle(0, 0, BaseGame.ScreenWidth, BaseGame.ScreenHeight),
        HasFixedLayer = true,
        Layer = 0.9f,
        TilePositionOffset = 0,
      };

      var insideBackgroundText = Content.Load<Texture2D>("Buildings/InsideBackground");

      _insideBackground = new Sprite(insideBackgroundText)
      {
        HasFixedLayer = true,
        Layer = 0.91f,
        TilePositionOffset = 0,
        TilePosition = new Vector2((BaseGame.ScreenWidth / 2) - (insideBackgroundText.Width / 2), (BaseGame.ScreenHeight / 2) - (insideBackgroundText.Height / 2)),
      };

      _house = new Building(Content.Load<Texture2D>("Buildings/House_01"))
      {
        TilePosition = new Vector2(200, 200),
        TileRectangleTopOffset = 120,
        ExitRectangle = new Rectangle((_insideBackground.TileRectangle.Left + (_insideBackground.TileRectangle.Width / 2)) - Map.TileWidth, _insideBackground.TileRectangle.Bottom, (Map.TileWidth * 2), Map.TileHeight),
      };

      _mapManagerOutside.Refresh = () =>
      {
        _mapManagerOutside.Map.Clear();

        _mapManagerOutside.Map.AddItem(_house.TileRectangle);
        _mapManagerOutside.Map.RemoveItem(_house.EntranceRectangle);

        _mapManagerOutside.Map.AddItem(_playerOutside.TileRectangle);

        //_mapManager.Map.Write();
      };

      _mapManagerInside.Refresh = () =>
      {
        _mapManagerInside.Map.Clear();

        _mapManagerInside.Map.AddItem(new Rectangle(0, 0, BaseGame.ScreenWidth, BaseGame.ScreenHeight));
        _mapManagerInside.Map.RemoveItem(_insideBackground.Rectangle);
        _mapManagerInside.Map.RemoveItem(_house.ExitRectangle);

        _mapManagerInside.Map.Write();
      };

      _mapManagerOutside.Refresh();
      _mapManagerInside.Refresh();
    }

    public override void UnloadContent()
    {
      _mapManagerOutside.UnloadContent();
    }

    public override void Update(GameTime gameTime)
    {
      switch (State)
      {
        case States.Outside:

          _playerOutside.Update(gameTime);
          _mapManagerOutside.Update(gameTime);

          if (_house.EntranceRectangle.Intersects(_playerOutside.NextRectangleTile))
          {
            State = States.EnteringBuilding;
            _insideFog.Opacity = 0.0f;
          }

          break;

        case States.EnteringBuilding:

          _playerOutside.Update(gameTime);
          _mapManagerOutside.Update(gameTime);

          var diff = Map.TileHeight / _playerOutside.Speed;
          _insideFog.Opacity += 0.8f / diff;

          if (_playerOutside.TileRectangle == _house.EntranceRectangle)
          {
            _insideFog.Opacity = 0.8f;
            State = States.Inside;

            _mapManagerInside.Refresh();

            _playerInside.TilePosition = new Vector2(_house.ExitRectangle.X, _house.ExitRectangle.Y);
            _playerInside.Move(Directions.Up);
          }

          break;

        case States.Inside:

          _playerInside.Update(gameTime);

          if (BaseGame.GameKeyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape) || _house.ExitRectangle.Intersects(_playerInside.NextRectangleTile))
          {
            _playerInside.NextRectangleTile = Rectangle.Empty;
            State = States.Outside;
            _mapManagerOutside.Map.RemoveItem(_playerOutside.TileRectangle);
            _playerOutside.Move(Directions.Down);
          }

          break;

        default:
          break;
      }
    }

    public override void Draw(GameTime gameTime)
    {

      switch (State)
      {
        case States.Outside:

          SpriteBatch.Begin(SpriteSortMode.FrontToBack);

          _house.Draw(gameTime, SpriteBatch);
          _playerOutside.Draw(gameTime, SpriteBatch);

          SpriteBatch.End();

          break;
        case States.EnteringBuilding:

          SpriteBatch.Begin(SpriteSortMode.FrontToBack);

          _house.Draw(gameTime, SpriteBatch);
          _playerOutside.Draw(gameTime, SpriteBatch);

          _insideFog.Draw(gameTime, SpriteBatch);

          SpriteBatch.End();

          break;
        case States.Inside:

          SpriteBatch.Begin(SpriteSortMode.FrontToBack);
          _house.Draw(gameTime, SpriteBatch);
          SpriteBatch.End();

          SpriteBatch.Begin();
          _insideFog.Draw(gameTime, SpriteBatch);
          _insideBackground.Draw(gameTime, SpriteBatch);
          SpriteBatch.End();

          SpriteBatch.Begin(SpriteSortMode.FrontToBack);
          _playerInside.Draw(gameTime, SpriteBatch);
          SpriteBatch.End();

          break;
      }
    }
  }
}

using System;
using Engine.Models;
using Microsoft.Xna.Framework;
using HopHop.Sprites.Home;
using Microsoft.Xna.Framework.Graphics;
using HopHop.Managers;

namespace HopHop.States
{
  public class HomeState : State
  {
    private Player _player;

    private Building _house;

    private MapManager _mapManager;

    public HomeState(GameModel gameModel) 
      : base(gameModel)
    {
    }

    public override void LoadContent()
    {
      _mapManager = new MapManager(Content);

      _player = new Player(Content.Load<Texture2D>("Enemy"), _mapManager)
      {
        Speed = 7,
        TilePosition = new Vector2(40, 40),
      };

      _house = new Building(Content.Load<Texture2D>("Buildings/House_01"))
      {
        TilePosition = new Vector2(200, 200),
        TileRectangleTopOffset = 120,
      };

      _mapManager.Refresh = () =>
      {
        _mapManager.Map.Clear();

        _mapManager.Map.AddItem(_player.TileRectangle);
        _mapManager.Map.AddItem(_house.TileRectangle);
        _mapManager.Map.RemoveItem(_house.EntranceRectangle);

        _mapManager.Map.Write();
      };

      _mapManager.Refresh();
    }

    public override void UnloadContent()
    {
      _mapManager.UnloadContent();
    }

    public override void Update(GameTime gameTime)
    {
      _player.Update(gameTime);

      _mapManager.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
      SpriteBatch.Begin(SpriteSortMode.FrontToBack);

      _mapManager.Draw(gameTime, SpriteBatch);

      _house.Draw(gameTime, SpriteBatch);

      _player.Draw(gameTime, SpriteBatch);

      SpriteBatch.End();
    }
  }
}

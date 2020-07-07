using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Models;
using Microsoft.Xna.Framework;
using HopHop.Sprites.Home;
using Microsoft.Xna.Framework.Graphics;

namespace HopHop.States
{
  public class HomeState : State
  {
    private Player _player;

    public HomeState(GameModel gameModel) 
      : base(gameModel)
    {
    }

    public override void LoadContent()
    {
      _player = new Player(Content.Load<Texture2D>("Enemy"))
      {
        Speed = 7,
      };
    }

    public override void UnloadContent()
    {
      throw new NotImplementedException();
    }

    public override void Update(GameTime gameTime)
    {
      _player.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
      SpriteBatch.Begin(SpriteSortMode.FrontToBack);

      _player.Draw(gameTime, SpriteBatch);

      SpriteBatch.End();
    }
  }
}

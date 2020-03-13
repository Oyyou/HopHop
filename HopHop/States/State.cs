using Engine.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace HopHop.States
{
  public abstract class State
  {
    protected GameModel _gameModel;

    protected ContentManager Content
    {
      get
      {
        return _gameModel.Content;
      }
    }

    protected GraphicsDeviceManager GraphicsDeviceManager
    {
      get
      {
        return _gameModel.GraphicsDeviceManager;
      }
    }

    protected SpriteBatch SpriteBatch
    {
      get
      {
        return _gameModel.SpriteBatch;
      }
    }

    protected State(GameModel gameModel)
    {
      _gameModel = gameModel;
    }

    public abstract void LoadContent();

    public abstract void UnloadContent();

    public abstract void Update(GameTime gameTime);

    public abstract void Draw(GameTime gameTime);
  }
}

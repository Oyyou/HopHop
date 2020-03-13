using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
  public class GameModel
  {
    public readonly ContentManager Content;

    public readonly GraphicsDeviceManager GraphicsDeviceManager;

    public readonly SpriteBatch SpriteBatch;

    public GameModel(ContentManager content, GraphicsDeviceManager graphicsDeviceManager, SpriteBatch spriteBatch)
    {
      Content = content;
      GraphicsDeviceManager = graphicsDeviceManager;
      SpriteBatch = spriteBatch;
    }
  }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
  public class Sprite
  {
    public Texture2D Texture { get; set; }

    public Vector2 Position { get; set; }

    public Color Colour { get; set; } = Color.White;

    public float Layer { get; set; }

    public Sprite(Texture2D texture)
    {
      Texture = texture;
    }

    public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(Texture, Position, null, Colour, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, Layer);
    }
  }
}

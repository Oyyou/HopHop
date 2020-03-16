using HopHop.MapStuff;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopHop.Sprites
{
  public class Sprite : IMapable
  {
    public Texture2D Texture { get; set; }

    public Vector2 Position
    {
      get
      {
        return new Vector2(TilePosition.X, TilePosition.Y - Map.TileHeight);
      }
    }

    public Rectangle Rectangle
    {
      get
      {
        return new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
      }
    }

    public Rectangle TileRectangle
    {
      get
      {
        return new Rectangle((int)TilePosition.X, (int)TilePosition.Y, Texture.Width, Texture.Height - Map.TileHeight);
      }
    }

    public Vector2 TilePosition { get; set; }

    public float Layer { get; set; }

    public bool HasFixedLayer { get; set; } = false;

    public Sprite(Texture2D texture)
    {
      Texture = texture;
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      if (!HasFixedLayer)
        Layer = 0.2f + MathHelper.Clamp(TilePosition.Y / 1000f, 0.0f, 0.7f);

      spriteBatch.Draw(Texture, Position, null, Color.White, 0f, new Vector2(0, 0), new Vector2(1f, 1f), SpriteEffects.None, Layer);
    }
  }
}

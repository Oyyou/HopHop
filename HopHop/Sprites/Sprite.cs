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
        return new Vector2(TilePosition.X, TilePosition.Y - TilePositionOffset);
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
        var test = Texture.Height % Map.TileHeight;

        return new Rectangle((int)TilePosition.X, (int)TilePosition.Y + TileRectangleTopOffset, Texture.Width, (Texture.Height - TilePositionOffset) - TileRectangleTopOffset);
      }
    }

    public Rectangle? SourceRectangle;

    public float Opacity = 1f;

    public int TileRectangleTopOffset = 0;

    public int TilePositionOffset = Map.TileHeight;

    public Vector2 TilePosition { get; set; }

    public float Layer { get; set; }

    public bool HasFixedLayer { get; set; } = false;

    public Sprite(Texture2D texture)
    {
      Texture = texture;
    }

    public virtual void Update(GameTime gameTime)
    {

    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      if (!HasFixedLayer)
        Layer = 0.2f + MathHelper.Clamp(TileRectangle.Y / 1000f, 0.0f, 0.7f);

      spriteBatch.Draw(Texture, Position, SourceRectangle, Color.White * Opacity, 0f, new Vector2(0, 0), new Vector2(1f, 1f), SpriteEffects.None, Layer);
    }
  }
}

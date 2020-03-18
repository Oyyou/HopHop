using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HopHop.MapStuff;

namespace HopHop.Sprites
{
  public class UnitPointer : Engine.Sprite
  {
    private bool _goingDown;
    private float _distance = 0;

    private Vector2 _additionalPosition;

    private int _x;

    public UnitPointer(Texture2D texture)
      : base(texture)
    {
      Layer = 1f;

      _x = (Map.TileWidth / 2) - (texture.Width / 2);
    }

    public void Update(GameTime gameTime, Vector2 position)
    {
      Position = position;

      _distance += _goingDown ? 0.25f : -0.25f;

      if (Math.Abs(_distance) >= 10)
        _goingDown = !_goingDown;

      _additionalPosition = new Vector2(0, _distance);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(Texture, Position + new Vector2(_x, -20) + _additionalPosition, null, Colour, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, Layer);
    }
  }
}

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace HopHop.Sprites
{
  public class TilePointer : Engine.Sprite
  {
    private Texture2D _outerTexture;

    private Vector2 _origin = new Vector2(20, 20);

    private float _scale = 1f;

    private bool _increasing = true;

    public TilePointer(Texture2D innerTexture, Texture2D outerTexture) :
      base(innerTexture)
    {
      _outerTexture = outerTexture;
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      base.Draw(gameTime, spriteBatch);

      var speed = 0.005f;
      var change = 0.1f;

      if (_increasing)
        _scale += speed;
      else _scale -= speed;

      if (_scale > (1+ change) || _scale < (1 - change))
        _increasing = !_increasing;

      spriteBatch.Draw(_outerTexture, Position + _origin, null, Colour, 0f, _origin, _scale, SpriteEffects.None, Layer);
    }
  }
}

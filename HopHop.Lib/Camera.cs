using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopHop.Lib
{
  public class Camera
  {
    private Vector2? _moveTo;

    public Vector2 Position;

    public Matrix Transform { get; set; }

    private float _smoothTime = 0.2f;

    public Camera()
    {
    }

    public void Update(GameTime gameTime)
    {
      var speed = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 5;

      if (_moveTo != null)
      {
        Position = Vector2.Lerp(Position, _moveTo.Value, _smoothTime);

        if (Vector2.Distance(Position, _moveTo.Value) < 1f)
        {
          _moveTo = null;
        }
      }
      else
      {
        if (BaseGame.GameKeyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
          speed *= 2.0f;

        if (BaseGame.GameKeyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D))
          Position.X += speed;
        else if (BaseGame.GameKeyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A))
          Position.X -= speed;

        if (BaseGame.GameKeyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W))
          Position.Y -= speed;
        else if (BaseGame.GameKeyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.S))
          Position.Y += speed;
      }

      var offset = Matrix.CreateTranslation(
          BaseGame.ScreenWidth / 2,
          BaseGame.ScreenHeight / 2,
          0);

      Transform = Matrix.CreateTranslation(-Position.X, -Position.Y, 0) *
        offset;
    }

    public void GoTo(Rectangle rectangle)
    {
      _moveTo = new Vector2(
        (rectangle.X + (rectangle.Width / 2)),
        (rectangle.Y + (rectangle.Height / 2)) + (BaseGame.ScreenHeight / 6));
    }
  }
}

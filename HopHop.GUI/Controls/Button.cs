using Engine.Input;
using HopHop.Lib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopHop.GUI.Controls
{
  public class Button : IClickable
  {
    private Texture2D _texture;

    private Texture2D _clickTexture;

    private SpriteFont _font;

    protected Color _colour = Color.White;

    public Action Click;

    public bool Clicked { get; protected set; }

    public bool IsSelected { get; set; }

    public Vector2 Position { get; set; }

    public Rectangle Rectangle
    {
      get
      {
        return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);
      }
    }


    public Vector2 Origin { get; set; }

    public string Text { get; set; }

    public Color PenColour { get; set; } = Color.White;

    public bool IsClicked
    {
      get
      {
        return IsHovering && (BaseGame.GameMouse.HasLeftClicked || BaseGame.GameMouse.HasRightClicked);
      }
    }

    public bool IsHovering
    {
      get
      {
        return BaseGame.GameMouse.Intersects(Rectangle);
      }
    }

    public bool IsEnabled { get; set; } = true;

    public Color HoverColour { get; set; } = Color.Yellow;

    public Color DisabledColour { get; set; } = Color.Gray;

    public float Layer { get; set; } = 1f;

    public Button(Texture2D texture)
    {
      _texture = texture;

      Origin = new Vector2(0, 0);
    }

    public Button(Texture2D texture, Texture2D clickTexture)
    {
      _texture = texture;
      _clickTexture = clickTexture;

      Origin = new Vector2(0, 0);
    }

    public Button(Texture2D texture, SpriteFont font)
      : this(texture)
    {
      _font = font;
    }

    public void UnloadContent()
    {
      _texture.Dispose();
      _clickTexture?.Dispose();
    }

    public void Update(GameTime gameTime)
    {
      _colour = Color.White;

      if (BaseGame.GameMouse.Intersects(this.Rectangle))
        BaseGame.GameMouse.AddObject(this);
      else BaseGame.GameMouse.ClickableObjects.Remove(this);

      if (!IsEnabled)
      {
        _colour = DisabledColour;
        return;
      }

      Clicked = false;

      if (IsHovering)
      {
        _colour = HoverColour;

        if (IsClicked)
        {
          Clicked = true;
          OnClick();
        }
      }

      //if (IsHovering)
      //  OnHover();
      //else OffHover();
    }

    public virtual void OnClick()
    {
      Click?.Invoke();

      //IsSelected = true;
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(_texture, Position, null, _colour, 0f, Origin, 1f, SpriteEffects.None, 0);

      if (IsSelected && _clickTexture != null)
        spriteBatch.Draw(_clickTexture, Position, null, _colour, 0f, Origin, 1f, SpriteEffects.None, 0);

      DrawText(spriteBatch);
    }

    protected void DrawText(SpriteBatch spriteBatch)
    {
      if (string.IsNullOrEmpty(Text) || _font == null)
        return;

      float x = ((Rectangle.X + (Rectangle.Width / 2)) - (_font.MeasureString(Text).X / 2)) - Origin.X;
      float y = ((Rectangle.Y + (Rectangle.Height / 2)) - (_font.MeasureString(Text).Y / 2)) - Origin.Y;

      spriteBatch.DrawString(_font, Text, new Vector2(x, y), PenColour, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0.1f);
    }
  }
}

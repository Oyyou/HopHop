using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Input;
using Microsoft.Xna.Framework.Input;
using HopHop.Lib;
using HopHop.MapStuff;
using HopHop.Managers;

namespace HopHop.Sprites.Home
{
  public enum Directions
  {
    Up,
    Down,
    Left,
    Right,
  }

  public class Player : Sprite
  {
    private readonly MapManager _mapManager;

    private Vector2 _direction = Vector2.Zero;
    private int _distanceTravelling = 0;

    private int _speed = 4;

    public int Speed
    {
      get { return _speed; }
      set
      {
        _speed = MathHelper.Clamp(value, 1, Map.TileHeight);

        while (Map.TileHeight % _speed != 0)
        {
          _speed--;
        }
      }
    }

    public Player(Texture2D texture, MapManager map)
      : base(texture)
    {
      _mapManager = map;
    }

    public override void Update(GameTime gameTime)
    {
      if (_distanceTravelling > 0)
      {
        TilePosition += (_direction * Speed);

        _distanceTravelling -= Speed;

        if (_distanceTravelling == 0)
        {
          _direction = Vector2.Zero;
          _mapManager.Refresh();
        }
      }
      else
      {
        if (BaseGame.GameKeyboard.IsKeyDown(Keys.W))
          _direction = new Vector2(0, -1);
        else if (BaseGame.GameKeyboard.IsKeyDown(Keys.S))
          _direction = new Vector2(0, 1);
        else if (BaseGame.GameKeyboard.IsKeyDown(Keys.A))
          _direction = new Vector2(-1, 0);
        else if (BaseGame.GameKeyboard.IsKeyDown(Keys.D))
          _direction = new Vector2(1, 0);

        if (_direction != Vector2.Zero)
        {
          _distanceTravelling = Map.TileHeight;

          // We add a temp position to the 
          var newPosition = TilePosition + (_direction * Map.TileHeight);
          var newRectangle = new Rectangle((int)newPosition.X, (int)newPosition.Y, Map.TileWidth, Map.TileHeight);

          var value = _mapManager.Map.GetValue(newRectangle);

          if (value == Map.CollisionResults.None)
          {
            _mapManager.Map.AddItem(newRectangle);
            _mapManager.Map.Write();
          }
          else
          {
            _distanceTravelling = 0;
          }
        }
      }
    }
  }
}

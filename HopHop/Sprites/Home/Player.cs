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
using Engine;

namespace HopHop.Sprites.Home
{
  public enum Directions
  {
    Up,
    Down,
    Left,
    Right,
    None,
  }

  public class Player : Sprite
  {
    private readonly MapManager _mapManager;

    private Vector2 _direction = Vector2.Zero;
    private int _distanceTravelling = 0;

    private int _speed = 4;

    public Rectangle PreviousRectangleTile;
    public Rectangle NextRectangleTile;

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
          _mapManager.Map.RemoveItem(PreviousRectangleTile);
          PreviousRectangleTile = Rectangle.Empty;
          NextRectangleTile = Rectangle.Empty;
          //_mapManager.Refresh();
        }
      }

      if (_distanceTravelling == 0)
      {
        var direction = Directions.None;

        if (BaseGame.GameKeyboard.IsKeyDown(Keys.W))
          direction = Directions.Up;
        else if (BaseGame.GameKeyboard.IsKeyDown(Keys.S))
          direction = Directions.Down;
        else if (BaseGame.GameKeyboard.IsKeyDown(Keys.A))
          direction = Directions.Left;
        else if (BaseGame.GameKeyboard.IsKeyDown(Keys.D))
          direction = Directions.Right;

        Move(direction);

        if (_direction != Vector2.Zero)
        {
          _distanceTravelling = Map.TileHeight;

          // We add a temp position to the map
          var newPosition = TilePosition + (_direction * Map.TileHeight);
          PreviousRectangleTile = new Rectangle((int)TilePosition.X, (int)TilePosition.Y, Map.TileWidth, Map.TileHeight);
          NextRectangleTile = new Rectangle((int)newPosition.X, (int)newPosition.Y, Map.TileWidth, Map.TileHeight);

          var value = _mapManager.Map.GetValue(NextRectangleTile);

          if (value == Map.CollisionResults.None)
          {
            _mapManager.Map.AddItem(NextRectangleTile);
            //_mapManager.Map.Write();
          }
          else
          {
            _distanceTravelling = 0;
            PreviousRectangleTile = Rectangle.Empty;
            NextRectangleTile = Rectangle.Empty;
          }
        }
      }
    }

    public void Move(Directions direction)
    {
      switch (direction)
      {
        case Directions.Up:
          _direction = new Vector2(0, -1);
          break;
        case Directions.Down:
          _direction = new Vector2(0, 1);
          break;
        case Directions.Left:
          _direction = new Vector2(-1, 0);
          break;
        case Directions.Right:
          _direction = new Vector2(1, 0);
          break;
      }

      _distanceTravelling = Map.TileHeight;
    }
  }
}

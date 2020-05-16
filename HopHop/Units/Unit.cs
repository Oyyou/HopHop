using Engine.Input;
using HopHop.Lib.Models;
using HopHop.MapStuff;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopHop.Units
{
  public class Unit : IClickable, IMapable
  {
    protected Color _colour = Color.White;

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
        return new Rectangle((int)TilePosition.X, (int)TilePosition.Y, Map.TileWidth, Map.TileHeight);
      }
    }

    public List<Point> MovementPositions { get; private set; } = new List<Point>();

    public List<Tuple<int, Point>> PotentialPoints = new List<Tuple<int, Point>>();

    public Vector2 TilePosition { get; set; }

    public float Layer { get; set; }

    public UnitModel UnitModel { get; set; }

    public int TilesMoved { get; set; } = 0;

    public Color Colour { get; set; } = Color.White;

    public Unit(Texture2D texture)
    {
      Texture = texture;
    }

    public void SetPath(List<Point> points)
    {
      int max = UnitModel.Speed * UnitModel.Stamina;

      MovementPositions = points.GetRange(0, points.Count > max ? max : points.Count);
    }

    internal void Move()
    {
      PotentialPoints = new List<Tuple<int, Point>>();

      if (MovementPositions.Count == 0)
        return;

      var point = MovementPositions.First();
      var targetPosition = Map.PointToVector2(point.X, point.Y);

      var movement = new Vector2();
      int speed = 4;
      if (targetPosition.X < TilePosition.X)
        movement.X = -speed;
      else if (targetPosition.X > TilePosition.X)
        movement.X = speed;
      
      if (targetPosition.Y < TilePosition.Y)
        movement.Y = -speed;
      else if (targetPosition.Y > TilePosition.Y)
        movement.Y = speed;

      TilePosition += movement;

      if (TilePosition == targetPosition)
      {
        MovementPositions.RemoveAt(0);
        TilesMoved++;
      }
    }

    public void UpdateStamina()
    {
      var value = (int)Math.Ceiling((decimal)TilesMoved / UnitModel.Speed);
      UnitModel.Stamina -= value;
    }

    public void Update(GameTime gameTime)
    {
      //_colour = Color.White;
      Colour = Color.White;

      if (Game1.GameMouse.Intersects_withCamera(this.Rectangle))
        Game1.GameMouse.AddObject(this);
      else Game1.GameMouse.ClickableObjects.Remove(this);

      //if (Game1.GameMouse.ValidObject == this)
      //  _colour = Color.Gray;
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      Layer = 0.2f + MathHelper.Clamp(TilePosition.Y / 1000f, 0.0f, 0.7f);

      spriteBatch.Draw(Texture, Position, null, Colour, 0f, new Vector2(0, 0), new Vector2(1f, 1f), SpriteEffects.None, Layer);
    }
  }
}

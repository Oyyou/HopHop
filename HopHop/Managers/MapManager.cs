using Engine;
using HopHop.MapStuff;
using HopHop.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HopHop.Managers
{
  public class MapManager
  {
    private Unit _validUnit;

    private List<Sprite> _sprites;

    private List<Sprite> _potentialPathSprites;

    private readonly List<Sprite> _gridSprites;

    private readonly Texture2D _tileTexture;

    private readonly Texture2D _tileBorderTexture;

    public readonly Map Map;

    public Action Refresh;

    private Dictionary<string, Texture2D> _paths = new Dictionary<string, Texture2D>();

    public MapManager(ContentManager content)
    {
      Map = new Map(20, 20);

      _tileTexture = content.Load<Texture2D>("Tiles/Tile");
      _tileBorderTexture = content.Load<Texture2D>("Tiles/TileBorder");

      _gridSprites = new List<Sprite>();
      _sprites = new List<Sprite>();

      _paths.Add("LD", content.Load<Texture2D>("Tiles/Path/LD"));
      _paths.Add("LR", content.Load<Texture2D>("Tiles/Path/LR"));
      _paths.Add("LU", content.Load<Texture2D>("Tiles/Path/LU"));
      _paths.Add("RD", content.Load<Texture2D>("Tiles/Path/RD"));
      _paths.Add("RU", content.Load<Texture2D>("Tiles/Path/RU"));
      _paths.Add("UD", content.Load<Texture2D>("Tiles/Path/UD"));
      _paths.Add("Last_Outer", content.Load<Texture2D>("Tiles/Path/Last_Outer"));
      _paths.Add("Last_Inner", content.Load<Texture2D>("Tiles/Path/Last_Inner"));
      _paths.Add("Tile", content.Load<Texture2D>("Tiles/Tile"));

      for (int y = 0; y < Map.GetHeight(); y++)
      {
        for (int x = 0; x < Map.GetWidth(); x++)
        {
          _gridSprites.Add(new Sprite(_tileBorderTexture)
          {
            Colour = Color.White,
            Position = Map.PointToVector2(x, y),
            Layer = 0.1f
          });
        }
      }
    }

    public void SetUnit(Unit unit)
    {
      _validUnit = unit;
    }

    public void UnloadContent()
    {
      _sprites.Clear();
      _gridSprites.Clear();
      _tileTexture.Dispose();
      _tileBorderTexture.Dispose();
    }

    private Point _currentPosition;
    private Point _previousPosition;

    private List<Tuple<int, Point>> _potentialPoints;

    public void Update(GameTime gameTime)
    {
      if (_validUnit == null)
        return;

      if (_potentialPoints != _validUnit.PotentialPoints)
      {
        _potentialPoints = _validUnit.PotentialPoints;
        _potentialPathSprites = new List<Sprite>();

        foreach (var point in _potentialPoints)
        {
          _potentialPathSprites.Add(new Sprite(_paths["Tile"])
          {
            Position = Map.PointToVector2(point.Item2.X, point.Item2.Y),
            Colour = point.Item1 > _validUnit.UnitModel.Speed ? Color.Orange : Color.Green,
            Layer = 0.049f,
          });
        }
      }

      if (_validUnit.MovementPositions.Count == 0)
        return;

      _previousPosition = _currentPosition;
      _currentPosition = _validUnit.MovementPositions.Last();

      if (_previousPosition == _currentPosition)
        return;

      _sprites = new List<Sprite>();

      var speed = _validUnit.UnitModel.Speed;
      var stamina = _validUnit.UnitModel.Stamina;

      for (int i = 0; i < _validUnit.MovementPositions.Count; i++)
      {
        var p = _validUnit.MovementPositions[i];

        var path = _validUnit.MovementPositions[i];

        var colour = Color.Green;

        //if (i >= (speed - _validUnit.TilesMoved))
        //  colour = Color.Orange;

        switch (stamina)
        {
          case 2:

            if (i >= (speed - _validUnit.TilesMoved))
              colour = Color.Orange;
            break;

          case 1:

            colour = Color.Orange;
            break;
        }

        var previousPoint = Map.Vector2ToPoint(_validUnit.TilePosition);
        var currentPoint = new Point(path.X, path.Y);
        var nextPoint = Point.Zero;

        if (i > 0)
          previousPoint = _validUnit.MovementPositions[i - 1];

        if (i < _validUnit.MovementPositions.Count - 1)
          nextPoint = _validUnit.MovementPositions[i + 1];

        Texture2D texture = null;

        // If this is the last point
        if (nextPoint == Point.Zero)
        {
          if (previousPoint.Y != currentPoint.Y)
            texture = _paths["UD"];
          else
            texture = _paths["LR"];
        }
        else
        {
          if (previousPoint.Y < currentPoint.Y)
          {
            if (nextPoint.Y > currentPoint.Y)
              texture = _paths["UD"];
            else if (nextPoint.X < currentPoint.X)
              texture = _paths["LU"];
            else if (nextPoint.X > currentPoint.X)
              texture = _paths["RU"];
            else
              throw new Exception("wut");
          }
          else if (previousPoint.Y > currentPoint.Y)
          {
            if (nextPoint.Y < currentPoint.Y)
              texture = _paths["UD"];
            else if (nextPoint.X < currentPoint.X)
              texture = _paths["LD"];
            else if (nextPoint.X > currentPoint.X)
              texture = _paths["RD"];
            else
              throw new Exception("wut");
          }
          else if (previousPoint.X < currentPoint.X)
          {
            if (nextPoint.X > currentPoint.X)
              texture = _paths["LR"];
            else if (nextPoint.Y < currentPoint.Y)
              texture = _paths["LU"];
            else if (nextPoint.Y > currentPoint.Y)
              texture = _paths["LD"];
            else
              throw new Exception("wut");
          }
          else if (previousPoint.X > currentPoint.X)
          {
            if (nextPoint.X < currentPoint.X)
              texture = _paths["LR"];
            else if (nextPoint.Y < currentPoint.Y)
              texture = _paths["RU"];
            else if (nextPoint.Y > currentPoint.Y)
              texture = _paths["RD"];
            else
              throw new Exception("wut");
          }
        }


        if (i == _validUnit.MovementPositions.Count - 1)
        {
          _sprites.Add(new Sprites.TilePointer(_paths["Last_Inner"], _paths["Last_Outer"])
          {
            Colour = colour,
            Position = Map.PointToVector2(currentPoint.X, currentPoint.Y),
            Layer = 0.05f,
          });
        }
        else
        {
          _sprites.Add(new Sprite(texture)
          {
            Colour = colour,
            Position = Map.PointToVector2(currentPoint.X, currentPoint.Y),
            Layer = 0.05f,
          });
        }
      }
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      //foreach (var sprite in _gridSprites)
      //  sprite.Draw(gameTime, spriteBatch);

      foreach (var sprite in _potentialPathSprites)
        sprite.Draw(gameTime, spriteBatch);

      // The tiles we see when the unit tries to move
      foreach (var sprite in _sprites)
        sprite.Draw(gameTime, spriteBatch);
    }
  }
}

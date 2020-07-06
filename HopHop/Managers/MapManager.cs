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

    private List<Sprite> _pathOutlineSprites;

    private List<Sprite> _endPointSprites = new List<Sprite>();

    private readonly List<Sprite> _gridSprites;

    private readonly Texture2D _tileTexture;

    private readonly Texture2D _tileBorderTexture;

    public readonly Map Map;

    public Action Refresh;

    private Dictionary<string, Texture2D> _paths = new Dictionary<string, Texture2D>();
    private Dictionary<string, Texture2D> _potentialPaths = new Dictionary<string, Texture2D>();

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

      _potentialPaths.Add("1_Left", content.Load<Texture2D>("Tiles/PotentialPaths/1_Left"));
      _potentialPaths.Add("1_Top", content.Load<Texture2D>("Tiles/PotentialPaths/1_Top"));
      _potentialPaths.Add("1_Right", content.Load<Texture2D>("Tiles/PotentialPaths/1_Right"));
      _potentialPaths.Add("1_Bottom", content.Load<Texture2D>("Tiles/PotentialPaths/1_Bottom"));

      _potentialPaths.Add("2_Left", content.Load<Texture2D>("Tiles/PotentialPaths/2_Left"));
      _potentialPaths.Add("2_Top", content.Load<Texture2D>("Tiles/PotentialPaths/2_Top"));
      _potentialPaths.Add("2_Right", content.Load<Texture2D>("Tiles/PotentialPaths/2_Right"));
      _potentialPaths.Add("2_Bottom", content.Load<Texture2D>("Tiles/PotentialPaths/2_Bottom"));

      _potentialPaths.Add("2_LeftRight", content.Load<Texture2D>("Tiles/PotentialPaths/2_LeftRight"));
      _potentialPaths.Add("2_TopBottom", content.Load<Texture2D>("Tiles/PotentialPaths/2_TopBottom"));

      _potentialPaths.Add("3_Left", content.Load<Texture2D>("Tiles/PotentialPaths/3_Left"));
      _potentialPaths.Add("3_Top", content.Load<Texture2D>("Tiles/PotentialPaths/3_Top"));
      _potentialPaths.Add("3_Right", content.Load<Texture2D>("Tiles/PotentialPaths/3_Right"));
      _potentialPaths.Add("3_Bottom", content.Load<Texture2D>("Tiles/PotentialPaths/3_Bottom"));

      _potentialPaths.Add("4_All", content.Load<Texture2D>("Tiles/PotentialPaths/4_All"));

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
        _pathOutlineSprites = new List<Sprite>();

        foreach (var value in _potentialPoints)
        {
          var point = value.Item2;

          var left = point + new Point(-1, 0);
          var top = point + new Point(0, -1);
          var right = point + new Point(1, 0);
          var bottom = point + new Point(0, 1);

          var speeed = _validUnit.UnitModel.Speed;

          bool condition(int speed1, int speed2)
          {
            return ((speed1 > speeed && speed2 > speeed) || (speed1 <= speeed && speed2 <= speeed));
          }

          bool hasLeft = _potentialPoints.Any(c => c.Item2 == left && condition(c.Item1, value.Item1));
          bool hasTop = _potentialPoints.Any(c => c.Item2 == top && condition(c.Item1, value.Item1));
          bool hasRight = _potentialPoints.Any(c => c.Item2 == right && condition(c.Item1, value.Item1));
          bool hasBottom = _potentialPoints.Any(c => c.Item2 == bottom && condition(c.Item1, value.Item1));

          int i = 0;
          var name = "";

          if (hasLeft && hasRight && hasTop && hasBottom)
            continue;


          if (hasLeft && hasTop && hasRight)
          {
            i = 1;
            name = "Bottom";
          }
          else if (hasTop && hasRight && hasBottom)
          {
            i = 1;
            name = "Left";
          }
          else if (hasRight && hasBottom && hasLeft)
          {
            i = 1;
            name = "Top";
          }
          else if (hasBottom && hasLeft && hasTop)
          {
            i = 1;
            name = "Right";
          }
          else if (hasLeft && hasTop)
          {
            i = 2;
            name = "Right";
          }
          else if (hasTop && hasRight)
          {
            i = 2;
            name = "Bottom";
          }
          else if (hasRight && hasBottom)
          {
            i = 2;
            name = "Left";
          }
          else if (hasBottom && hasLeft)
          {
            i = 2;
            name = "Top";
          }
          else if (hasLeft && hasRight)
          {
            i = 2;
            name = "LeftRight";
          }
          else if (hasTop && hasBottom)
          {
            i = 2;
            name = "TopBottom";
          }
          else if (hasLeft)
          {
            i = 3;
            name = "Top";
          }
          else if (hasTop)
          {
            i = 3;
            name = "Right";
          }
          else if (hasRight)
          {
            i = 3;
            name = "Bottom";
          }
          else if (hasBottom)
          {
            i = 3;
            name = "Left";
          }
          else
          {
            i = 4;
            name = "All";
          }


          var finalName = $"{i}_{name}";

          if (string.IsNullOrEmpty(name))
            continue;

          _pathOutlineSprites.Add(new Sprite(_potentialPaths[finalName])
          {
            Position = Map.PointToVector2(point.X, point.Y),
            Colour = value.Item1 > _validUnit.UnitModel.Speed ? Color.Orange : Color.Green,
            Layer = 0.049f,
          });
        }
      }

      if (_validUnit.PotentialPaths.Count == 0)
        _endPointSprites = new List<Sprite>();

      if (_validUnit.MovementPath.Count == 0)
      {
        _sprites = new List<Sprite>();
        return;
      }

      _previousPosition = _currentPosition;
      _currentPosition = _validUnit.MovementPath.Last();

      if (_previousPosition == _currentPosition && !_validUnit.HasPotentialPathsChanged)
        return;

      _sprites = new List<Sprite>();

      var speed = _validUnit.UnitModel.Speed;
      var stamina = _validUnit.UnitModel.Stamina;

      if (_validUnit.HasPotentialPathsChanged)
      {
        _endPointSprites = new List<Sprite>();

        var points = _validUnit.PotentialPaths.Where(c => c.Count() > 0).Select(c => c.Last());
        foreach (var point in points)
        {
          _endPointSprites.Add(new Sprite(_paths["Tile"])
          {
            Colour = Color.Red,
            Layer = 0.048f,
            Position = Map.PointToVector2(point.X, point.Y),
          });
        }
      }

      for (int i = 0; i < _validUnit.MovementPath.Count; i++)
      {
        var p = _validUnit.MovementPath[i];

        var path = _validUnit.MovementPath[i];

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
          previousPoint = _validUnit.MovementPath[i - 1];

        if (i < _validUnit.MovementPath.Count - 1)
          nextPoint = _validUnit.MovementPath[i + 1];

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


        if (i == _validUnit.MovementPath.Count - 1)
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

      foreach (var sprite in _endPointSprites)
        sprite.Draw(gameTime, spriteBatch);

      foreach (var sprite in _pathOutlineSprites)
        sprite.Draw(gameTime, spriteBatch);

      // The tiles we see when the unit tries to move
      foreach (var sprite in _sprites)
        sprite.Draw(gameTime, spriteBatch);
    }
  }
}

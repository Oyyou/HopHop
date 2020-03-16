using Engine;
using HopHop.MapStuff;
using HopHop.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace HopHop.Managers
{
  public class MapManager
  {
    private Unit _validUnit;

    private List<Sprite> _sprites;

    private readonly List<Sprite> _gridSprites;

    private readonly Texture2D _tileTexture;

    private readonly Texture2D _tileBorderTexture;

    public readonly Map Map;

    public Action Refresh;

    public MapManager(ContentManager content)
    {
      Map = new Map(20, 20);

      _tileTexture = content.Load<Texture2D>("Tiles/Tile");
      _tileBorderTexture = content.Load<Texture2D>("Tiles/TileBorder");

      _gridSprites = new List<Sprite>();
      _sprites = new List<Sprite>();

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

      _sprites = new List<Sprite>();
    }

    public void UnloadContent()
    {
      _sprites.Clear();
      _gridSprites.Clear();
      _tileTexture.Dispose();
      _tileBorderTexture.Dispose();
    }

    public void Update(GameTime gameTime)
    {
      _sprites = new List<Sprite>();

      if (_validUnit == null)
        return;

      if (_validUnit.MovementPositions.Count == 0)
        return;

      var speed = _validUnit.UnitModel.Speed;

      foreach (var p in _validUnit.MovementPositions)
      {
        for (int i = 0; i < speed * 2; i++)
        {
          if (i >= (_validUnit.MovementPositions.Count))
            continue;

          var path = _validUnit.MovementPositions[i];

          var colour = Color.Green;

          if (i >= (speed - _validUnit.TilesMoved))
            colour = Color.Orange;

          var unitPosition = Map.Vector2ToPoint(_validUnit.TilePosition);

          _sprites.Add(new Sprite(_tileTexture)
          {
            Colour = colour,
            Position = Map.PointToVector2(path.X, path.Y),
            Layer = 0.05f,
          });
        }
      }
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      foreach (var sprite in _gridSprites)
        sprite.Draw(gameTime, spriteBatch);

      foreach (var sprite in _sprites)
        sprite.Draw(gameTime, spriteBatch);
    }
  }
}

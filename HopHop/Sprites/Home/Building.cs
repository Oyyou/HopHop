using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using HopHop.MapStuff;

namespace HopHop.Sprites.Home
{
  public class Building : Sprite
  {
    public Rectangle EntranceRectangle
    {
      get
      {
        return new Rectangle(TileRectangle.Left + (Map.TileHeight * 2), TileRectangle.Bottom - Map.TileHeight, Map.TileHeight, Map.TileHeight);
      }
    }

    public Rectangle ExitRectangle { get; set; }

    public Building(Texture2D texture)
      : base(texture)
    {

    }

    public override void Update(GameTime gameTime)
    {
      
    }
  }
}

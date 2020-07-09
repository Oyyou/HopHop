using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace HopHop.Sprites.Home
{
  public class Building : Sprite
  {
    public Rectangle EntranceRectangle
    {
      get
      {
        return new Rectangle(TileRectangle.Left + 80, TileRectangle.Bottom - 80, 40, 80);
      }
    }

    public Building(Texture2D texture)
      : base(texture)
    {

    }

    public override void Update(GameTime gameTime)
    {
      
    }
  }
}

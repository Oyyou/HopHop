using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopHop.MapStuff
{
  public interface IMapable
  {
    Rectangle TileRectangle { get; }

    Vector2 TilePosition { get; set; }
  }
}

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
  public static class Helpers
  {
    public static int GetWidth(this char[,] value)
    {
      return value.GetLength(1);
    }

    public static int GetHeight(this char[,] value)
    {
      return value.GetLength(0);
    }

    public static Point Clamp(this Point value1, Point min, Point max)
    {
      return new Point(
          MathHelper.Clamp(value1.X, min.X, max.X),
          MathHelper.Clamp(value1.Y, min.Y, max.Y));
    }
  }
}

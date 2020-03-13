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
  }
}

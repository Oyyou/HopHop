using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopHop.Models
{
  public class UnitModel
  {
    /// <summary>
    /// Tiles that can be covered in 1-turn
    /// </summary>
    public int Speed { get; set; }

    public int Health { get; set; }

    public int Armour { get; set; }
  }
}

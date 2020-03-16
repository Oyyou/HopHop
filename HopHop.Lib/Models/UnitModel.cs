using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopHop.Lib.Models
{
  public class UnitModel
  {
    private static int _ids;

    public readonly int Id;

    public string Name { get; set; }

    /// <summary>
    /// Tiles that can be covered in 1-turn
    /// </summary>
    public int Speed { get; set; }

    public int Health { get; set; }

    public int Armour { get; set; }

    public AbilitiesModel Abilities { get; set; }

    public UnitModel()
    {
      Id = _ids++;
    }
  }
}

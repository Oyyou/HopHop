using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopHop.Lib.Models
{
  public class UnitModel
  {
    public enum UnitTypes
    {
      Friendly,
      Enemy,
    }

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

    public UnitTypes UnitType { get; set; }

    public UnitModel()
    {
      Id = _ids++;
    }

    public override bool Equals(object obj)
    {
      return this.Id == ((UnitModel)obj).Id;
    }

    public override int GetHashCode()
    {
      var hashCode = 806287619;
      hashCode = hashCode * -1521134295 + Id.GetHashCode();
      hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
      hashCode = hashCode * -1521134295 + Speed.GetHashCode();
      hashCode = hashCode * -1521134295 + Health.GetHashCode();
      hashCode = hashCode * -1521134295 + Armour.GetHashCode();
      hashCode = hashCode * -1521134295 + EqualityComparer<AbilitiesModel>.Default.GetHashCode(Abilities);
      return hashCode;
    }
  }
}

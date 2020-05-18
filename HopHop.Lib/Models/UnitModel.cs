using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace HopHop.Lib.Models
{
  public class UnitModel
  {
    public enum UnitTypes
    {
      Friendly,
      Enemy,
    }

    public int Id;

    public string Name { get; set; }

    /// <summary>
    /// Tiles that can be covered with 1 stamina
    /// </summary>
    public int Speed { get; set; }

    public int Stamina { get; set; }

    public int Health { get; set; }

    public int Armour { get; set; }

    public List<int> AbilityIds { get; set; }

    [JsonIgnore]
    public AbilitiesModel Abilities { get; set; }

    public UnitTypes UnitType { get; set; }

    public UnitModel()
    {
      //Id = _ids++;
    }

    public override string ToString()
    {
      return $"{Id}: {Name}";
    }

    public override bool Equals(object obj)
    {
      if (!(obj is UnitModel))
        return false;

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

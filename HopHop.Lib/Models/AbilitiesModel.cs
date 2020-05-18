using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopHop.Lib.Models
{
  public class AbilitiesModel
  {
    public readonly AbilityModel Ability1;
    public readonly AbilityModel Ability2;
    public readonly AbilityModel Ability3;
    public readonly AbilityModel Ability4;

    public AbilitiesModel(List<AbilityModel> abilties)
    {
      Ability1 = abilties.Count > 0 ? abilties[0] : null;
      Ability2 = abilties.Count > 1 ? abilties[1] : null;
      Ability3 = abilties.Count > 2 ? abilties[2] : null;
      Ability4 = abilties.Count > 3 ? abilties[3] : null;
    }

    /// <summary>
    /// Get the abilities for this actor
    /// </summary>
    /// <returns>A list of all the abilities</returns>
    public IEnumerable<AbilityModel> Get()
    {
      return new List<AbilityModel>()
      {
        Ability1,
        Ability2,
        Ability3,
        Ability4,
      };
    }

    public AbilityModel Get(int index)
    {
      if (index < 0)
        return null;

      return this.Get().ToList()[index];
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopHop.Lib.Models
{
  public class AbilitiesModel
  {
    public AbilityModel Ability1 { get; set; }

    public AbilityModel Ability2 { get; set; }

    public AbilityModel Ability3 { get; set; }

    public AbilityModel Ability4 { get; set; }

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
      return this.Get().ToList()[index];
    }
  }
}

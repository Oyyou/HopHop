using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace HopHop.Lib.Models
{
  public class AbilityModel
  {
    public enum TargetTypes
    {
      Enemies,
      Friendlies,
      Self,
      All,
    }

    public enum AbilityTypes
    {
      Close,
      Ranged,
      Self,
    }

    public int Id;

    public string Name;

    public string Text;

    public int StaminaCost;
    
    /// <summary>
    /// How much the health the target loes on successful cast
    /// </summary>
    public int DamageAmount { get; set; }

    /// <summary>
    /// How much the health the target receives on successful cast
    /// </summary>
    public int HealAmount { get; set; }

    /// <summary>
    /// Who can be targeted
    /// </summary>
    public TargetTypes TargetType;

    /// <summary>
    /// Distance of ability
    /// </summary>
    public AbilityTypes AbilityType;

    /// <summary>
    /// What happens to the target on successful cast
    /// </summary>
    public List<int> TargetStatusEffects { get; set; }

    /// <summary>
    /// What happens to the user on successful cast
    /// </summary>
    public List<int> SelfStatusEffects { get; set; }

    [JsonIgnore]
    public bool IsEnabled { get; set; }

    [JsonIgnore]
    public List<UnitModel> Targets { get; set; }

    public override string ToString()
    {
      return Name;
    }
  }
}

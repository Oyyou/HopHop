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

    public readonly string IconName;

    public readonly string Text;

    public readonly int StaminaCost;

    public readonly TargetTypes TargetType;

    public readonly AbilityTypes AbilityType;

    [XmlIgnore]
    public bool IsEnabled { get; set; }

    [XmlIgnore]
    public List<UnitModel> Targets { get; set; }

    public AbilityModel(string text, string iconName, int staminaCost, TargetTypes targetType, AbilityTypes abilityType)
    {
      Text = text;

      StaminaCost = staminaCost;

      IconName = iconName;

      TargetType = targetType;

      AbilityType = abilityType;
    }
  }
}

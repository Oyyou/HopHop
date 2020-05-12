using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopHop.Lib.Models
{
  public class AbilityModel
  {
    public enum TargetTypes
    {
      Enemies,
      Friendlies,
      Self,
    }

    public readonly string IconName;

    public readonly string Text;

    public readonly TargetTypes TargetType;

    public AbilityModel(string text, string iconName, TargetTypes targetType)
    {
      Text = text;

      IconName = iconName;

      TargetType = targetType;
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopHop.Lib.Models
{
  public class AbilityModel
  {
    public readonly string IconName;

    public readonly string Text;

    public AbilityModel(string text, string iconName)
    {
      Text = text;

      IconName = iconName;
    }
  }
}

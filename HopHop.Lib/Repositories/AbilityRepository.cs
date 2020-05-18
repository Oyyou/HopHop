using HopHop.Lib.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopHop.Lib.Repositories
{
  public class AbilityRepository
  {
    public List<AbilityModel> Abilities { get; private set; } = new List<AbilityModel>();

    public static string Path = "Abilities";

    public AbilityModel GetById(int id)
    {
      return Abilities.FirstOrDefault(c => c.Id == id);
    }

    public void Save(AbilityModel ability)
    {
      var serializer = new JsonSerializer();
      serializer.NullValueHandling = NullValueHandling.Ignore;
      serializer.Formatting = Formatting.Indented;

      if (!Directory.Exists(Path))
        Directory.CreateDirectory(Path);

      using (var writer = new StreamWriter($@"{Path}\\{ability.Id}_{ability.Name}.json"))
      {
        using (var json = new JsonTextWriter(writer))
        {
          serializer.Serialize(json, ability);
        }
      }
    }

    public void Load()
    {
      if (!Directory.Exists(Path))
      {
        Directory.CreateDirectory(Path);
        return;
      }

      var files = Directory.GetFiles(Path, "*.json");

      foreach (var file in files)
      {
        var values = File.ReadAllLines(file);
        var content = string.Join("", values);
        var ability = JsonConvert.DeserializeObject<AbilityModel>(content);

        Abilities.Add(ability);
      }

      var groupedById = Abilities.GroupBy(c => c.Id);

      var errors = groupedById.Where(c => c.Count() > 1);
      if (errors.Count() > 0)
        throw new Exception("Duped ability Id(s): " + string.Join(", ", errors.Select(c => c.Key)));
    }
  }
}

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
  public class UnitRepository
  {
    public List<UnitModel> Units { get; private set; } = new List<UnitModel>();

    public static string Path = "Units";

    public UnitModel GetById(int id)
    {
      return Units.FirstOrDefault(c => c.Id == id);
    }

    public void Save(UnitModel unit)
    {
      var serializer = new JsonSerializer();
      serializer.NullValueHandling = NullValueHandling.Ignore;
      serializer.Formatting = Formatting.Indented;

      if (!Directory.Exists(Path))
        Directory.CreateDirectory(Path);

      using (var writer = new StreamWriter($@"{Path}\\{unit.Id}_{unit.Name}.json"))
      {
        using (var json = new JsonTextWriter(writer))
        {
          serializer.Serialize(json, unit);
        }
      }
    }

    public void Load(AbilityRepository abilityRepository)
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
        var unit = JsonConvert.DeserializeObject<UnitModel>(content);

        var abilities = unit.AbilityIds.Select(c => abilityRepository.GetById(c)).ToList();

        unit.Abilities = new AbilitiesModel(abilities);

        Units.Add(unit);
      }

      var groupedById = Units.GroupBy(c => c.Id);

      var errors = groupedById.Where(c => c.Count() > 1);
      if (errors.Count() > 0)
        throw new Exception("Duped unit Id(s): " + string.Join(", ", errors.Select(c => c.Key)));
    }
  }
}

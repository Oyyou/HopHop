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
  public class SquadRepository
  {
    public List<SquadModel> Squads { get; private set; } = new List<SquadModel>();

    public static string Path = "Squads";

    public SquadModel GetById(int id)
    {
      return Squads.FirstOrDefault(c => c.Id == id);
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
        var squad = JsonConvert.DeserializeObject<SquadModel>(content);

        var ids = squad.UnitIds.Distinct().ToList();

        squad.UnitIds = ids.GetRange(0, Math.Min(4, ids.Count));

        Squads.Add(squad);
      }

      var groupedById = Squads.GroupBy(c => c.Id);

      var errors = groupedById.Where(c => c.Count() > 1);
      if (errors.Count() > 0)
        throw new Exception("Duped squad Id(s): " + string.Join(", ", errors.Select(c => c.Key)));
    }
  }
}

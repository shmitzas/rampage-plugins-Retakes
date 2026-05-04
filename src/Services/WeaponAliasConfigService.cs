using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SwiftlyS2.Shared;
using SwiftlyS2_Retakes.Interfaces;

namespace SwiftlyS2_Retakes.Services;

/// <summary>
/// Loads user-defined weapon aliases from resources/guns.jsonc (created automatically on first run).
/// The file format is a JSON object mapping weapon entity name → list of aliases, e.g.:
///   { "weapon_ak47": ["ak", "ak47"], "weapon_m4a1_silencer": ["m4a1s", "m4s"] }
/// </summary>
public sealed class WeaponAliasConfigService : IWeaponAliasConfigService
{
  private readonly ISwiftlyCore _core;
  private readonly ILogger _logger;

  private Dictionary<string, string> _aliases = new(StringComparer.OrdinalIgnoreCase);

  private static readonly JsonSerializerOptions JsonReadOptions = new()
  {
    ReadCommentHandling = JsonCommentHandling.Skip,
    AllowTrailingCommas = true,
    PropertyNameCaseInsensitive = true,
  };

  public WeaponAliasConfigService(ISwiftlyCore core, ILogger logger)
  {
    _core = core;
    _logger = logger;
    LoadOrCreate();
  }

  public bool TryResolve(string input, out string weaponName)
  {
    if (_aliases.TryGetValue(input, out var found))
    {
      weaponName = found;
      return true;
    }

    weaponName = string.Empty;
    return false;
  }

  public void LoadOrCreate()
  {
    var path = Path.Combine(_core.PluginPath, "resources", "guns.jsonc");

    if (!File.Exists(path))
    {
      CreateDefault(path);
    }

    try
    {
      var text = File.ReadAllText(path);
      var parsed = JsonSerializer.Deserialize<Dictionary<string, string[]>>(text, JsonReadOptions);
      if (parsed is not null)
      {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var (weapon, aliases) in parsed)
        {
          foreach (var alias in aliases)
          {
            if (!string.IsNullOrWhiteSpace(alias))
              map[alias.Trim()] = weapon;
          }
        }
        _aliases = map;
        _logger.LogInformation("Retakes: loaded {Count} weapon alias(es) from guns.jsonc", _aliases.Count);
      }
    }
    catch (Exception ex)
    {
      _logger.LogWarning(ex, "Retakes: failed to load guns.jsonc, using empty alias map");
      _aliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }
  }

  private void CreateDefault(string path)
  {
    try
    {
      var dir = Path.GetDirectoryName(path);
      if (dir is not null)
        Directory.CreateDirectory(dir);

      const string defaultContent = """
// guns.jsonc — Custom weapon aliases for the !gun command.
//
// Format: "weapon_entity_name": ["alias1", "alias2", ...]
//
// Examples:
//   "weapon_ak47":         ["ak", "ak47"]
//   "weapon_m4a1_silencer": ["m4a1s", "m4s"]
//   "weapon_awp":          ["awp", "sniper"]
//
// Aliases are case-insensitive. Run !reloadcfg after editing this file.

{
  // "weapon_ak47":          ["ak", "ak47"],
  // "weapon_m4a1_silencer": ["m4a1s", "m4s"],
  // "weapon_awp":           ["awp", "sniper"]
}
""";

      File.WriteAllText(path, defaultContent);
      _logger.LogInformation("Retakes: created default guns.jsonc at {Path}", path);
    }
    catch (Exception ex)
    {
      _logger.LogWarning(ex, "Retakes: failed to create default guns.jsonc");
    }
  }
}

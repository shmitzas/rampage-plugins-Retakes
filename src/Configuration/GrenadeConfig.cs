namespace SwiftlyS2_Retakes.Configuration;

/// <summary>
/// Configuration for grenade allocation.
/// </summary>
public sealed class GrenadeConfig
{
  /// <summary>
  /// Allocation type applied to every round type:
  /// <list type="bullet">
  ///   <item><term>random</term> – each grenade in <see cref="TeamGrenadeConfig.RandomChances"/> has an independent per-player
  ///     probability (0–100) of being given. Grenades are rolled individually.</item>
  ///   <item><term>fixed</term> – every player always receives the grenades listed in <see cref="TeamGrenadeConfig.Fixed"/>.</item>
  ///   <item><term>dynamic</term> – grenades in <see cref="TeamGrenadeConfig.DynamicPool"/> are distributed round-robin across
  ///     the team starting from the player who dealt the most damage last round. Players who dealt less than
  ///     <see cref="DynamicMinDamage"/> receive nothing.</item>
  /// </list>
  /// </summary>
  public string AllocationType { get; set; } = "random";

  /// <summary>
  /// Dynamic mode: minimum damage a player must have dealt last round to be eligible for grenades.
  /// </summary>
  public int DynamicMinDamage { get; set; } = 1;

  /// <summary>
  /// Dynamic mode: maximum grenades any single player can receive per round. 0 = no limit.
  /// </summary>
  public int DynamicMaxPerPlayer { get; set; } = 0;

  /// <summary>
  /// Dynamic mode: fraction (0.0–1.0) of the eligible players per team who can receive grenades.
  /// Players are sorted by last-round damage (or cumulative score if <see cref="DynamicUseCumulativeScore"/> is true)
  /// and only the top fraction are considered, even if more players meet <see cref="DynamicMinDamage"/>.
  /// The result is rounded up, so at least 1 player is always included when there are any eligible players.
  /// 1.0 = all eligible players can receive grenades (no cap).
  /// </summary>
  public float DynamicTopFraction { get; set; } = 0.5f;

  /// <summary>
  /// Dynamic mode: when true, priority uses the cumulative performance score (exponentially weighted
  /// towards recent rounds) instead of last round's raw damage. Useful for rewarding consistent players
  /// over single-round outliers. The <see cref="DynamicMinDamage"/> threshold still uses last-round damage.
  /// </summary>
  public bool DynamicUseCumulativeScore { get; set; } = false;

  /// <summary>
  /// Maximum number of each grenade type a single player can receive, applied across all allocation modes.
  /// Keys are grenade class names (e.g. <c>weapon_flashbang</c>), values are the per-player cap.
  /// Grenades not listed here are uncapped (subject to <see cref="DynamicMaxPerPlayer"/> in dynamic mode).
  /// Example: <c>{ "weapon_flashbang": 2, "weapon_smokegrenade": 1 }</c>
  /// </summary>
  public Dictionary<string, int> MaxPerGrenade { get; set; } = new(StringComparer.OrdinalIgnoreCase)
  {
    ["weapon_flashbang"] = 2,
    ["weapon_smokegrenade"] = 1,
    ["weapon_hegrenade"] = 1,
    ["weapon_molotov"] = 1,
    ["weapon_incgrenade"] = 1,
    ["weapon_decoy"] = 1,
  };

  public RoundTypeGrenadeConfig Pistol { get; set; } = new()
  {
    T = new()
    {
      RandomChances = new() { ["weapon_flashbang"] = 50, ["weapon_smokegrenade"] = 50 },
      Fixed = new() { "weapon_smokegrenade" },
      DynamicPool = new() { "weapon_smokegrenade", "weapon_flashbang", "weapon_smokegrenade", "weapon_flashbang" },
    },
    Ct = new()
    {
      RandomChances = new() { ["weapon_flashbang"] = 50, ["weapon_smokegrenade"] = 50 },
      Fixed = new() { "weapon_smokegrenade" },
      DynamicPool = new() { "weapon_smokegrenade", "weapon_flashbang", "weapon_smokegrenade", "weapon_flashbang" },
    },
  };

  public RoundTypeGrenadeConfig HalfBuy { get; set; } = new()
  {
    T = new()
    {
      RandomChances = new() { ["weapon_smokegrenade"] = 100, ["weapon_flashbang"] = 60, ["weapon_hegrenade"] = 40, ["weapon_molotov"] = 35 },
      Fixed = new() { "weapon_smokegrenade", "weapon_flashbang" },
      DynamicPool = new() { "weapon_smokegrenade", "weapon_smokegrenade", "weapon_flashbang", "weapon_flashbang", "weapon_hegrenade", "weapon_molotov" },
    },
    Ct = new()
    {
      RandomChances = new() { ["weapon_smokegrenade"] = 100, ["weapon_flashbang"] = 60, ["weapon_hegrenade"] = 40, ["weapon_incgrenade"] = 35 },
      Fixed = new() { "weapon_smokegrenade", "weapon_flashbang" },
      DynamicPool = new() { "weapon_smokegrenade", "weapon_smokegrenade", "weapon_flashbang", "weapon_flashbang", "weapon_hegrenade", "weapon_incgrenade" },
    },
  };

  public RoundTypeGrenadeConfig FullBuy { get; set; } = new()
  {
    T = new()
    {
      RandomChances = new() { ["weapon_smokegrenade"] = 100, ["weapon_flashbang"] = 70, ["weapon_hegrenade"] = 50, ["weapon_molotov"] = 45 },
      Fixed = new() { "weapon_smokegrenade", "weapon_flashbang", "weapon_molotov" },
      DynamicPool = new() { "weapon_smokegrenade", "weapon_smokegrenade", "weapon_smokegrenade", "weapon_flashbang", "weapon_flashbang", "weapon_flashbang", "weapon_hegrenade", "weapon_hegrenade", "weapon_molotov", "weapon_molotov" },
    },
    Ct = new()
    {
      RandomChances = new() { ["weapon_smokegrenade"] = 100, ["weapon_flashbang"] = 70, ["weapon_hegrenade"] = 50, ["weapon_incgrenade"] = 45 },
      Fixed = new() { "weapon_smokegrenade", "weapon_flashbang", "weapon_incgrenade" },
      DynamicPool = new() { "weapon_smokegrenade", "weapon_smokegrenade", "weapon_smokegrenade", "weapon_flashbang", "weapon_flashbang", "weapon_flashbang", "weapon_hegrenade", "weapon_hegrenade", "weapon_incgrenade", "weapon_incgrenade" },
    },
  };
}

/// <summary>
/// Grenade configuration for a single round type, split by team.
/// </summary>
public sealed class RoundTypeGrenadeConfig
{
  public TeamGrenadeConfig T { get; set; } = new();
  public TeamGrenadeConfig Ct { get; set; } = new();
}

/// <summary>
/// Per-team grenade settings used by all three allocation modes.
/// </summary>
public sealed class TeamGrenadeConfig
{
  /// <summary>
  /// Random mode: grenade class name → independent chance (0–100) each player has of receiving it.
  /// Each entry is rolled separately, so a player can receive multiple grenades.
  /// </summary>
  public Dictionary<string, int> RandomChances { get; set; } = new();

  /// <summary>
  /// Fixed mode: every player on this team always receives exactly these grenades.
  /// </summary>
  public List<string> Fixed { get; set; } = new();

  /// <summary>
  /// Dynamic mode: ordered pool of grenades distributed across the team by damage priority.
  /// Duplicates represent multiple grenades of the same type in the pool.
  /// </summary>
  public List<string> DynamicPool { get; set; } = new();
}

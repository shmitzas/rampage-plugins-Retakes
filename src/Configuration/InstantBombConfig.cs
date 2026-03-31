namespace SwiftlyS2_Retakes.Configuration;

/// <summary>
/// Who receives a defuse result message.
/// </summary>
public enum DefuseMessageTarget
{
  /// <summary>Send to every player on the server.</summary>
  All,
  /// <summary>Send only to the CT team (the defusing team).</summary>
  Team,
  /// <summary>Send only to the player who defused.</summary>
  Player,
}

/// <summary>
/// Configuration for instant plant/defuse mechanics.
/// </summary>
public sealed class InstantBombConfig
{
  public bool InstaPlant { get; set; } = true;
  public bool InstaDefuse { get; set; } = true;
  public bool BlockDefuseIfTAlive { get; set; } = true;
  public bool BlockDefuseIfMollyNear { get; set; } = true;
  public float MollyRadius { get; set; } = 120f;
  public DefuseMessageTarget SuccessfulMessageTarget { get; set; } = DefuseMessageTarget.All;
  public DefuseMessageTarget UnsuccessfulMessageTarget { get; set; } = DefuseMessageTarget.All;
}

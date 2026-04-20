using SwiftlyS2.Shared.Players;

namespace SwiftlyS2_Retakes.Interfaces;

public interface IDamageReportService
{
  void OnRoundStart(bool isWarmup);

  void OnRoundEnd();

  void OnPlayerHurt(IPlayer attacker, IPlayer victim, int dmgHealth);

  float GetPlayerScore(ulong steamId);

  float GetPlayerScore(IPlayer player);

  void OnPlayerKill(IPlayer attacker);

  int GetRoundKills(ulong steamId);

  int GetRoundDamage(ulong steamId);

  int GetLastRoundDamage(ulong steamId);

  void SetLastDefuser(ulong steamId);

  ulong GetLastDefuser();

  void PrintRoundReport();
}

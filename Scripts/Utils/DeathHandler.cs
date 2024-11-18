using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnboundLib.Networking;
using UnityEngine;

namespace JARL.Utils {
    public struct DamageInfo {
        public float DamageAmount;
        public float TimeSinceLastDamage;

        public DamageInfo(float damageAmount, float timeSinceLastDamage) {
            DamageAmount = damageAmount;
            TimeSinceLastDamage = timeSinceLastDamage;
        }
    }

    /// <summary>
    /// The `DamageInfo` in the dictionary represents the damage dealt and time since the player was last damaged.
    /// </summary>
    public delegate void DeathHandlerDelegate(Player player, Dictionary<Player, DamageInfo> playerDamageInfo);
    /// <summary>
    /// The `DamageInfo` in the dictionary represents the damage dealt and time since the player was last damaged,
    /// This delegate only works on the host, Other client will never get this event.
    /// </summary>
    public delegate void DeathHandlerDelegateHost(Player player, Dictionary<Player, DamageInfo> playerDamageInfo);

    public static class DeathHandler {
        public static event DeathHandlerDelegate OnPlayerDeath;
        public static event DeathHandlerDelegateHost OnPlayerDeathHost;

        private static readonly Dictionary<Player, Dictionary<Player, DamageInfo>> damageTrackings = new Dictionary<Player, Dictionary<Player, DamageInfo>>();

        internal static void PlayerDamaged(Player player, Player damagingPlayer, float damageAmount) {
            if(damagingPlayer == null || damageAmount <= 0) return;

            if(!damageTrackings.ContainsKey(player)) {
                damageTrackings.Add(player, new Dictionary<Player, DamageInfo>());
            }

            if(!damageTrackings[player].ContainsKey(damagingPlayer)) {
                damageTrackings[player].Add(damagingPlayer, new DamageInfo(damageAmount, Time.time));
            } else {
                damageTrackings[player][damagingPlayer] = new DamageInfo(damageTrackings[player][damagingPlayer].DamageAmount + damageAmount, Time.time);
            }
        }

        internal static void PlayerDeath(Player player) {
            if(!damageTrackings.ContainsKey(player)) {
                damageTrackings.Add(player, new Dictionary<Player, DamageInfo>());
            }

            if(PhotonNetwork.IsMasterClient || PhotonNetwork.OfflineMode) {
                OnPlayerDeathHost?.Invoke(player, GetPlayerDamage(player));

                int[] playerIDs = damageTrackings[player].Keys.Select(p => p.playerID).ToArray();
                float[] timeSinceLastDamage = damageTrackings[player].Values.Select(d => Time.time - d.TimeSinceLastDamage).ToArray();
                float[] damageAmounts = damageTrackings[player].Values.Select(d => d.DamageAmount).ToArray();
                NetworkingManager.RPC(typeof(DeathHandler), nameof(RPCA_PlayerDeath), player.playerID, playerIDs, timeSinceLastDamage, damageAmounts);

                damageTrackings.Remove(player);
            }
        }

        [UnboundRPC]
        private static void RPCA_PlayerDeath(int playerID, int[] playerIDs, float[] timeSinceLastDamage, float[] damageAmounts) {
            Dictionary<Player, DamageInfo> playerDamageInfo = new Dictionary<Player, DamageInfo>();
            for(int i = 0; i < playerIDs.Length; i++) {
                playerDamageInfo.Add(PlayerManager.instance.players.Find(p => p.playerID == playerIDs[i]), new DamageInfo(damageAmounts[i], timeSinceLastDamage[i]));
            }

            Player player = PlayerManager.instance.players.Find(p => p.playerID == playerID);
            OnPlayerDeath?.Invoke(player, playerDamageInfo);
        }

        private static Dictionary<Player, DamageInfo> GetPlayerDamage(Player player) {
            if(damageTrackings.ContainsKey(player)) {
                return damageTrackings[player]
                    .Select(pair => new KeyValuePair<Player, DamageInfo>(pair.Key, new DamageInfo(pair.Value.DamageAmount, Time.time - pair.Value.TimeSinceLastDamage)))
                    .ToDictionary(pair => pair.Key, pair => pair.Value);
            }
            return new Dictionary<Player, DamageInfo>();
        }
    }
}

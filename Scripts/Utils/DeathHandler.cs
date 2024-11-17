using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnboundLib.Networking;
using UnityEngine;

namespace JARL.Utils {
    /// <summary>
    /// The `float` in the dictionary is the time since the player was last damaged.
    /// </summary>
    public delegate void DeathHandlerDelegate(Player player, Dictionary<Player, float> playerLastDamageDamage);
    /// <summary>
    /// The `float` in the dictionary is the time since the player was last damaged,
    /// This delegate only works on the host, Other client will never get this event.
    /// </summary>
    public delegate void DeathHandlerDelegateHost(Player player, Dictionary<Player, float> playerLastDamageDamage);

    public static class DeathHandler {
        public static event DeathHandlerDelegate OnPlayerDeath;
        public static event DeathHandlerDelegateHost OnPlayerDeathHost;

        private static readonly Dictionary<Player, Dictionary<Player, float>> damageTrackings = new Dictionary<Player, Dictionary<Player, float>>();

        internal static void PlayerDamaged(Player player, Player damagingPlayer) {
            if(damagingPlayer == null) return;

            if(!damageTrackings.ContainsKey(player)) {
                damageTrackings.Add(player, new Dictionary<Player, float>());
            }
            if(!damageTrackings[player].ContainsKey(damagingPlayer)) {
                damageTrackings[player].Add(damagingPlayer, Time.time);
            }
            damageTrackings[player][damagingPlayer] = Time.time;
        }

        internal static void PlayerDeath(Player player) {
            if(!damageTrackings.ContainsKey(player)) {
                damageTrackings.Add(player, new Dictionary<Player, float>());
            }

            if(PhotonNetwork.IsMasterClient || PhotonNetwork.OfflineMode) {
                OnPlayerDeathHost?.Invoke(player, GetPlayerDamage(player));

                int[] playerIDs = damageTrackings[player].Keys.Select(p => p.playerID).ToArray();
                float[] playerTimes = damageTrackings[player].Values.ToArray();
                NetworkingManager.RPC(typeof(DeathHandler), nameof(RPCA_PlayerDeath), player.playerID, playerIDs, playerTimes);

                damageTrackings.Remove(player);
            }
        }

        [UnboundRPC]
        private static void RPCA_PlayerDeath(int playerID, int[] playerIDs, float[] playerTimes) {
            Dictionary<Player, float> playerLastDamageDamage = new Dictionary<Player, float>();
            for(int i = 0; i < playerIDs.Length; i++) {
                playerLastDamageDamage.Add(PlayerManager.instance.players.Find(player => player.playerID == playerIDs[i]), playerTimes[i]);
            }

            Player player = PlayerManager.instance.players.Find(p => p.playerID == playerID);
            OnPlayerDeath?.Invoke(player, playerLastDamageDamage);
        }

        private static Dictionary<Player, float> GetPlayerDamage(Player player) {
            if(damageTrackings.ContainsKey(player)) {
                return damageTrackings[player]
                    .Select(pair => new KeyValuePair<Player, float>(pair.Key, Time.time - pair.Value))
                    .ToDictionary(pair => pair.Key, pair => pair.Value);
            }
            return new Dictionary<Player, float>();
        }
    }
}

using HarmonyLib;
using JARL.Armor;
using JARL.Extensions;
using System.Collections;
using UnityEngine;

namespace JARL.Patches {
    [HarmonyPatch(typeof(DamageOverTime))]
    public class DamageOverTimePatch {
        [HarmonyPatch("TakeDamageOverTime")]
        [HarmonyPrefix]
        public static bool TakeDamageOverTimePrefix(DamageOverTime __instance, ref Vector2 damage, Player damagingPlayer) {
            CharacterData data = (CharacterData)Traverse.Create(__instance).Field("data").GetValue();
            if(damage == Vector2.zero || !data.isPlaying || data.dead || __instance.GetComponent<HealthHandler>().isRespawning || HealthHandlerPatch.TakeDamageRunning) {
                return true;
            }

            if(data.GetAdditionalData().totalArmor > 0) {
                data.player.GetComponent<ArmorHandler>().ProcessDamage(ref damage, damagingPlayer, data.player, ArmorDamagePatchType.TakeDamageOverTime);

                if(damage == Vector2.zero) {
                    return false;
                }
            }

            return true;
        }

        private static IEnumerator EmptyEnumerator() {
            yield break;
        }
    }
}

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
            if(!data.CanDamage() || HealthHandlerPatch.TakeDamageRunning) {
                return true;
            }

            if(data.GetAdditionalData().totalArmor > 0) {
                ArmorFramework.ArmorHandlers[data.player].ProcessDamage(ref damage, damagingPlayer, data.player, ArmorDamagePatchType.TakeDamageOverTime);

                if(damage == Vector2.zero) {
                    return false;
                }
            }

            return true;
        }
    }
}

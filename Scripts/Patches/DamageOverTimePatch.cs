using HarmonyLib;
using JARL.Armor;
using JARL.Extensions;
using UnityEngine;

namespace JARL.Patches {
    [HarmonyPatch(typeof(DamageOverTime))]
    public class DamageOverTimePatch {
        [HarmonyPatch("TakeDamageOverTime")]
        [HarmonyPrefix]
        private static void TakeDamageOverTimePrefix(DamageOverTime __instance, ref Vector2 damage, Player damagingPlayer) {
            CharacterData data = (CharacterData)Traverse.Create(__instance).Field("data").GetValue();

            if(data.GetAdditionalData().totalArmor > 0) {
                data.player.GetComponent<ArmorHandler>().ProcessDamage(ref damage, damagingPlayer, data.player, ArmorDamagePatchType.TakeDamageOverTime);
            }
        }
    }
}

using HarmonyLib;
using JARL.ArmorFramework;
using JARL.Extensions;
using UnityEngine;

namespace JARL.Patchs
{
    [HarmonyPatch(typeof(HealthHandler))]
    public class HealthHandlerPatch
    {
        [HarmonyPatch("DoDamage")]
        [HarmonyPrefix]
        public static void DoDamage(HealthHandler __instance, ref Vector2 damage, Vector2 position, Color blinkColor, GameObject damagingWeapon, Player damagingPlayer, bool healthRemoval, bool lethal, bool ignoreBlock)
        {
            CharacterData data = (CharacterData)Traverse.Create(__instance).Field("data").GetValue();

            // Deal damage to armor not health
            if (data.GetAdditionalData().totalArmor > 0)
            {
                data.player.GetComponent<ArmorHandler>().ProcessDamage(ref damage, damagingPlayer, data.player);
            }
        }

        [HarmonyPatch("Revive")]
        [HarmonyPostfix]
        public static void Revive(CharacterData ___data)
        {
            if (___data.GetComponent<ArmorHandler>())
            {
                ___data.GetComponent<ArmorHandler>().OnRespawn();
            }
        }
    }
}

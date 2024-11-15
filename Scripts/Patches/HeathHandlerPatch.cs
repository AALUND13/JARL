﻿using HarmonyLib;
using JARL.Armor;
using JARL.Extensions;
using UnityEngine;

namespace JARL.Patches {
    [HarmonyPatch(typeof(HealthHandler))]
    public class HealthHandlerPatch {
        [HarmonyPatch("TakeDamage", typeof(Vector2), typeof(Vector2), typeof(Color), typeof(GameObject), typeof(Player), typeof(bool), typeof(bool))]
        [HarmonyPrefix]
        private static void TakeDamagePrefix(HealthHandler __instance, ref Vector2 damage, Player damagingPlayer) {
            CharacterData data = (CharacterData)Traverse.Create(__instance).Field("data").GetValue();

            if(data.GetAdditionalData().totalArmor > 0) {
                data.player.GetComponent<ArmorHandler>().ProcessDamage(ref damage, damagingPlayer, data.player, ArmorDamagePatchType.TakeDamage);
            }
        }

        [HarmonyPatch("DoDamage")]
        [HarmonyPrefix]
        public static void DoDamage(HealthHandler __instance, ref Vector2 damage, Vector2 position, Color blinkColor, GameObject damagingWeapon, Player damagingPlayer, bool healthRemoval, bool lethal, bool ignoreBlock) {
            CharacterData data = (CharacterData)Traverse.Create(__instance).Field("data").GetValue();

            if(data.GetAdditionalData().totalArmor > 0) {
                data.player.GetComponent<ArmorHandler>().ProcessDamage(ref damage, damagingPlayer, data.player, ArmorDamagePatchType.DoDamage);
            }
        }

        [HarmonyPatch("Revive")]
        [HarmonyPostfix]
        public static void Revive(CharacterData ___data) {
            if(___data.GetComponent<ArmorHandler>()) {
                ___data.GetComponent<ArmorHandler>().OnRespawn();
            }
        }
    }
}

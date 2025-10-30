using HarmonyLib;
using JARL.Armor;
using JARL.Extensions;
using JARL.Utils;
using UnboundLib;
using UnityEngine;

namespace JARL.Patches {
    [HarmonyPatch(typeof(HealthHandler))]
    public class HealthHandlerPatch {
        public static bool TakeDamageRunning = false;

        [HarmonyPatch("TakeDamage", typeof(Vector2), typeof(Vector2), typeof(Color), typeof(GameObject), typeof(Player), typeof(bool), typeof(bool))]
        [HarmonyPrefix]
        public static void TakeDamagePrefix(HealthHandler __instance, ref Vector2 damage, Player damagingPlayer, bool ignoreBlock) {
            TakeDamageRunning = true;
            CharacterData data = (CharacterData)Traverse.Create(__instance).Field("data").GetValue();
            if(!data.CanDamage() || !(bool)data.playerVel.GetFieldValue("simulated")) {
                return;
            }

            if(data.GetAdditionalData().totalArmor > 0) {

                ArmorFramework.ArmorHandlers[data.player].ProcessDamage(ref damage, damagingPlayer, data.player, ArmorDamagePatchType.TakeDamage);
            }
        }

        [HarmonyPatch("TakeDamage", typeof(Vector2), typeof(Vector2), typeof(Color), typeof(GameObject), typeof(Player), typeof(bool), typeof(bool))]
        [HarmonyPostfix]
        public static void TakeDamagePostfix(HealthHandler __instance, Vector2 damage) => TakeDamageRunning = false;

        [HarmonyPatch("DoDamage")]
        [HarmonyPrefix]
        public static void DoDamage(HealthHandler __instance, ref Vector2 damage, Vector2 position, Color blinkColor, GameObject damagingWeapon, Player damagingPlayer, bool healthRemoval, bool lethal, bool ignoreBlock) {
            CharacterData data = (CharacterData)Traverse.Create(__instance).Field("data").GetValue();
            if(!data.CanDamage()) return;

            if(data.GetAdditionalData().totalArmor > 0) {
                ArmorFramework.ArmorHandlers[data.player].ProcessDamage(ref damage, damagingPlayer, data.player, ArmorDamagePatchType.DoDamage);
            }

            DeathHandler.PlayerDamaged(data.player, damagingPlayer, damage.magnitude);
        }



        [HarmonyPatch("Revive")]
        [HarmonyPostfix]
        public static void Revive(CharacterData ___data) {
            if(___data.GetComponent<ArmorHandler>()) {
                ___data.GetComponent<ArmorHandler>().OnRespawn();
            }
        }

        [HarmonyPatch("RPCA_Die")]
        [HarmonyPrefix]
        public static void RPCA_Die(Player ___player) {
            if(___player.data.dead) return;
            DeathHandler.PlayerDeath(___player);
        }

        [HarmonyPatch("RPCA_Die_Phoenix")]
        [HarmonyPrefix]
        public static void RPCA_Die_Phoenix(Player ___player) {
            if(___player.data.dead) return;
            DeathHandler.PlayerDeath(___player);
        }
    }
}


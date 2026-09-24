using HarmonyLib;
using JARL.Armor;
using UnboundLib;

namespace JARL.Patches {
    [HarmonyPatch(typeof(Player), "Start")]
    public class PlayerPatch {
        private static void Postfix(Player __instance) {
            __instance.gameObject.GetOrAddComponent<ArmorHandler>().ResetArmorStats();
        }
    }
}

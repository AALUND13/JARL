using HarmonyLib;
using JARL.Armor;
using JARL.Extensions;
using UnboundLib;

namespace JARL.Patches {
    [HarmonyPatch(typeof(CharacterStatModifiers))]
    public class CharacterStatModifiersPatch {
        [HarmonyPatch("ResetStats")]
        [HarmonyPrefix]
        public static void ResetStats(CharacterStatModifiers __instance) {
            CharacterData data = (CharacterData)Traverse.Create(__instance).Field("data").GetValue();

            data.gameObject.GetOrAddComponent<ArmorHandler>().ResetArmorStats();
            data.GetAdditionalData().ResetArmorStats();
        }
    }

}
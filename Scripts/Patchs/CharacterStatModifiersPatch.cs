using HarmonyLib;
using JARL.Extensions;

namespace JARL.Patchs {
    [HarmonyPatch(typeof(CharacterStatModifiers))]
    public class CharacterStatModifiersPatch {
        [HarmonyPatch("ResetStats")]
        [HarmonyPrefix]
        public static void ResetStats(CharacterStatModifiers __instance) {
            CharacterData data = (CharacterData)Traverse.Create(__instance).Field("data").GetValue();
            data.GetAdditionalData().ArmorPiercePercent = 0;
            ArmorFramework.ArmorFramework.ResetEveryPlayerArmorStats();
        }
    }

}
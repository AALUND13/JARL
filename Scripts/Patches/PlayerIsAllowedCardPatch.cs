using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using HarmonyLib;
using JARL.Armor;
using JARL.Extensions;
using System.Linq;

namespace JARL.Patches {
    [HarmonyPatch(typeof(ModdingUtils.Utils.Cards))]
    internal class PlayerIsAllowedCard {
        public static CardCategory DrawWhenPlayerHaveArmorCategory = CustomCardCategories.instance.CardCategory("DrawWhenPlayerHaveArmor");

        [HarmonyPatch("PlayerIsAllowedCard")]
        [HarmonyPostfix]
        public static void Postfix(Player player, CardInfo card, ref bool __result, ModdingUtils.Utils.Cards __instance) {
            bool otherPlayersNotHaveArmor = PlayerManager.instance.players
                .Where(p => p != player)
                .All(p => ArmorFramework.ArmorHandlers[p].ActiveArmors.Count == 0);

            // If other players don't have armor and the card is in the DrawWhenPlayerHaveArmor category, don't allow the card to be drawn
            if(otherPlayersNotHaveArmor
                && card.categories.Contains(DrawWhenPlayerHaveArmorCategory)
            ) __result = false;
        }
    }
}

using JARL.Bases;
using JARL.Extensions;
using UnityEngine;

namespace JARL {
    public class ArmorPierceCard : CustomUnityCard {
        [Header("Stats: Armor Pierce Percent")]
        public float ArmorPiercePercent;

        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats) {
            player.data.GetAdditionalData().ArmorPiercePercent = Mathf.Clamp(player.data.GetAdditionalData().ArmorPiercePercent + ArmorPiercePercent, 0, 1);
        }

        public override string GetModName() {
            return JustAnotherRoundsLibrary.ModInitials;
        }
    }
}

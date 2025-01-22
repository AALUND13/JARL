using JARL.Armor;
using JARL.Armor.Builtin;
using JARL.Bases;
using UnityEngine;
namespace JARL.Cards {
    public class DefaultArmorCard : CustomUnityCard {
        [Header("Armor: Max Armor Amd Regen")]
        public float MaxArmorValue;
        public float RegenerationRate;
        public float RegenCooldownSeconds;

        [Header("Armor: Armor Reactivate")]
        public ArmorReactivateType ArmorReactivateType;
        public float ReactivateArmorValue;
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats) {
            ArmorFramework.ArmorHandlers[player].AddArmor(typeof(DefaultArmor), MaxArmorValue, RegenerationRate, RegenCooldownSeconds, ArmorReactivateType, ReactivateArmorValue);
        }

        public override string GetModName() {
            return JustAnotherRoundsLibrary.ModInitials;
        }
    }
}

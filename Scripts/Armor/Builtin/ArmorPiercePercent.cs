using JARL.ArmorFramework.Classes;
using JARL.Extensions;
using System;

namespace JARL.ArmorFramework.Bases.Builtin {
    public class ArmorPiercePercent {
        /// <summary>
        /// Applies armor piercing percentage to the damage, considering the provided armor, damaging player, hurt player,
        /// remaining damage, original damage, and flags indicating lethality and whether to ignore blocking effects.
        /// </summary>
        /// <param name="armor">The armor to consider for armor piercing.</param>
        /// <param name="damageingPlayer">The player dealing the damage.</param>
        /// <param name="hurtPlayer">The player receiving the damage.</param>
        /// <param name="remaindingDamage">The remaining damage after initial reductions.</param>
        /// <param name="Damage">The original damage amount.</param>
        /// <returns>Modified damage and armor values.</returns>
        public static DamageAndArmorResult ApplyArmorPiercePercent(ArmorBase armor, Player damageingPlayer, Player hurtPlayer, float remaindingDamage, float Damage) {
            if(armor.armorTags.Contains("CanArmorPierce")) {
                // Calculate the armor piercing percentage
                float armorPiercePercent = (float)(damageingPlayer?.data?.GetAdditionalData()?.ArmorPiercePercent ?? 0);

                // Apply armor piercing to the damage
                float modifiedDamage = Math.Min(remaindingDamage + (Damage * armorPiercePercent), Damage);

                // Return the modified damage and the original armor
                return new DamageAndArmorResult(modifiedDamage, armor.currentArmorValue);
            } else {
                return new DamageAndArmorResult(remaindingDamage, armor.currentArmorValue);
            }
        }
    }
}

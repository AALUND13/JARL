using JARL.Extensions;
using System;

namespace JARL.Armor.Bases.Builtin {
    internal class ArmorPiercePercent {
        public static DamageArmorInfo ApplyArmorPiercePercent(ArmorBase armor, Player damageingPlayer, Player hurtPlayer, float remaindingDamage, float Damage) {
            if(armor.ArmorTags.Contains("CanArmorPierce")) {
                float armorPiercePercent = (float)(damageingPlayer?.data.GetAdditionalData().ArmorPiercePercent ?? 0);
                float modifiedDamage = Math.Min(remaindingDamage + (Damage * armorPiercePercent), Damage);

                return new DamageArmorInfo(modifiedDamage, armor.CurrentArmorValue);
            } else {
                return new DamageArmorInfo(remaindingDamage, armor.CurrentArmorValue);
            }
        }
    }
}

using JARL.Armor.Processors;
using JARL.Extensions;
using System;

namespace JARL.Armor.Bases.Builtin {
    internal class ArmorPiercePercentProcessor : ArmorProcessor {
        public override float AfterArmorProcess(float remaindingDamage, float originalDamage, float takenArmorDamage) {
            if(Armor.ArmorTags.Contains("CanArmorPierce")) {
                float armorPiercePercent = (float)(DamagingPlayer?.data.GetAdditionalData().ArmorPiercePercent ?? 0);
                float modifiedDamage = Math.Min(remaindingDamage + (originalDamage * armorPiercePercent), originalDamage);

                return modifiedDamage;
            } else {
                return remaindingDamage;
            }
        }
    }
}

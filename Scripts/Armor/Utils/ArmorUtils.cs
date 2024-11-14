using JARL.Armor.Bases;
using System;
using UnityEngine;

namespace JARL.Armor.Utlis {
    public static class ArmorUtils {
        public static DamageArmorInfo ApplyDamage(float armor, float damage) {
            float remainingArmor = Mathf.Max(0, armor - damage);
            float remainingDamage = Mathf.Max(0, damage - armor);

            var updatedDamageAndArmor = new DamageArmorInfo(remainingDamage, remainingArmor);

            // Returning the updated DamageAndArmorResult object
            return updatedDamageAndArmor;
        }

        public static ArmorBase GetRegisteredArmorByType(Type type) {
            return ArmorFramework.RegisteredArmorTypes.Find(armor => armor.GetType() == type);
        }
    }
}

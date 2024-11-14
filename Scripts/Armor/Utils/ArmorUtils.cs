using JARL.ArmorFramework.Bases;
using JARL.ArmorFramework.Classes;
using System.Collections.Generic;
using UnityEngine;

namespace JARL.ArmorFramework.Utlis {
    public static class ArmorUtils {
        /// <summary>
        /// Applies damage to the provided armor, calculating and returning the resulting damage and armor values.
        /// </summary>
        /// <param name="armor">The initial armor value before damage.</param>
        /// <param name="damage">The amount of damage to be applied.</param>
        /// <returns>A DamageAndArmorResult object containing the calculated remaining damage and armor values.</returns>
        public static DamageAndArmorResult ApplyDamage(float armor, float damage) {
            // Calculating remaining armor after damage is applied
            float remainingArmor = Mathf.Max(0, armor - damage);

            // Calculating remaining damage after armor reduction
            float remainingDamage = Mathf.Max(0, damage - armor);

            // Creating a new DamageAndArmorResult object with updated values
            DamageAndArmorResult updatedDamageAndArmor = new DamageAndArmorResult(remainingDamage, remainingArmor);

            // Returning the updated DamageAndArmorResult object
            return updatedDamageAndArmor;
        }

        /// <summary>
        /// Retrieves an active armor instance of the specified type from the provided ArmorHandler.
        /// </summary>
        /// <param name="armorHandler">The ArmorHandler instance containing the active armors.</param>
        /// <param name="type">The armor type to search for.</param>
        /// <returns>The active armor instance with the specified type, or null if not found.</returns>
        public static ArmorBase GetArmorByType(this ArmorHandler armorHandler, string type) {
            return armorHandler.armors.Find(armor => armor.GetArmorType() == type);
        }

        /// <summary>
        /// Retrieves an active armor instance of the specified type from the provided ArmorHandler.
        /// </summary>
        /// <param name="type">The armor type to search for.</param>
        /// <returns>The active armor instance with the specified type, or null if not found.</returns>
        public static ArmorBase GetRegisteredArmorByType(string type) {
            return ArmorFramework.registeredArmorTypes.Find(armor => armor.GetArmorType() == type);
        }

        /// <summary>
        /// Retrieves a list of currently active armors from the provided ArmorHandler.
        /// </summary>
        /// <param name="armorHandler">The ArmorHandler instance containing the active armors.</param>
        /// <returns>A list of active armors.</returns>
        public static List<ArmorBase> GetActiveArmors(this ArmorHandler armorHandler) {
            return armorHandler.armors.FindAll(armor => armor.maxArmorValue > 0);
        }
    }
}

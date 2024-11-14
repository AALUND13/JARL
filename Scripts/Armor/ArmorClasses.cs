using JARL.ArmorFramework.Bases;
using UnityEngine;

namespace JARL.ArmorFramework.Classes {
    public enum ArmorReactivateType {
        Percent, // Reactivate armor based on a percentage
        Second,   // Reactivate armor based on a time interval
        Null
    }

    /// <summary>
    /// Represents the result of damage and armor processing, including damage amount and armor value.
    /// </summary>
    public class DamageAndArmorResult {
        public float damage;
        public float armor;

        public DamageAndArmorResult(float damage, float armor) {
            this.damage = damage;
            this.armor = armor;
        }
    }

    /// <summary>
    /// Represents the result of armor processing, including damage amount, armor value, and a flag to skip armor damage processing.
    /// </summary>
    public class ArmorProcessingResult {
        public float damage;
        public float armor;
        public bool skipArmorDamageProcess;

        public ArmorProcessingResult(float damage, float armor, bool skipArmorDamageProcess) {
            this.damage = damage;
            this.armor = armor;
            this.skipArmorDamageProcess = skipArmorDamageProcess;
        }
    }

    /// <summary>
    /// Represents a combination of a health bar GameObject and an associated ArmorBase object.
    /// Useful for pairing health bars with specific armor instances in a game environment.
    /// </summary>
    public class HealthBarObjectWithArmor {
        public GameObject healthBarObject;
        public ArmorBase armor;

        public HealthBarObjectWithArmor(GameObject healthBarObject, ArmorBase armor) {
            this.healthBarObject = healthBarObject;
            this.armor = armor;
        }
    }

    public class BarColor {
        public Color activedBarColor;
        public Color deactivatedBarColor;

        public BarColor(Color activedBarColor, Color deactivatedBarColor) {
            this.activedBarColor = activedBarColor;
            this.deactivatedBarColor = deactivatedBarColor;
        }
    }
}

using System;
using UnityEngine;

namespace JARL.Armor {
    [Flags]
    public enum ArmorDamagePatchType {
        DoDamage = 1 << 0,
        TakeDamage = 1 << 1,
        TakeDamageOverTime = 1 << 2
    }

    public enum ArmorReactivateType {
        Percent, // Reactivate armor based on a percentage
        Second   // Reactivate armor based on a time interval
    }

    public struct DamageArmorInfo {
        public float Damage;
        public float Armor;

        public DamageArmorInfo(float damage, float armor) {
            Damage = damage;
            Armor = armor;
        }
    }

    public struct ArmorProcessingResult {
        public float Damage;
        public float Armor;
        public bool SkipArmorDamageProcess;

        public ArmorProcessingResult(float damage, float armor, bool skipArmorDamageProcess) {
            Damage = damage;
            Armor = armor;
            SkipArmorDamageProcess = skipArmorDamageProcess;
        }
    }

    public struct BarColor {
        public Color ActivedBarColor;
        public Color DeactivatedBarColor;

        public BarColor(Color activedBarColor, Color deactivatedBarColor) {
            ActivedBarColor = activedBarColor;
            DeactivatedBarColor = deactivatedBarColor;
        }
    }
}

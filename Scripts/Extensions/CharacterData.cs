using JARL.Armor;
using System;
using System.Runtime.CompilerServices;

namespace JARL.Extensions {
    public class JARLCharacterDataAdditionalData {
        public float totalArmor;
        public float totalMaxArmor;
        public float ArmorPiercePercent;

        public JARLCharacterDataAdditionalData() {
            totalArmor = 0;
            totalMaxArmor = 0;
            ArmorPiercePercent = 0;
        }
    }

    public static class CharacterDataExtensions {
        public static readonly ConditionalWeakTable<CharacterData, JARLCharacterDataAdditionalData> data = new ConditionalWeakTable<CharacterData, JARLCharacterDataAdditionalData>();

        public static JARLCharacterDataAdditionalData GetAdditionalData(this CharacterData block) {
            return data.GetOrCreateValue(block);
        }

        public static void AddData(this CharacterData block, JARLCharacterDataAdditionalData value) {
            try {
                data.Add(block, value);
            } catch(Exception) { }
        }
    }
}

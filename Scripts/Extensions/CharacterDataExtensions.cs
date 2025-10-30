using System;
using System.Runtime.CompilerServices;

namespace JARL.Extensions {
    public class JARLCharacterDataAdditionalData {
        public float totalArmor;
        public float totalMaxArmor;
        public float ArmorPiercePercent;

        public JARLCharacterDataAdditionalData() {
            ResetArmorStats();
        }

        public void ResetArmorStats() {
            totalArmor = 0;
            totalMaxArmor = 0;
            ArmorPiercePercent = 0;
        }
    }

    public static class CharacterDataExtensions {
        public static readonly ConditionalWeakTable<CharacterData, JARLCharacterDataAdditionalData> data = new ConditionalWeakTable<CharacterData, JARLCharacterDataAdditionalData>();

        public static JARLCharacterDataAdditionalData GetAdditionalData(this CharacterData data) {
            return CharacterDataExtensions.data.GetOrCreateValue(data);
        }

        public static void AddData(this CharacterData data, JARLCharacterDataAdditionalData value) {
            try {
                CharacterDataExtensions.data.Add(data, value);
            } catch(Exception) { }
        }
    }
}

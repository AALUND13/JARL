using JARL.Armor.Bases;
using UnityEngine;

namespace JARL.Armor.Builtin {
    public class DefaultArmor : ArmorBase {
        public override BarColor GetBarColor() {
            return new BarColor(Color.cyan * 0.6f, Color.cyan * 0.45f);
        }

        public DefaultArmor() {
            ArmorTags.Add("CanArmorPierce");
            reactivateArmorType = ArmorReactivateType.Second;
        }
    }
}

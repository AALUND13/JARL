using JARL.ArmorFramework.Bases;
using JARL.ArmorFramework.Classes;
using UnityEngine;

namespace JARL.ArmorFramework.Builtin
{
    public class DefaultArmor : ArmorBase
    {
        public override string GetArmorType()
        {
            return "Default";
        }

        public override BarColor GetBarColor()
        {
            return new BarColor(Color.cyan * 0.6f, Color.cyan * 0.45f);
        }

        public override void SetupArmor()
        {
            armorTags.Add("CanArmorPierce");
            reactivateArmorType = ArmorReactivateType.Second;
        }
    }
}

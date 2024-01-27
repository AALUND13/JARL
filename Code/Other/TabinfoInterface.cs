using BepInEx;
using JARL.ArmorFramework.Abstract;
using JARL.ArmorFramework.Utlis;
using JARL.Extensions;
using TabInfo.Utils;
using UnboundLib;

namespace JARL
{
    internal class TabinfoInterface
    {
        public static object armorsStatsCategory = null;
        public static void SetUpTabinfoInterface()
        {
            StatCategory JARLStatsCategory = TabInfoManager.RegisterCategory("JARL Stats", 0);
            ExtensionMethods.SetFieldValue(JARLStatsCategory, "priority", -45);

            armorsStatsCategory = TabInfoManager.RegisterCategory("Armor Stats", 0);
            ExtensionMethods.SetFieldValue(armorsStatsCategory, "priority", -44);

            TabInfoManager.RegisterStat(JARLStatsCategory, "Total Armor", (p) => p.data.GetAdditionalData().totalMaxArmor > 0, (p) => $"{p.data.GetAdditionalData().totalArmor:0.0}/{p.data.GetAdditionalData().totalMaxArmor:0.0}");
            TabInfoManager.RegisterStat(JARLStatsCategory, "Armor Pierce", (p) => p.data.GetAdditionalData().ArmorPiercePercent > 0, (p) => $"{p.data.GetAdditionalData().ArmorPiercePercent * 100:0}%");
        }

        public static void RegisterArmorTabinfoInterface(ArmorBase registerArmor)
        {
            if (JustAnotherRoundsLibrary.plugins.Exists(plugin => plugin.Info.Metadata.GUID == "com.willuwontu.rounds.tabinfo"))
            {
                string armorType = registerArmor.GetArmorType();
                TabInfoManager.RegisterStat((StatCategory)armorsStatsCategory, $"{armorType} Armor", (p) => p.data.GetAdditionalData().armorHandler.GetArmorByType(armorType).maxArmorValue > 0, (p) =>
                {
                    ArmorBase armor = p.data.GetAdditionalData().armorHandler.GetArmorByType(armorType);
                    if (armor.currentArmorValue <= 0 && !armor.deactivateText.IsNullOrWhiteSpace())
                    {
                        return armor.deactivateText;
                    }
                    else
                    {
                        return $"{armor.currentArmorValue}/{armor.maxArmorValue}";
                    }
                });


                StatCategory armorStatsCategory = TabInfoManager.RegisterCategory($"{armorType} Armor Stats", 127);
                TabInfoManager.RegisterStat(armorStatsCategory, $"{armorType} Armor Health", (p) => p.data.GetAdditionalData().armorHandler.GetArmorByType(armorType).maxArmorValue > 0 && Utils.DetailsMode,
                    (p) => $"{p.data.GetAdditionalData().armorHandler.GetArmorByType(armorType).currentArmorValue:0.0}/{p.data.GetAdditionalData().armorHandler.GetArmorByType(armorType).maxArmorValue:0.0}");
                TabInfoManager.RegisterStat(armorStatsCategory, $"{armorType} Armor Regeneration Rate", (p) => p.data.GetAdditionalData().armorHandler.GetArmorByType(armorType).maxArmorValue > 0 && Utils.DetailsMode,
                    (p) => $"{p.data.GetAdditionalData().armorHandler.GetArmorByType(armorType).armorRegenerationRate:0.00}");
                TabInfoManager.RegisterStat(armorStatsCategory, $"{armorType} Armor Regen Cooldown ", (p) => p.data.GetAdditionalData().armorHandler.GetArmorByType(armorType).maxArmorValue > 0 && Utils.DetailsMode,
                    (p) => $"{p.data.GetAdditionalData().armorHandler.GetArmorByType(armorType).armorRegenCooldownSeconds:0.00}s");
                TabInfoManager.RegisterStat(armorStatsCategory, $"{armorType} Armor Reactivate Armor Type", (p) => p.data.GetAdditionalData().armorHandler.GetArmorByType(armorType).maxArmorValue > 0 && Utils.DetailsMode,
                    (p) => $"{p.data.GetAdditionalData().armorHandler.GetArmorByType(armorType).reactivateArmorType}");
                TabInfoManager.RegisterStat(armorStatsCategory, $"{armorType} Armor Reactivate Armor Value", (p) => p.data.GetAdditionalData().armorHandler.GetArmorByType(armorType).maxArmorValue > 0 && Utils.DetailsMode,
                    (p) => $"{p.data.GetAdditionalData().armorHandler.GetArmorByType(armorType).reactivateArmorValue}");
            }
        }
    }
}

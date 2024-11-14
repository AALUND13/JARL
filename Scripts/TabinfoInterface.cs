using BepInEx;
using JARL.Armor;
using JARL.Armor.Bases;
using JARL.Extensions;
using System;
using TabInfo.Utils;
using UnboundLib;

namespace JARL {
    internal class TabinfoInterface {
        public static object ArmorsStatsCategory = null;
        public static void SetUpTabinfoInterface() {
            StatCategory JARLStatsCategory = TabInfoManager.RegisterCategory("JARL Stats", 0);
            ExtensionMethods.SetFieldValue(JARLStatsCategory, "priority", -45);

            ArmorsStatsCategory = TabInfoManager.RegisterCategory("Armor Stats", 0);
            ExtensionMethods.SetFieldValue(ArmorsStatsCategory, "priority", -44);

            TabInfoManager.RegisterStat(JARLStatsCategory, "Total Armor", (p) => p.data.GetAdditionalData().totalMaxArmor > 0, (p) => $"{p.data.GetAdditionalData().totalArmor:0.0}/{p.data.GetAdditionalData().totalMaxArmor:0.0}");
            TabInfoManager.RegisterStat(JARLStatsCategory, "Armor Pierce", (p) => p.data.GetAdditionalData().ArmorPiercePercent > 0, (p) => $"{p.data.GetAdditionalData().ArmorPiercePercent * 100:0}%");
        }

        public static void RegisterArmorTabinfoInterface(ArmorBase registerArmor) {
            if(JustAnotherRoundsLibrary.plugins.Exists(plugin => plugin.Info.Metadata.GUID == "com.willuwontu.rounds.tabinfo")) {
                Type armorType = registerArmor.GetType();
                TabInfoManager.RegisterStat((StatCategory)ArmorsStatsCategory, $"{registerArmor} Armor", (p) => ArmorFramework.ArmorHandlers[p].GetArmorByType(armorType).MaxArmorValue > 0, (p) => {
                    ArmorBase armor = ArmorFramework.ArmorHandlers[p].GetArmorByType(armorType);
                    if(armor.CurrentArmorValue <= 0 && !armor.DeactivateText.IsNullOrWhiteSpace()) {
                        return armor.DeactivateText;
                    } else {
                        return $"{armor.CurrentArmorValue:0.0}/{armor.MaxArmorValue:0.0}";
                    }
                });


                StatCategory armorStatsCategory = TabInfoManager.RegisterCategory($"{registerArmor} Armor Stats", 127);

                TabInfoManager.RegisterStat(armorStatsCategory, $"{registerArmor} Armor Health", (p) => ArmorFramework.ArmorHandlers[p].GetArmorByType(armorType).MaxArmorValue > 0 && ConfigHandler.DetailsMode.Value,
                    (p) => $"{ArmorFramework.ArmorHandlers[p].GetArmorByType(armorType).CurrentArmorValue:0.0}/{ArmorFramework.ArmorHandlers[p].GetArmorByType(armorType).MaxArmorValue:0.0}");

                TabInfoManager.RegisterStat(armorStatsCategory, $"{registerArmor} Armor Regeneration Rate", (p) => ArmorFramework.ArmorHandlers[p].GetArmorByType(armorType).MaxArmorValue > 0 && ConfigHandler.DetailsMode.Value,
                    (p) => $"{ArmorFramework.ArmorHandlers[p].GetArmorByType(armorType).ArmorRegenerationRate:0.00}");

                TabInfoManager.RegisterStat(armorStatsCategory, $"{registerArmor} Armor Regen Cooldown ", (p) => ArmorFramework.ArmorHandlers[p].GetArmorByType(armorType).MaxArmorValue > 0 && ConfigHandler.DetailsMode.Value,
                    (p) => $"{ArmorFramework.ArmorHandlers[p].GetArmorByType(armorType).ArmorRegenCooldownSeconds:0.00}s");

                TabInfoManager.RegisterStat(armorStatsCategory, $"{registerArmor} Armor Reactivate Armor Type", (p) => ArmorFramework.ArmorHandlers[p].GetArmorByType(armorType).MaxArmorValue > 0 && ConfigHandler.DetailsMode.Value,
                    (p) => $"{ArmorFramework.ArmorHandlers[p].GetArmorByType(armorType).reactivateArmorType}");

                TabInfoManager.RegisterStat(armorStatsCategory, $"{registerArmor} Armor Reactivate Armor Value", (p) => ArmorFramework.ArmorHandlers[p].GetArmorByType(armorType).MaxArmorValue > 0 && ConfigHandler.DetailsMode.Value,
                    (p) => $"{ArmorFramework.ArmorHandlers[p].GetArmorByType(armorType).reactivateArmorValue}");
            }
        }
    }
}

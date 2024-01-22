using JARL.Extensions;
using TabInfo.Utils;
using UnboundLib;

namespace JARL
{
    internal class TabinfoInterface
    {
        public static void SetUpTabinfoInterface()
        {
            StatCategory JARLStatsCategory = TabInfoManager.RegisterCategory("JARL Stats", 0);
            ExtensionMethods.SetFieldValue(JARLStatsCategory, "priority", -45);
            TabInfoManager.RegisterStat(JARLStatsCategory, "Total Armor", (p) => p.data.GetAdditionalData().totalMaxArmor > 0, (p) => $"{p.data.GetAdditionalData().totalArmor:0.0}/{p.data.GetAdditionalData().totalMaxArmor:0.0}");
            TabInfoManager.RegisterStat(JARLStatsCategory, "Armor Pierce", (p) => p.data.GetAdditionalData().ArmorPiercePercent > 0, (p) => $"{p.data.GetAdditionalData().ArmorPiercePercent * 100:0}%");
        }
    }
}

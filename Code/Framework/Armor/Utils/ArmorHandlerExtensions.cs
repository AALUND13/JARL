using JARL.ArmorFramework.Abstract;
using JARL.ArmorFramework.Classes;
using Photon.Pun;
using UnboundLib;
using UnityEngine;

namespace JARL.ArmorFramework.Utlis
{
    public static class ArmorHandlerExtensions
    {
        /// <summary>
        /// Adds armor to the specified armor type in the given ArmorHandler with default regeneration values.
        /// </summary>
        /// <param name="armorHandler">The ArmorHandler containing the armor.</param>
        /// <param name="armorType">The type of armor to be added.</param>
        /// <param name="maxArmorValue">The maximum armor value to be added.</param>
        public static void AddArmor(this ArmorHandler armorHandler, string armorType, float maxArmorValue)
        {
            AddArmor(armorHandler, armorType, maxArmorValue, 0, 0);
        }

        /// <summary>
        /// Adds armor to the specified armor type in the given ArmorHandler with custom regeneration values.
        /// </summary>
        /// <param name="armorHandler">The ArmorHandler containing the armor.</param>
        /// <param name="armorType">The type of armor to be added.</param>
        /// <param name="maxArmorValue">The maximum armor value to be added.</param>
        /// <param name="regenerationRate">The armor regeneration rate.</param>
        /// <param name="regenCooldownSeconds">The cooldown for armor regeneration.</param>
        public static void AddArmor(this ArmorHandler armorHandler, string armorType, float maxArmorValue, float regenerationRate, float regenCooldownSeconds)
        {
            AddArmor(armorHandler, armorType, maxArmorValue, regenerationRate, regenCooldownSeconds, ArmorReactivateType.Null, 0);
        }

        /// <summary>
        /// Adds armor to the specified armor type in the given ArmorHandler with custom regeneration and reactivation values.
        /// </summary>
        /// <param name="armorHandler">The ArmorHandler containing the armor.</param>
        /// <param name="armorType">The type of armor to be added.</param>
        /// <param name="maxArmorValue">The maximum armor value to be added.</param>
        /// <param name="regenerationRate">The armor regeneration rate.</param>
        /// <param name="regenCooldownSeconds">The cooldown for armor regeneration.</param>
        /// <param name="reactivateArmorType">The reactivation type of armor.</param>
        /// <param name="reactivateArmorValue">The reactivation value of armor.</param>
        public static void AddArmor(this ArmorHandler armorHandler, string armorType, float maxArmorValue, float regenerationRate, float regenCooldownSeconds, ArmorReactivateType reactivateArmorType, float reactivateArmorValue)
        {
            if (PhotonNetwork.IsMasterClient || PhotonNetwork.OfflineMode)
            {
                NetworkingManager.RaiseEvent("AddArmor", new object[7] { armorHandler, armorType, maxArmorValue, regenerationRate, regenCooldownSeconds, reactivateArmorType, reactivateArmorValue });
            }

            ArmorBase armor = ArmorUtils.GetArmorByType(armorHandler, armorType);

            armor.maxArmorValue += Mathf.Max(maxArmorValue, 0);
            armor.armorRegenerationRate += Mathf.Max(regenerationRate, 0);

            if (armor.armorRegenCooldownSeconds < regenCooldownSeconds)
            {
                armor.armorRegenCooldownSeconds = regenCooldownSeconds;
            }



            if (reactivateArmorType != ArmorReactivateType.Null)
            {
                armor.reactivateArmorType = reactivateArmorType;
            }

            armor.reactivateArmorValue = reactivateArmorValue;
        }

        /// <summary>
        /// Adds armor to the specified armor type in the given ArmorHandler with default regeneration values.
        /// </summary>
        /// <param name="playerID">The Id the player.</param>
        /// <param name="armorType">The type of armor to be added.</param>
        /// <param name="maxArmorValue">The maximum armor value to be added.</param>
        public static void AddArmor(int playerID, string armorType, float maxArmorValue)
        {
            AddArmor(playerID, armorType, maxArmorValue, 0, 0);
        }

        /// <summary>
        /// Adds armor to the specified armor type in the given ArmorHandler with custom regeneration values.
        /// </summary>
        /// <param name="playerID">TThe Id the player.</param>
        /// <param name="armorType">The type of armor to be added.</param>
        /// <param name="maxArmorValue">The maximum armor value to be added.</param>
        /// <param name="regenerationRate">The armor regeneration rate.</param>
        /// <param name="regenCooldownSeconds">The cooldown for armor regeneration.</param>
        public static void AddArmor(int playerID, string armorType, float maxArmorValue, float regenerationRate, float regenCooldownSeconds)
        {
            AddArmor(playerID, armorType, maxArmorValue, regenerationRate, regenCooldownSeconds, ArmorReactivateType.Null, 0);
        }

        /// <summary>
        /// Adds armor to the specified armor type in the given ArmorHandler with custom regeneration and reactivation values.
        /// </summary>
        /// <param name="playerID">The Id the player.</param>
        /// <param name="armorType">The type of armor to be added.</param>
        /// <param name="maxArmorValue">The maximum armor value to be added.</param>
        /// <param name="regenerationRate">The armor regeneration rate.</param>
        /// <param name="regenCooldownSeconds">The cooldown for armor regeneration.</param>
        /// <param name="reactivateArmorType">The reactivation type of armor.</param>
        /// <param name="reactivateArmorValue">The reactivation value of armor.</param>
        public static void AddArmor(int playerID, string armorType, float maxArmorValue, float regenerationRate, float regenCooldownSeconds, ArmorReactivateType reactivateArmorType, float reactivateArmorValue)
        {
            if (PhotonNetwork.IsMasterClient || PhotonNetwork.OfflineMode)
            {
                NetworkingManager.RaiseEvent("AddArmor", new object[7] { playerID, armorType, maxArmorValue, regenerationRate, regenCooldownSeconds, reactivateArmorType, reactivateArmorValue });
            }
        }
    }
}

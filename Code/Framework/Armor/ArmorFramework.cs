using JARL.ArmorFramework.Abstract;
using JARL.ArmorFramework.Classes;
using JARL.ArmorFramework.Utlis;
using JARL.Extensions;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnityEngine;

namespace JARL.ArmorFramework
{
    public class ArmorFramework
    {
        /// <summary>
        /// List of all registered armor types.
        /// </summary>
        public static List<ArmorBase> registeredArmorTypes = new List<ArmorBase>();

        /// <summary>
        /// Registers a new armor type.
        /// </summary>
        /// <param name="armorType">The armor type to register.</param>
        public static void RegisterArmorType(ArmorBase armorType)
        {
            Utils.LogInfo($"Registering ArmorType: '{armorType.GetArmorType()}'");

            if (registeredArmorTypes.Contains(armorType))
            {
                Utils.LogWarn($"ArmorType '{armorType.GetArmorType()}' already exists");
                return;
            }

            if (registeredArmorTypes.Any(armor => armor.GetPriority() == armorType.GetPriority()))
            {
                Utils.LogWarn($"Another ArmorType with the same priority already exists");
                return;
            }

            registeredArmorTypes.Add(armorType);

            Utils.LogInfo($"Successfully register ArmorType: '{armorType.GetArmorType()}'");
        }

        /// <summary>
        /// Resets the armor statistics for each player in the game, initializing the associated ArmorHandler if not present.
        /// This method iterates through all players managed by the PlayerManager, and if a player lacks an ArmorHandler component,
        /// it adds and initializes one by invoking the ResetArmorStats method.
        /// </summary>
        /// <param name="skipArmorHandlerChecking">If true, skips checking for the ArmorHandler component before adding it. Default is true.</param>
        public static void ResetEveryPlayerArmorStats(bool skipArmorHandlerChecking = true)
        {
            for (int i = 0; i < PlayerManager.instance.players.Count; i++)
            {
                Player player = PlayerManager.instance.players[i];
                if (skipArmorHandlerChecking || player.GetComponent<ArmorHandler>() == null)
                {
                    Utils.LogInfo("Reseting Stats");
                    ArmorHandler armorHandler = player.gameObject.GetOrAddComponent<ArmorHandler>();
                    player.data.GetAdditionalData().armorHandler = armorHandler;
                    armorHandler.ResetArmorStats();
                }
            }
        }

        internal static void SetupNetworkEvent()
        {
            NetworkingManager.RegisterEvent("AddArmor", (data) =>
            {
                ArmorBase armor = ArmorUtils.GetArmorByType(PlayerManager.instance.players.Find(player => player.playerID == (int)data[0]).GetComponent<ArmorHandler>(), (string)data[1]);

                armor.maxArmorValue += Mathf.Max((float)data[2], 0);
                armor.armorRegenerationRate += Mathf.Max((float)data[3], 0);

                if (armor.armorRegenCooldownSeconds < (float)data[4])
                {
                    armor.armorRegenCooldownSeconds = (float)data[4];
                }



                if ((ArmorReactivateType)data[5] != ArmorReactivateType.Null)
                {
                    armor.reactivateArmorType = (ArmorReactivateType)data[5];
                }

                armor.reactivateArmorValue = (float)data[6];
            });
        }
    }
}

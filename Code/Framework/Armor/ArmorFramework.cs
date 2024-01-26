using JARL.ArmorFramework.Abstract;
using JARL.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;

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

            // Ensure each armor type has a unique priority
            while (registeredArmorTypes.Any(armor => armor.priority == armorType.priority))
            {
                armorType.priority++;
            }

            registeredArmorTypes.Add(armorType);

            RegisterArmorTabinfoInterface(armorType);

            Utils.LogInfo($"Successfully registered ArmorType: '{armorType.GetArmorType()}'");
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

        internal static void RegisterArmorTabinfoInterface(ArmorBase armor)
        {
            if (JustAnotherRoundsLibrary.plugins.Exists(plugin => plugin.Info.Metadata.GUID == "com.willuwontu.rounds.tabinfo"))
            {
                TabinfoInterface.RegisterArmorTabinfoInterface(armor);
            }
        }
    }
}

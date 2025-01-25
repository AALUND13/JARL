using JARL.Armor.Bases;
using JARL.Utils;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using System;

namespace JARL.Armor {
    public class ArmorFramework {
        public static List<ArmorBase> RegisteredArmorTypes { get; private set; } = new List<ArmorBase>();

        public static readonly Dictionary<Player, ArmorHandler> ArmorHandlers = new Dictionary<Player, ArmorHandler>();

        public static void RegisterArmorType<T>() where T : ArmorBase, new() {
            RegisterArmorType(new T());
        }

        public static void RegisterArmorType(ArmorBase armorType) {
            if(armorType == null) 
                throw new ArgumentNullException(nameof(armorType));
            else if(RegisteredArmorTypes.Contains(armorType)) 
                throw new Exception($"ArmorType '{armorType.GetType().Name}' is already registered");

            LoggingUtils.LogInfo($"Registering ArmorType: '{armorType.GetType().Name}'");


            RegisteredArmorTypes.Add(armorType);

            armorType.OnRegister();
            EnsureUniquePriorities(RegisteredArmorTypes);

            RegisterArmorTabinfoInterface(armorType);

            LoggingUtils.LogInfo($"Successfully registered ArmorType: '{armorType.GetType().Name}'");
        }

        public static void ResetEveryPlayerArmorStats(bool skipArmorHandlerChecking = true) {
            foreach(Player player in PlayerManager.instance.players) {
                if(skipArmorHandlerChecking || player.GetComponent<ArmorHandler>() == null) {
                    LoggingUtils.LogInfo($"Reseting player id '{player.playerID}' armor stats");
                    var armorHandler = player.gameObject.GetOrAddComponent<ArmorHandler>();
                    armorHandler.ResetArmorStats();
                }
            }
        }

        internal static void RegisterArmorTabinfoInterface(ArmorBase armor) {
            if(JustAnotherRoundsLibrary.Plugins.Exists(plugin => plugin.Info.Metadata.GUID == "com.willuwontu.rounds.tabinfo")) {
                TabinfoInterface.RegisterArmorTabinfoInterface(armor);
            }
        }

        public static void EnsureUniquePriorities(List<ArmorBase> registeredArmorTypes) {
            var sortedArmorTypes = registeredArmorTypes.OrderBy(a => a.Priority).ThenBy(a => a.GetType().Name).ToList();

            int currentPriority = sortedArmorTypes.FirstOrDefault()?.Priority ?? 0;

            foreach(var armor in sortedArmorTypes) {
                if(armor.Priority <= currentPriority) {
                    armor.Priority = currentPriority + 1;
                }
                currentPriority = armor.Priority;
            }

            registeredArmorTypes = sortedArmorTypes;
        }
    }
}

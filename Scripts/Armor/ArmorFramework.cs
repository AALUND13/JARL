using JARL.Armor.Bases;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;

namespace JARL.Armor {
    public class ArmorFramework {
        public static readonly List<ArmorBase> RegisteredArmorTypes = new List<ArmorBase>();

        public static readonly Dictionary<Player, ArmorHandler> ArmorHandlers = new Dictionary<Player, ArmorHandler>();

        public static void RegisterArmorType(ArmorBase armorType) {
            LoggingUtils.LogInfo($"Registering ArmorType: '{armorType.GetType().Name}'");

            if(RegisteredArmorTypes.Contains(armorType)) {
                LoggingUtils.LogWarn($"ArmorType '{armorType.GetType().Name}' already exists");
                return;
            }

            var maxPriority = RegisteredArmorTypes.Count > 0 ? RegisteredArmorTypes.Max(a => a.Priority) : 0;
            armorType.Priority = maxPriority + 1;

            RegisteredArmorTypes.Add(armorType);

            RegisterArmorTabinfoInterface(armorType);

            LoggingUtils.LogInfo($"Successfully registered ArmorType: '{armorType.GetType().Name}'");
        }

        public static void ResetEveryPlayerArmorStats(bool skipArmorHandlerChecking = true) {
            for(int i = 0; i < PlayerManager.instance.players.Count; i++) {
                Player player = PlayerManager.instance.players[i];
                if(skipArmorHandlerChecking || player.GetComponent<ArmorHandler>() == null) {
                    LoggingUtils.LogInfo($"Reseting player id '{player.playerID}' armor stats");
                    var armorHandler = player.gameObject.GetOrAddComponent<ArmorHandler>();
                    armorHandler.ResetArmorStats();
                }
            }
        }

        internal static void RegisterArmorTabinfoInterface(ArmorBase armor) {
            if(JustAnotherRoundsLibrary.plugins.Exists(plugin => plugin.Info.Metadata.GUID == "com.willuwontu.rounds.tabinfo")) {
                TabinfoInterface.RegisterArmorTabinfoInterface(armor);
            }
        }
    }
}

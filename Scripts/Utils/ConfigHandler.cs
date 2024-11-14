using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnboundLib;
using UnboundLib.Utils.UI;
using UnityEngine;

namespace JARL
{
    internal class ConfigHandler
    {
        public static ConfigEntry<bool> DetailsMode;
        public static ConfigEntry<bool> DebugMode;

        public static void RegesterMenu(ConfigFile config)
        {
            Unbound.RegisterMenu(JustAnotherRoundsLibrary.ModName, () => { }, NewGui, null, false);
            DetailsMode = config.Bind(JustAnotherRoundsLibrary.ModName, "DetailsMode", false, "Enabled or disabled DetailsMode.");
            DebugMode = config.Bind(JustAnotherRoundsLibrary.ModName, "DebugMode", false, "Enabled or disabled Debug Mode");
        }

        public static void addBlank(GameObject menu)
        {
            MenuHandler.CreateText(" ", menu, out TextMeshProUGUI _, 30);
        }

        public static void NewGui(GameObject menu)
        {
            MenuHandler.CreateText("Details Mode | Show extracts details about every armor.", menu, out TextMeshProUGUI _, 30);
            MenuHandler.CreateToggle(DetailsMode.Value, "Details Mode", menu, DetailsModeChanged, 30);
            addBlank(menu);;
            MenuHandler.CreateToggle(DebugMode.Value, "<#c41010> Debug Mode", menu, DebugModeChanged, 30);
            void DetailsModeChanged(bool val)
            {
                DetailsMode.Value = val;
            }
            void DebugModeChanged(bool val)
            {
                DebugMode.Value = val;
            }
        }
    }
}

﻿using BepInEx;
using ClassesManagerReborn;
using HarmonyLib;
using JARL;
using JARL.Armor;
using JARL.Armor.Bases.Builtin;
using JARL.Armor.Builtin;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[BepInDependency("com.willis.rounds.unbound")]
[BepInDependency("pykess.rounds.plugins.moddingutils")]
[BepInDependency("com.willis.rounds.modsplus")]
[BepInDependency("root.classes.manager.reborn")]
[BepInDependency("com.willuwontu.rounds.tabinfo", BepInDependency.DependencyFlags.SoftDependency)]
[BepInPlugin(ModId, ModName, Version)]
[BepInProcess("Rounds.exe")]

public class JustAnotherRoundsLibrary : BaseUnityPlugin {
    internal const string ModInitials = "JARL";
    internal const string ModId = "com.aalund13.rounds.jarl";
    internal const string ModName = "Just Another Rounds Library";
    internal const string Version = "1.3.1"; // What version are we on (major.minor.patch)?

    internal static List<BaseUnityPlugin> plugins;
    internal static AssetBundle assets = Jotunn.Utils.AssetUtils.LoadAssetBundleFromResources("jarl_assets", typeof(JustAnotherRoundsLibrary).Assembly);

    void Awake() {
        assets.LoadAsset<GameObject>("ModCards").GetComponent<CardResgester>().RegisterCards();

        var harmony = new Harmony(ModId);
        harmony.PatchAll();

        ClassesRegistry.Register(CardResgester.ModCards["Armor Piercing"].GetComponent<CardInfo>(), CardType.NonClassCard, 4);
    }

    void Start() {
        ConfigHandler.RegesterMenu(Config);

        plugins = (List<BaseUnityPlugin>)typeof(BepInEx.Bootstrap.Chainloader).GetField("_plugins", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
        UnboundLib.GameModes.GameModeManager.AddHook(UnboundLib.GameModes.GameModeHooks.HookGameStart, (_) => GameStart());

        if(plugins.Exists(plugin => plugin.Info.Metadata.GUID == "com.willuwontu.rounds.tabinfo")) {
            TabinfoInterface.SetUpTabinfoInterface();
        }

        ArmorFramework.RegisterArmorType(new DefaultArmor());
        ArmorHandler.DamageProcessingMethodsAfter += ArmorPiercePercent.ApplyArmorPiercePercent;
    }

    void Update() {
        ArmorFramework.ResetEveryPlayerArmorStats(false);
    }

    IEnumerator GameStart() {
        ArmorFramework.ResetEveryPlayerArmorStats();
        yield break;
    }
}
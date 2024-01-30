using BepInEx;
using ClassesManagerReborn;
using HarmonyLib;
using JARL;
using JARL.ArmorFramework;
using JARL.ArmorFramework.Abstract.Builtin;
using JARL.ArmorFramework.Builtin;
using JARL.Extensions;
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

public class JustAnotherRoundsLibrary : BaseUnityPlugin
{
    internal const string modInitials = "JARL";
    internal const string ModId = "com.aalund13.rounds.jarl";
    internal const string ModName = "Just Another Rounds Library";
    internal const string Version = "1.2.6"; // What version are we on (major.minor.patch)?
    public static List<BaseUnityPlugin> plugins;

    internal static AssetBundle assets = Jotunn.Utils.AssetUtils.LoadAssetBundleFromResources("jarl_asset", typeof(JustAnotherRoundsLibrary).Assembly);

    public static CardCategory SoulstreakClassCards;

    void Awake()
    {
        assets.LoadAsset<GameObject>("ModCards").GetComponent<JARLCardResgester>().RegisterCards();

        var harmony = new Harmony(ModId);
        harmony.PatchAll();

        ClassesRegistry.Register(JARLCardResgester.ModCards["Armor Piercing"].GetComponent<CardInfo>(), CardType.Card, 4);
    }
    void Start()
    {
        ConfigHandler.RegesterMenu(Config);

        plugins = (List<BaseUnityPlugin>)typeof(BepInEx.Bootstrap.Chainloader).GetField("_plugins", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
        
        UnboundLib.GameModes.GameModeManager.AddHook(UnboundLib.GameModes.GameModeHooks.HookGameStart, (_) => GameStart());

        if (plugins.Exists(plugin => plugin.Info.Metadata.GUID == "com.willuwontu.rounds.tabinfo"))
        {
            TabinfoInterface.SetUpTabinfoInterface();
        }

        ArmorFramework.RegisterArmorType(new DefaultArmor());
        ArmorHandler.DamageProcessingMethodsAfter.Add("ApplyArmorPiercePercent", ArmorPiercePercent.ApplyArmorPiercePercent);
    }

    void Update()
    {
        ArmorFramework.ResetEveryPlayerArmorStats(false);
    }

    IEnumerator GameStart()
    {
        ArmorFramework.ResetEveryPlayerArmorStats();
        for (int i = 0; i < PlayerManager.instance.players.Count; i++)
        {
            Player player = PlayerManager.instance.players[i];
            player.data.GetAdditionalData().ArmorPiercePercent = 0;
        }
        yield break;
    }
}

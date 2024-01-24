﻿using JARL.Abstract;
using JARL.ArmorFramework.Classes;
using JARL.ArmorFramework.Utlis;
using JARL.Extensions;
using UnityEngine;

namespace JARL
{
    public class DefaultArmorCard : CustomCardUnity
    {
        [Header("Armor: Max Armor Amd Regen")]
        public float maxArmorValue;
        public float regenerationRate;
        public float regenCooldownSeconds;

        [Header("Armor: Armor Reactivate")]
        public ArmorReactivateType armorReactivateType;
        public float reactivateArmorValue;
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            
            ArmorHandlerExtensions.AddArmor(player.playerID, "Default", maxArmorValue, regenerationRate, regenCooldownSeconds, armorReactivateType, reactivateArmorValue);
        }

        public override string GetModName()
        {
            return JustAnotherRoundsLibrary.modInitials;
        }
    }
}

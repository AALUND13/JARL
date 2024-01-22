using JARL.ArmorFramework.Classes;
using JARL.ArmorFramework.Utlis;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JARL.ArmorFramework.Abstract
{
    /// <summary>
    /// Represents an abstract class for armor.
    /// </summary>
    public abstract class ArmorBase : ICloneable
    {
        public object Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>
        /// Abstract method to get the type of the armor.
        /// </summary>
        /// <returns>Returns the string representing the armor type.</returns>
        public abstract string GetArmorType();

        /// <summary>
        /// Abstract method to get the color of the armor bar.
        /// </summary>
        /// <returns>Returns the color that the armor bar should be.</returns>
        public abstract BarColor GetBarColor();

        /// <summary>
        /// Abstract method to get the priority of the armor.
        /// </summary>
        /// <returns>Returns an integer representing the priority of the armor.</returns>
        public abstract int GetPriority();

        // Properties for a list of tags
        public List<string> armorTags = new List<string>();

        // Maximum and current armor values
        public float maxArmorValue = 0; // Maximum armor value
        public float currentArmorValue = 0; // Current armor value

        // Properties for armor regeneration and cooldown
        public float armorRegenerationRate = 0; // Rate at which armor regenerates
        public float armorRegenCooldownSeconds = 0; // Time in seconds before armor regeneration reactivates after taking damage

        // Properties for armor reactivation
        public ArmorReactivateType reactivateArmorType = ArmorReactivateType.Percent; // Type of reactivation (percentage or time interval)
        public float reactivateArmorValue = 0; // Value for armor reactivation (leave at 0 for instant reactivation)

        // Properties for deactivate text
        public string deactivateText = ""; // Text to display when armor is deactivated (leave empty to display 0)

        // Indicates whether the armor is currently active.
        public bool isActive = false;

        // Represents the time elapsed since the last damage was taken.
        public float timeSinceLastDamage = 0;

        // Indicates whether armor should fully regenerate after revive.
        public bool regenerateFullyAfterRevive = true;

        // The "ArmorHandler" that this armor bind too
        public ArmorHandler armorHandler;

        /// <summary>
        /// Processes damage inflicted on the armor, taking the current armor value into account.
        /// Override this method in derived classes to implement custom armor damage calculations.
        /// </summary>
        /// <param name="damage">The incoming damage to be processed.</param>
        /// <param name="DamagingPlayer">The player dealing the damage.</param>
        /// <returns>The result of damage and armor processing, including the modified damage and armor values.</returns>
        public virtual DamageAndArmorResult OnDamage(float damage, Player DamagingPlayer)
        {
            return ArmorUtils.ApplyDamage(currentArmorValue, damage);
        }

        public virtual void OnArmorDamage(float damage, Player DamagingPlayer)
        {

        }

        /// <summary>
        /// Sets up any initial configuration for the armor. Override this method in derived classes to implement specific setup logic.
        /// </summary>
        public virtual void SetupArmor() { }

        /// <summary>
        /// Checks if the armor has a specified tag.
        /// </summary>
        /// <param name="armorTag">The tag to check for.</param>
        /// <returns>True if the armor has the specified tag; otherwise, false.</returns>
        public bool HasArmorTag(string armorTag)
        {
            return armorTags.Contains(armorTag);
        }

        /// <summary>
        /// Heals the armor by the specified amount, ensuring the armor value does not exceed the maximum.
        /// </summary>
        /// <param name="healValue">The amount to heal the armor.</param>
        /// <returns>The updated current armor value.</returns>
        public float HealArmor(float healValue)
        {
            currentArmorValue = Mathf.Clamp(currentArmorValue + healValue, 0, maxArmorValue);
            return currentArmorValue;
        }

        /// <summary>
        /// Damages the armor by the specified amount, ensuring the armor value does not go below zero.
        /// </summary>
        /// <param name="damageValue">The amount to damage the armor.</param>
        /// <returns>The updated current armor value.</returns>
        public float DamageArmor(float damageValue)
        {
            currentArmorValue = Mathf.Max(currentArmorValue - damageValue, 0);
            OnArmorDamage(damageValue, null);
            return currentArmorValue;
        }

        /// <summary>
        /// Initiates the regeneration of armor over time.
        /// </summary>
        public void RegenerationArmor()
        {
            if (Time.time > timeSinceLastDamage + armorRegenCooldownSeconds)
            {
                float healValue = armorRegenerationRate * Time.deltaTime;
                HealArmor(healValue);
            }
        }

        /// <summary>
        /// Method called during each frame update. Can be overridden in derived classes for custom behavior.
        /// </summary>
        public virtual void OnUpdate()
        {

        }

        /// <summary>
        /// Invoked when this armor instance is added to the list of active armors ("activeArmors").
        /// </summary>
        public virtual void OnArmorAdded()
        {

        }


        public virtual void OnRespawn()
        {

        }

        public ArmorBase() { SetupArmor(); }
    }
}
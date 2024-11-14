using JARL.ArmorFramework.Bases;
using JARL.ArmorFramework.Classes;
using JARL.ArmorFramework.Utlis;
using JARL.Extensions;
using ModsPlus;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnboundLib.Extensions;
using UnityEngine;

namespace JARL.ArmorFramework
{
    /// <summary>
    /// Handles armor-related functionalities for player characters.
    /// </summary>
    public class ArmorHandler : MonoBehaviour
    {
        // Armor Health Bar Fields
        private List<HealthBarObjectWithArmor> armorHeathBars = new List<HealthBarObjectWithArmor>();
        private float healthBarOffset = 0.25f;

        // Disable Fields
        public bool allArmorDisabled = false; // When true, disable all registered armors
        public int disableTimeRemainingSeconds = 0; // Remaining time in seconds for all armor to be disabled

        // Lists and Dictionaries
        public List<ArmorBase> armors = new List<ArmorBase>();
        public List<ArmorBase> activeArmors = new List<ArmorBase>();

        public int activeArmorsCount = 0;

        // The player that this "ArmorHandler" bind to
        public Player player;

        /// <summary>
        /// Dictionary storing methods for processing damage after armor reduction.
        /// Each method takes parameters representing the current armor, players involved (dealing and receiving damage),
        /// remaining damage after initial reductions, original damage amount,
        /// a flag indicating whether the damage is lethal, and a flag indicating whether to ignore blocking effects.
        /// Methods return modified damage and armor values.
        /// </summary>
        public static Dictionary<string, Func<ArmorBase, Player, Player, float, float, DamageAndArmorResult>> DamageProcessingMethodsAfter =
            new Dictionary<string, Func<ArmorBase, Player, Player, float, float, DamageAndArmorResult>>();

        /// <summary>
        /// Dictionary storing methods for processing damage before armor reduction.
        /// Each method takes parameters representing the current armor, players involved (dealing and receiving damage),
        /// remaining damage after initial reductions, original damage amount,
        /// a flag indicating whether the damage is lethal, and a flag indicating whether to ignore blocking effects.
        /// Methods return modified damage and armor values.
        /// </summary>
        public static Dictionary<string, Func<ArmorBase, Player, Player, float, float, ArmorProcessingResult>> DamageProcessingMethodsBefore =
            new Dictionary<string, Func<ArmorBase, Player, Player, float, float, ArmorProcessingResult>>();

        /// <summary>
        /// Tries to reactivate armor based on specific conditions.
        /// </summary>
        /// <param name="armor">The armor to be considered for reactivation.</param>
        public void TryReactivateArmor(ArmorBase armor)
        {
            if ((!armor.isActive && armor.reactivateArmorType == ArmorReactivateType.Second && Time.time >= armor.timeSinceLastDamage + armor.reactivateArmorValue) ||
                (!armor.isActive && armor.reactivateArmorType == ArmorReactivateType.Percent && armor.currentArmorValue / armor.maxArmorValue > armor.reactivateArmorValue))
            {
                Utils.LogInfo($"Armor '{armor.GetArmorType()}' passed all reactivation checks. Reactivating...");
                armor.isActive = true;
                armor.OnReactivate();
            }
        }

        /// <summary>
        /// Processes damage, taking armor into account, and updates relevant variables.
        /// </summary>
        /// <param name="damageVector">The damage to be processed.</param>
        /// <param name="damagingPlayer">The player dealing the damage.</param>
        /// <param name="hurtPlayer">The player receiving the damage.</param>
        public void ProcessDamage(ref Vector2 damageVector, Player damagingPlayer, Player hurtPlayer)
        {
            Utils.LogInfo("Proocessing Damage");

            float remainingDamage = damageVector.magnitude;

            foreach (ArmorBase armor in armors)
            {
                Utils.LogInfo($"Proocessing damage for '{armor.GetArmorType()}'");
                if (remainingDamage <= 0) break; // Exit the loop if incoming damage is non-positive

                // Skip to the next iteration if the armor is not active or has no max armor value
                if (!armor.isActive || armor.maxArmorValue == 0) continue;

                // Invoke additional damage processing methods before armor reduction
                Utils.LogInfo($"Runing all 'DamageProcessingMethodsBefore' method for '{armor.GetArmorType()}'");
                foreach (KeyValuePair<string, Func<ArmorBase, Player, Player, float, float, ArmorProcessingResult>> methodEntry in DamageProcessingMethodsBefore)
                {
                    string methodName = methodEntry.Key;
                    Func<ArmorBase, Player, Player, float, float, ArmorProcessingResult> method = methodEntry.Value;

                    Utils.LogInfo($"Runing '{methodName}' method for '{armor.GetArmorType()}'");
                    try
                    {
                        // Invoke the method with the provided parameters
                        ArmorProcessingResult result = method.Invoke(armor, damagingPlayer, hurtPlayer, remainingDamage, damageVector.magnitude);
                        remainingDamage = result.damage;
                        armor.currentArmorValue = result.armor;

                        if (result.skipArmorDamageProcess) continue;
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.LogError($"An error occurred while executing method '{methodName}': {ex.Message}");
                    }
                }

                // Process armor damage and update variables
                DamageAndArmorResult armorAndDamage;
                Utils.LogInfo($"Runing 'OnArmorDamage' method for '{armor.GetArmorType()}'");
                try
                {
                    // Attempt to called "OnArmorDamage" method
                    armor.OnArmorDamage(remainingDamage, damagingPlayer);

                    // Attempt to calculate the armor and damage based on the incoming damage
                    armorAndDamage = armor.OnDamage(remainingDamage, damagingPlayer);

                    // Update variables based on the armor and damage calculation
                    remainingDamage = armorAndDamage.damage;
                    armor.currentArmorValue = armorAndDamage.armor;

                    // Deactivate armor if its armor value drops below or equal to zero
                    if (armorAndDamage.armor <= 0)
                        armor.isActive = false;

                    // Update the time since the last damage for the current armor
                    armor.timeSinceLastDamage = Time.time;
                }
                catch (Exception ex)
                {
                    // Log the error and take appropriate action
                    UnityEngine.Debug.LogError($"An error occurred while executing the 'OnArmorDamage' method of '{armor.GetType().Name}': {ex.Message}");

                    // Break out of the loop to prevent further processing if an error occurs
                    break;
                }

                // Invoke additional damage processing methods after armor reduction
                Utils.LogInfo($"Runing all 'DamageProcessingMethodsAfter' method for '{armor.GetArmorType()}'");
                foreach (KeyValuePair<string, Func<ArmorBase, Player, Player, float, float, DamageAndArmorResult>> methodEntry in DamageProcessingMethodsAfter)
                {
                    string methodName = methodEntry.Key;
                    Func<ArmorBase, Player, Player, float, float, DamageAndArmorResult> method = methodEntry.Value;

                    Utils.LogInfo($"Runing '{methodName}' method for '{armor.GetArmorType()}'");
                    try
                    {
                        // Invoke the method with the provided parameters
                        DamageAndArmorResult result = method.Invoke(armor, damagingPlayer, hurtPlayer, remainingDamage, damageVector.magnitude);
                        remainingDamage = result.damage;
                        armor.currentArmorValue = result.armor;
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.LogError($"An error occurred while executing method '{methodName}': {ex.Message}");
                    }
                }
            }

            Vector2 remainingDamageDirection = damageVector.normalized * remainingDamage;
            damageVector = remainingDamageDirection;
        }

        /// <summary>
        /// Resets the armor stats by clearing the existing armors list and adding all elements from the registeredArmorTypes list.
        /// </summary>
        public void ResetArmorStats()
        {
            // Clear the existing armors list
            armors.Clear();

            // Add all elements from the registeredArmorTypes list to the armors list
            foreach (ArmorBase armor in ArmorFramework.registeredArmorTypes)
            {
                Utils.LogInfo($"Resetting stats for ArmorType: {armor.GetArmorType()}.");
                ArmorBase cloneArmor = (ArmorBase)armor.Clone();
                cloneArmor.armorHandler = this;

                armors.Add(cloneArmor);
            }

            armors = armors.OrderByDescending(armor => armor.priority).ToList();
        }

        /// <summary>
        /// Updates armor-related calculations and invokes the OnUpdate method for each active armor during each frame.
        /// </summary>
        public void Update()
        {
            float totalArmor = 0;
            float totalMaxArmor = 0;
            if (player.data.isPlaying)
            {
                activeArmors = this.GetActiveArmors();

                if (activeArmors.Count != activeArmorsCount)
                {
                    ResetArmorHealthBar();
                    activeArmorsCount = activeArmors.Count;
                }

                foreach (ArmorBase armor in armors)
                {
                    if (armor.isActive)
                    {
                        totalArmor += armor.currentArmorValue;
                    }
                    totalMaxArmor += armor.maxArmorValue;

                    player.data.GetAdditionalData().totalArmor = totalArmor;
                    player.data.GetAdditionalData().totalMaxArmor = totalMaxArmor;

                    armor.OnUpdate();

                    armor.RegenerationArmor();

                    // Attempt to reactivate armor based on specific conditions
                    TryReactivateArmor(armor);
                }

                UpdateArmorHealthBar();
            }
        }

        /// <summary>
        /// Method called upon player respawn. Restores armor values for armors that regenerate fully after respawn
        /// and invokes the OnRespawn method for each active armor. 
        /// Fully regenerates specified armors and triggers the OnRespawn event.
        /// </summary>
        public void OnRespawn()
        {
            foreach (ArmorBase armor in armors)
            {
                if (armor.regenerateFullyAfterRevive)
                {
                    Utils.LogInfo($"Regenerating ArmorType '{armor.GetArmorType()}' fully");
                    armor.currentArmorValue = armor.maxArmorValue;
                }
                armor.OnRespawn();
            }
        }

        void Start()
        {
            player = GetComponent<Player>();
        }

        private void UpdateArmorHealthBar()
        {
            foreach (HealthBarObjectWithArmor healthBarWithArmor in armorHeathBars)
            {
                CustomHealthBar armorHealthBar = healthBarWithArmor.healthBarObject.GetComponent<CustomHealthBar>();
                armorHealthBar.SetValues(healthBarWithArmor.armor.currentArmorValue, healthBarWithArmor.armor.maxArmorValue);

                if (healthBarWithArmor.armor.isActive)
                {
                    Color color = healthBarWithArmor.armor.GetBarColor().activedBarColor;
                    color.a = 1;
                    armorHealthBar.SetColor(color);
                }
                else
                {
                    Color color = healthBarWithArmor.armor.GetBarColor().deactivatedBarColor;
                    color.a = 1;
                    armorHealthBar.SetColor(color);
                }
            }
        }

        private void ResetArmorHealthBar()
        {
            Utils.LogInfo("Reseting all armor health bars");
            DestroyAllArmorHealthBar();

            float offsetMultiplier = 1;
            List<ArmorBase> @activeArmors = this.GetActiveArmors().OrderBy(armors => armors.priority).ToList();
            foreach (ArmorBase armor in @activeArmors)
            {
                AddArmorHealthBar(armor, offsetMultiplier);
                offsetMultiplier++;
            }
        }

        private void AddArmorHealthBar(ArmorBase armor, float offsetMultiplier)
        {
            GameObject healthBarWithArmor = new GameObject($"{armor.GetArmorType()} Armor Health Bar");
            healthBarWithArmor.transform.SetParent(player.GetComponentInChildren<PlayerWobblePosition>().transform);

            CustomHealthBar ArmorHealthBar = healthBarWithArmor.AddComponent<CustomHealthBar>();

            ArmorHealthBar.transform.localPosition = Vector3.up * (healthBarOffset * offsetMultiplier);
            ArmorHealthBar.transform.localScale = Vector3.one;

            armorHeathBars.Add(new HealthBarObjectWithArmor(healthBarWithArmor, armor));

            Utils.LogInfo($"Added {healthBarWithArmor.name}");
        }

        private void DestroyAllArmorHealthBar()
        {
            Utils.LogInfo("Destroying all armor health bars");

            foreach (HealthBarObjectWithArmor obj in armorHeathBars)
            {
                Utils.LogInfo($"Destroy {obj.healthBarObject.name}");
                Destroy(obj.healthBarObject);
            }

            armorHeathBars.Clear();
        }

        [PunRPC]
        public void RPCA_AddArmor(string armorType, float maxArmorValue, float regenerationRate, float regenCooldownSeconds, ArmorReactivateType reactivateArmorType, float reactivateArmorValue)
        {
            // Get the armor instance based on the specified armor type
            ArmorBase armor = ArmorUtils.GetArmorByType(this, armorType);

            // If the armor instance is null, exit the method
            if (armor == null)
            {
                Utils.LogError($"Failed to add armor. Armor type '{armorType}' not found.");
                return;
            }

            // Log the addition of armor
            Utils.LogInfo($"Adding armor '{armorType}' with max value {maxArmorValue}, regeneration rate {regenerationRate}, and reactivation value {reactivateArmorValue}");

            // Add or update the maximum armor value
            armor.maxArmorValue += Mathf.Max(maxArmorValue, 0);

            // Add or update the armor regeneration rate
            armor.armorRegenerationRate += Mathf.Max(regenerationRate, 0);

            // Update the armor regeneration cooldown if it's shorter than the provided cooldown
            if (armor.armorRegenCooldownSeconds < regenCooldownSeconds)
            {
                armor.armorRegenCooldownSeconds = regenCooldownSeconds;
            }

            // Set the reactivation armor type if it's not null
            if (reactivateArmorType != ArmorReactivateType.Null)
            {
                armor.reactivateArmorType = reactivateArmorType;
            }

            // Set the reactivation armor value
            armor.reactivateArmorValue = reactivateArmorValue;
        }


        /// <summary>
        /// Adds armor to a specific armor type within the provided ArmorHandler, incorporating custom regeneration and reactivation settings.
        /// </summary>
        /// <param name="armorType">The type of armor to be added.</param>
        /// <param name="maxArmorValue">The maximum armor value to be added.</param>
        /// <param name="regenerationRate">The armor regeneration rate.</param>
        /// <param name="regenCooldownSeconds">The cooldown for armor regeneration.</param>
        /// <param name="reactivateArmorType">The reactivation type of armor.</param>
        /// <param name="reactivateArmorValue">The reactivation value of armor.</param>
        public void AddArmor(string armorType, float maxArmorValue, float regenerationRate, float regenCooldownSeconds, ArmorReactivateType reactivateArmorType, float reactivateArmorValue)
        {
            if (PhotonNetwork.OfflineMode || PhotonNetwork.IsMasterClient)
            {
                Utils.LogInfo("Calling method 'RPCA_AddArmor' on all clients");
                player.data.view.RPC("RPCA_AddArmor", RpcTarget.All, armorType, maxArmorValue, regenerationRate, regenCooldownSeconds, reactivateArmorType, reactivateArmorValue);
            }
        }
    }
}

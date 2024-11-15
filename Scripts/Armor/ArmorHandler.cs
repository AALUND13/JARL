using JARL.Armor.Bases;
using JARL.Extensions;
using ModsPlus;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using UnboundLib.Extensions;
using UnityEngine;

namespace JARL.Armor {
    public delegate DamageArmorInfo ProcessDamageDelegate(ArmorBase armor, Player damagingPlayer, Player hurtPlayer, float remainingDamage, float originalDamage);
    public delegate ArmorProcessingResult ProcessDamageBeforeDelegate(ArmorBase armor, Player damagingPlayer, Player hurtPlayer, float remainingDamage, float originalDamage);

    public class ArmorHandler : MonoBehaviour {
        private readonly Dictionary<ArmorBase, GameObject> armorHealthBars = new Dictionary<ArmorBase, GameObject>();
        private int activeArmorsCount;

        public List<ArmorBase> Armors { get; private set; } = new List<ArmorBase>();
        public List<ArmorBase> ActiveArmors => Armors.FindAll(armor => armor.MaxArmorValue > 0);


        public static event ProcessDamageBeforeDelegate DamageProcessingMethodsBefore;
        public static event ProcessDamageDelegate DamageProcessingMethodsAfter;


        public Player Player { get; internal set; }


        public void ResetArmorStats() {
            Armors.Clear();

            foreach(ArmorBase armor in ArmorFramework.RegisteredArmorTypes) {
                LoggingUtils.LogInfo($"Resetting stats for ArmorType: {armor.GetType().Name}.");
                ArmorBase cloneArmor = (ArmorBase)Activator.CreateInstance(armor.GetType());
                cloneArmor.ArmorHandler = this;

                Armors.Add(cloneArmor);
            }

            Armors = Armors.OrderByDescending(armor => armor.Priority).ToList();
        }

        public ArmorBase GetArmorByType(Type type) {
            return Armors.Find(armor => armor.GetType() == type);
        }

        public void AddArmor(Type armorType, float maxArmorValue, float regenerationRate, float regenCooldownSeconds, ArmorReactivateType reactivateArmorType, float reactivateArmorValue) {
            if(PhotonNetwork.OfflineMode || PhotonNetwork.IsMasterClient) {
                LoggingUtils.LogInfo("Calling method 'RPCA_AddArmor' on all clients");
                Player.data.view.RPC(nameof(RPCA_AddArmor), RpcTarget.All, armorType.AssemblyQualifiedName, maxArmorValue, regenerationRate, regenCooldownSeconds, reactivateArmorType, reactivateArmorValue);
            }
        }

        private void Update() {
            float totalArmor = 0;
            float totalMaxArmor = 0;
            if(Player.data.isPlaying) {
                if(ActiveArmors.Count != activeArmorsCount) {
                    ResetArmorHealthBar();
                    activeArmorsCount = ActiveArmors.Count;
                }

                foreach(ArmorBase armor in Armors) {
                    if(armor.MaxArmorValue > 0) {
                        if(armor.IsActive) {
                            totalArmor += armor.CurrentArmorValue;
                        }
                        totalMaxArmor += armor.MaxArmorValue;

                        Player.data.GetAdditionalData().totalArmor = totalArmor;
                        Player.data.GetAdditionalData().totalMaxArmor = totalMaxArmor;

                        armor.OnUpdate();
                        armor.RegenerationArmor();

                        TryReactivateArmor(armor);
                    }
                }

                UpdateArmorHealthBar();
            }
        }

        private void Awake() {
            Player = GetComponent<Player>();
            if(!ArmorFramework.ArmorHandlers.ContainsKey(Player)) {
                ArmorFramework.ArmorHandlers.Add(Player, this);
            }
        }

        private void OnDestroy() {
            ArmorFramework.ArmorHandlers.Remove(Player);
        }


        internal void ProcessDamage(ref Vector2 damageVector, Player damagingPlayer, Player hurtPlayer, ArmorDamagePatchType armorDamagePatch) {
            LoggingUtils.LogInfo("Proocessing Damage");

            float remainingDamage = damageVector.magnitude;

            ArmorBase[] armorsOfPatch = Armors.Where(armor => armor.ArmorDamagePatch.HasFlag(armorDamagePatch)).ToArray();
            foreach(ArmorBase armor in armorsOfPatch.Reverse()) {
                LoggingUtils.LogInfo($"Proocessing damage for '{armor.GetType().Name}'");
                if(remainingDamage <= 0) break;

                if(!armor.IsActive || armor.MaxArmorValue == 0) continue;

                LoggingUtils.LogInfo($"Runing all 'DamageProcessingMethodsBefore' method for '{armor.GetType().Name}'");

                try {
                    ArmorProcessingResult? processingResult = DamageProcessingMethodsBefore?.Invoke(armor, damagingPlayer, hurtPlayer, remainingDamage, damageVector.magnitude);

                    if(processingResult.HasValue) {
                        remainingDamage = processingResult.Value.Damage;
                        armor.CurrentArmorValue = processingResult.Value.Armor;

                        if(processingResult.Value.SkipArmorDamageProcess) continue;
                    }
                } catch(Exception ex) {
                    UnityEngine.Debug.LogError($"An error occurred while executing the '{ex.TargetSite.Name}' event: {ex}");
                    break;
                }

                DamageArmorInfo armorAndDamage;
                LoggingUtils.LogInfo($"Runing 'OnArmorDamage' method for '{armor.GetType().Name}'");
                try {
                    armorAndDamage = armor.OnDamage(remainingDamage, damagingPlayer, armorDamagePatch);
                    remainingDamage = armorAndDamage.Damage;
                    armor.CurrentArmorValue = armorAndDamage.Armor;

                    if(armorAndDamage.Armor <= 0)
                        armor.IsActive = false;

                    armor.LastStateChangeTime = Time.time;
                } catch(Exception ex) {
                    UnityEngine.Debug.LogError($"An error occurred while executing the 'OnArmorDamage' method of '{armor.GetType().Name}': {ex.Message}");
                    break;
                }

                LoggingUtils.LogInfo($"Runing all 'DamageProcessingMethodsAfter' method for '{armor.GetType().Name}'");
                try {
                    DamageArmorInfo? result = DamageProcessingMethodsAfter?.Invoke(armor, damagingPlayer, hurtPlayer, remainingDamage, damageVector.magnitude);

                    if(result.HasValue) {
                        remainingDamage = result.Value.Damage;
                        armor.CurrentArmorValue = result.Value.Armor;
                    }
                } catch(Exception ex) {
                    UnityEngine.Debug.LogError($"An error occurred while executing the '{ex.TargetSite.Name}' event: {ex}");
                    break;
                }
            }

            Vector2 remainingDamageDirection = damageVector.normalized * remainingDamage;
            damageVector = remainingDamageDirection;
        }

        internal void OnRespawn() {
            foreach(ArmorBase armor in Armors) {
                if(armor.RegenerateFullyAfterRevive) {
                    LoggingUtils.LogInfo($"Regenerating ArmorType '{armor.GetType().Name}' fully");
                    armor.CurrentArmorValue = armor.MaxArmorValue;
                }
                armor.OnRespawn();
            }
        }
        private void TryReactivateArmor(ArmorBase armor) {
            bool isInactiveAndEnabled = !armor.IsActive && !armor.Disable;

            bool canReactivateByTime =
                armor.reactivateArmorType == ArmorReactivateType.Second &&
                Time.time >= armor.LastStateChangeTime + armor.reactivateArmorValue;

            bool canReactivateByPercent =
                armor.reactivateArmorType == ArmorReactivateType.Percent &&
                armor.CurrentArmorValue / armor.MaxArmorValue > armor.reactivateArmorValue;

            if(isInactiveAndEnabled && (canReactivateByTime || canReactivateByPercent)) {
                LoggingUtils.LogInfo($"Armor '{armor.GetType().Name}' passed all reactivation checks. Reactivating...");
                armor.IsActive = true;
                armor.OnReactivate();
            }
        }

        private void UpdateArmorHealthBar() {
            foreach(KeyValuePair<ArmorBase, GameObject> armorAndHealthBar in armorHealthBars) {
                CustomHealthBar armorHealthBar = armorAndHealthBar.Value.GetComponent<CustomHealthBar>();
                armorHealthBar.SetValues(armorAndHealthBar.Key.CurrentArmorValue, armorAndHealthBar.Key.MaxArmorValue);

                if(armorAndHealthBar.Key.IsActive) {
                    Color color = armorAndHealthBar.Key.GetBarColor().ActivedBarColor;
                    color.a = 1;
                    armorHealthBar.SetColor(color);
                } else {
                    Color color = armorAndHealthBar.Key.GetBarColor().DeactivatedBarColor;
                    color.a = 1;
                    armorHealthBar.SetColor(color);
                }
            }
        }

        private void ResetArmorHealthBar() {
            LoggingUtils.LogInfo("Reseting all armor health bars");
            DestroyAllArmorHealthBar();

            List<ArmorBase> activeArmors = ActiveArmors.OrderBy(armors => armors.Priority).ToList();
            foreach(ArmorBase armor in activeArmors) {
                AddArmorHealthBar(armor);
            }
        }

        private void AddArmorHealthBar(ArmorBase armor) {
            GameObject healthBarObj = new GameObject($"{armor.GetType().Name} Armor Health Bar");
            healthBarObj.transform.SetParent(Player.GetComponentInChildren<PlayerWobblePosition>().transform);

            healthBarObj.AddComponent<CustomHealthBar>();
            Player.AddStatusIndicator(healthBarObj);

            armorHealthBars.Add(armor, healthBarObj);

            LoggingUtils.LogInfo($"Added {healthBarObj.name}");
        }

        private void DestroyAllArmorHealthBar() {
            LoggingUtils.LogInfo("Destroying all armor health bars");

            foreach(KeyValuePair<ArmorBase, GameObject> obj in armorHealthBars) {
                LoggingUtils.LogInfo($"Destroy {obj.Value.name}");
                GameObject.Destroy(obj.Value);
            }

            armorHealthBars.Clear();
        }

        [PunRPC]
        private void RPCA_AddArmor(string armorType, float maxArmorValue, float regenerationRate, float regenCooldownSeconds, ArmorReactivateType reactivateArmorType, float reactivateArmorValue) {
            ArmorBase armor = GetArmorByType(Type.GetType(armorType));
            if(armor == null) {
                LoggingUtils.LogError($"Failed to add armor. Armor type '{armorType}' not found.");
                return;
            }

            LoggingUtils.LogInfo($"Adding armor '{armorType}' with max value {maxArmorValue}, regeneration rate {regenerationRate}, and reactivation value {reactivateArmorValue}");

            armor.MaxArmorValue += Mathf.Max(maxArmorValue, 0);
            armor.ArmorRegenerationRate += Mathf.Max(regenerationRate, 0);

            if(armor.ArmorRegenCooldownSeconds < regenCooldownSeconds) {
                armor.ArmorRegenCooldownSeconds = regenCooldownSeconds;
            }
#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
            if(reactivateArmorType != null) {
                armor.reactivateArmorType = reactivateArmorType;
            }
#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'

            armor.reactivateArmorValue = reactivateArmorValue;
        }
    }
}

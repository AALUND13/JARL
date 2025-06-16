using JARL.Armor.Bases;
using JARL.Armor.Bases.Builtin;
using JARL.Armor.Processors;
using JARL.Extensions;
using JARL.Utils;
using ModsPlus;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using UnboundLib.Extensions;
using UnityEngine;

namespace JARL.Armor {
    [Obsolete("Use ArmorProcessor instead. This will be removed in the future.")] public delegate DamageArmorInfo ProcessDamageDelegate(ArmorBase armor, Player damagingPlayer, Player hurtPlayer, float remainingDamage, float originalDamage);
    [Obsolete("Use ArmorProcessor instead. This will be removed in the future.")] public delegate ArmorProcessingResult ProcessDamageBeforeDelegate(ArmorBase armor, Player damagingPlayer, Player hurtPlayer, float remainingDamage, float originalDamage);

    public class ArmorHandler : MonoBehaviour {
        public IReadOnlyList<ArmorBase> ActiveArmors => armors.FindAll(armor => armor.MaxArmorValue > 0).AsReadOnly();

        [Obsolete("Use ArmorProcessor instead. This will be removed in the future.")] public static event ProcessDamageBeforeDelegate DamageProcessingMethodsBefore;
        [Obsolete("Use ArmorProcessor instead. This will be removed in the future.")] public static event ProcessDamageDelegate DamageProcessingMethodsAfter;

        public List<ArmorBase> armors = new List<ArmorBase>();
        public IReadOnlyList<ArmorBase> Armors => armors.AsReadOnly();

        public Player Player { get; private set; }

        private readonly Dictionary<ArmorBase, GameObject> armorHealthBars = new Dictionary<ArmorBase, GameObject>();
        private int activeArmorsCount;

        public void ResetArmorStats() {
            armors.Clear();

            foreach(ArmorBase armor in ArmorFramework.RegisteredArmorTypes) {
                LoggingUtils.LogInfo($"Resetting stats for ArmorType: {armor.GetType().Name}.");
                ArmorBase armorBase = Activator.CreateInstance(armor.GetType()) as ArmorBase;
                armorBase.ArmorHandler = this;
                armors.Add(armorBase);
            }

            armors = Armors.OrderByDescending(armor => armor.Priority).ToList();
        }

        public ArmorBase GetArmorByType<T>() where T : ArmorBase {
            ArmorBase armor = armors.Find(armorType => armorType.GetType() == typeof(T));
            if(armor == null) throw new InvalidOperationException($"Armor of type '{typeof(T).Name}' not found, Make sure it is registered.");
            return armor;
        }
        public ArmorBase GetArmorByType(Type type) {
            if(type == null) throw new ArgumentNullException(nameof(type));
            if(!typeof(ArmorBase).IsAssignableFrom(type)) throw new ArgumentException($"Type '{type.Name}' is not an Armor type.");

            ArmorBase armor = armors.Find(armorType => armorType.GetType() == type);
            if(armor == null) throw new InvalidOperationException($"Armor of type '{type.Name}' not found, Make sure it is registered.");
            return armor;
        }

        public void AddArmor<T>(float maxArmorValue, float regenerationRate, float regenCooldownSeconds, ArmorReactivateType reactivateArmorType, float reactivateArmorValue) where T : ArmorBase {
            if(PhotonNetwork.OfflineMode || PhotonNetwork.IsMasterClient) {
                LoggingUtils.LogInfo("Calling method 'RPCA_AddArmor' on all clients");
                Player.data.view.RPC(nameof(RPCA_AddArmor), RpcTarget.All, typeof(T).AssemblyQualifiedName, maxArmorValue, regenerationRate, regenCooldownSeconds, reactivateArmorType, reactivateArmorValue);
            }
        }

        // All methods below are private or internal

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

                        armor.OnUpdate();
                        armor.RegenerationArmor();

                        TryReactivateArmor(armor);
                    }
                }

                UpdateArmorHealthBar();
            }

            Player.data.GetAdditionalData().totalArmor = totalArmor;
            Player.data.GetAdditionalData().totalMaxArmor = totalMaxArmor;
        }

        private void Awake() {
            Player = GetComponent<Player>();
            if(!ArmorFramework.ArmorHandlers.ContainsKey(Player)) {
                ArmorFramework.ArmorHandlers.Add(Player, this);
            }
        }

        private void OnDestroy() {
            DestroyAllArmorHealthBar();
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
                float armorLastHealth = armor.CurrentArmorValue;

                #region Old Damage Processing Methods
                try {
                    ArmorProcessingResult? processingResult = DamageProcessingMethodsBefore?.Invoke(armor, damagingPlayer, hurtPlayer, remainingDamage, damageVector.magnitude);

                    if(processingResult.HasValue) {
                        remainingDamage = processingResult.Value.Damage;
                        armor.CurrentArmorValue = processingResult.Value.Armor;

                        if(processingResult.Value.SkipArmorDamageProcess) continue;
                    }
                } catch(Exception ex) {
                    UnityEngine.Debug.LogError($"An error occurred while executing the '{ex.TargetSite.Name}' error: {ex}");
                    break;
                }
                #endregion

                List<ArmorProcessor> processors = new List<ArmorProcessor>();
                foreach(Type type in ArmorFramework.armorProcessorTypes) {
                    if(type.IsSubclassOf(typeof(ArmorProcessor))) {
                        ArmorProcessor armorProcessor = (ArmorProcessor)Activator.CreateInstance(type);
                        armorProcessor.Armor = armor;
                        armorProcessor.DamagingPlayer = damagingPlayer;
                        armorProcessor.HurtPlayer = hurtPlayer;
                        armorProcessor.ArmorDamagePatchType = armorDamagePatch;

                        processors.Add(armorProcessor);
                    }
                }

                foreach(ArmorProcessor processor in processors) {
                    LoggingUtils.LogInfo($"Running armor processor '{processor.GetType().Name}' for '{armor.GetType().Name}'");
                    try {
                        remainingDamage = processor.BeforeArmorProcess(remainingDamage, damageVector.magnitude);
                    } catch(Exception ex) {
                        UnityEngine.Debug.LogError($"An error occurred while executing the '{processor.GetType().Name}' processor: {ex}");
                        break;
                    }
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

                foreach(ArmorProcessor processor in processors) {
                    LoggingUtils.LogInfo($"Running armor processor '{processor.GetType().Name}' for '{armor.GetType().Name}'");
                    try {
                        remainingDamage = processor.AfterArmorProcess(remainingDamage, damageVector.magnitude, Mathf.Max(armorLastHealth - armor.CurrentArmorValue, 0));
                    } catch(Exception ex) {
                        UnityEngine.Debug.LogError($"An error occurred while executing the '{processor.GetType().Name}' processor: {ex}");
                        break;
                    }
                }

                #region Old Damage Processing Methods
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
                #endregion
            }

            Vector2 remainingDamageDirection = damageVector.normalized * remainingDamage;
            damageVector = remainingDamageDirection;
            RefreshTotalArmor();
        }

        internal void OnRespawn() {
            foreach(ArmorBase armor in ActiveArmors) {
                if(armor.RegenerateFullyAfterRevive) {
                    LoggingUtils.LogInfo($"Regenerating ArmorType '{armor.GetType().Name}' fully");
                    armor.CurrentArmorValue = armor.MaxArmorValue;
                    armor.IsActive = true;
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

        private void RefreshTotalArmor() {
            float totalArmor = 0;
            float totalMaxArmor = 0;
            foreach(ArmorBase armor in Armors) {
                if(armor.MaxArmorValue > 0) {
                    if(armor.IsActive) {
                        totalArmor += armor.CurrentArmorValue;
                    }
                    totalMaxArmor += armor.MaxArmorValue;
                }
            }
            Player.data.GetAdditionalData().totalArmor = totalArmor;
            Player.data.GetAdditionalData().totalMaxArmor = totalMaxArmor;
        }

        [PunRPC]
        private void RPCA_AddArmor(string armorType, float maxArmorValue, float regenerationRate, float regenCooldownSeconds, ArmorReactivateType reactivateArmorType, float reactivateArmorValue) {
            ArmorBase armor = GetArmorByType(Type.GetType(armorType));
            if(armor == null) {
                LoggingUtils.LogError($"Failed to add armor. Armor type '{armorType}' not found.");
                return;
            }

            LoggingUtils.LogInfo($"Adding '{armor}' armor with max value {maxArmorValue}, regeneration rate {regenerationRate}, and reactivation value {reactivateArmorValue}");

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

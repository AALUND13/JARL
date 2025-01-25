using JARL.Armor.Utlis;
using System.Collections.Generic;
using UnityEngine;

namespace JARL.Armor.Bases {
    public abstract class ArmorBase {
        public abstract BarColor GetBarColor();

        public ArmorDamagePatchType ArmorDamagePatch = ArmorDamagePatchType.TakeDamage | ArmorDamagePatchType.TakeDamageOverTime;

        public List<string> ArmorTags = new List<string>();

        public float MaxArmorValue = 0;
        public float CurrentArmorValue = 0;

        public float ArmorRegenerationRate = 0;
        public float ArmorRegenCooldownSeconds = 0;

        public ArmorReactivateType? reactivateArmorType = ArmorReactivateType.Percent; // Type of reactivation (percentage or time interval)
        public float reactivateArmorValue = 0; // Value for armor reactivation (leave at 0 for instant reactivation)

        public bool IsActive { get; internal set; }
        public string DeactivateText = "";

        private bool disable;
        public bool Disable {
            get => disable;
            set {
                disable = value;
                if(value) {
                    IsActive = false;
                } else {
                    LastStateChangeTime = Time.time;
                    IsActive = true;
                }
            }
        }

        public float LastStateChangeTime { get; internal set; }

        public bool RegenerateFullyAfterRevive = true;


        public int Priority = 0;

        public ArmorHandler ArmorHandler { get; internal set; }

        public bool HasArmorTag(string armorTag) {
            return ArmorTags.Contains(armorTag);
        }

        public float HealArmor(float healValue) {
            CurrentArmorValue = Mathf.Clamp(CurrentArmorValue + healValue, 0, MaxArmorValue);
            return CurrentArmorValue;
        }

        public float DamageArmor(float damageValue) {
            var DamageArmorInfo = OnDamage(damageValue, null, null);
            CurrentArmorValue = DamageArmorInfo.Armor;
            if(CurrentArmorValue <= 0)
                IsActive = false;

            LastStateChangeTime = Time.time;

            return CurrentArmorValue;
        }

        internal void RegenerationArmor() {
            if(Time.time > LastStateChangeTime + ArmorRegenCooldownSeconds && !Disable) {
                float healValue = ArmorRegenerationRate * Time.deltaTime;
                HealArmor(healValue);
            }
        }

        public virtual DamageArmorInfo OnDamage(float damage, Player DamagingPlayer, ArmorDamagePatchType? armorDamagePatchType) {
            return ArmorUtils.ApplyDamage(CurrentArmorValue, damage);
        }

        public virtual void OnUpdate() { }

        public virtual void OnRespawn() { }

        public virtual void OnReactivate() { }

        public virtual void OnRegister() { }

        public override string ToString() {
            return GetType().Name.Replace("Armor", "");
        }
    }
}
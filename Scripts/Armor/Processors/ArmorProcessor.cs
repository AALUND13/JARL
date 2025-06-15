using JARL.Armor.Bases;
using System;

namespace JARL.Armor.Processors {
    public abstract class ArmorProcessor {
        protected readonly ArmorBase Armor;
        protected readonly Player DamagingPlayer;
        protected readonly Player HurtPlayer;
        protected readonly ArmorDamagePatchType ArmorDamagePatchType;

        public virtual float BeforeArmorProcess(float remaindingDamage, float originalDamage) {
            return remaindingDamage;
        }
        public virtual float AfterArmorProcess(float remaindingDamage, float originalDamage, float takenArmorDamage) {
            return remaindingDamage;
        }

        public ArmorProcessor(ArmorBase armorBase, Player damagingPlayer, Player hurtPlayer, ArmorDamagePatchType armorDamagePatchType) {
            Armor = armorBase ?? throw new ArgumentNullException(nameof(armorBase));
            DamagingPlayer = damagingPlayer;
            HurtPlayer = hurtPlayer;
            ArmorDamagePatchType = armorDamagePatchType;
        }
    }
}

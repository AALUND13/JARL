using JARL.Armor.Bases;

namespace JARL.Armor.Processors {
    public abstract class ArmorProcessor {
        public ArmorBase Armor { get; internal set; }
        public Player DamagingPlayer { get; internal set; }
        public Player HurtPlayer { get; internal set; }
        public ArmorDamagePatchType ArmorDamagePatchType { get; internal set; }

        public virtual float BeforeArmorProcess(float remaindingDamage, float originalDamage) {
            return remaindingDamage;
        }
        public virtual float AfterArmorProcess(float remaindingDamage, float originalDamage, float takenArmorDamage) {
            return remaindingDamage;
        }
    }
}

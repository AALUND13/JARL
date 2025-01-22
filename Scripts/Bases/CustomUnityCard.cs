using BepInEx;
using ClassesManagerReborn;
using ClassesManagerReborn.Util;
using ModdingUtils.Extensions;
using RarityLib.Utils;
using System.Collections.Generic;
using System.Linq;
using UnboundLib.Cards;
using UnityEngine;

namespace JARL.Bases {
    public enum CardRarity {
        FromCardInfo, // Use the rarity from the CardInfo

        Trinket,
        Common,
        Scarce,
        Uncommon,
        Exotic,
        Rare,
        Epic,
        Legendary,
        Mythical,
        Divine
    }

    public abstract class CustomUnityCard : CustomCard {
        [Header("Class Value")]
        public bool AutomatedlyCreateClass = true;
        public string OverrideClassName = "";

        [Header("Card Info")]
        public CardRarity CardRarity = CardRarity.FromCardInfo;
        public string CardRarityCustom = ""; // If you want to use a rarity that isn't in the enum, use this

        public bool CanBeReassigned = true;

        public sealed override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block) {
            cardInfo.GetAdditionalData().canBeReassigned = CanBeReassigned;
            cardInfo.rarity = GetRarity();

            OnSetupCard(cardInfo, gun, cardStats, statModifiers, block);
        }
        public sealed override void Callback() {
            CreateClassText();
            OnCallback();
        }

        public virtual void OnSetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block) { }

        public virtual void OnCallback() { }

        protected override GameObject GetCardArt() {
            return cardInfo.cardArt;
        }

        protected override string GetDescription() {
            return cardInfo.cardDestription;
        }

        protected override CardInfo.Rarity GetRarity() {
            if(CardRarity != CardRarity.FromCardInfo && CardRarityCustom.IsNullOrWhiteSpace()) {
                return RarityUtils.GetRarity(CardRarity.ToString());
            } else if(!CardRarityCustom.IsNullOrWhiteSpace()) {
                return RarityUtils.GetRarity(CardRarityCustom);
            } else {
                return cardInfo.rarity;
            }
        }

        protected override CardInfoStat[] GetStats() {
            return cardInfo.cardStats;
        }

        protected override CardThemeColor.CardThemeColorType GetTheme() {
            return cardInfo.colorTheme;
        }

        protected override string GetTitle() {
            return cardInfo.cardName;
        }

        private void CreateClassText() {
            if(AutomatedlyCreateClass) {
                List<CardInfo> subClasses = ClassesRegistry.GetClassInfos(CardType.SubClass).ToList();

                ClassObject classObject = ClassesRegistry.Get(cardInfo.sourceCard);

                // Check if the card is a class or subclass
                if(classObject != null && classObject.type != CardType.NonClassCard) {
                    CardInfo subClassCardInfo = classObject.RequiredClassesTree.FirstOrDefault()?.FirstOrDefault();
                    string className = GetClassName(classObject);

                    ClassNameMono classNameMono = gameObject.AddComponent<ClassNameMono>();
                    classNameMono.className = className;
                }
            }
        }
        private string GetClassName(ClassObject classObject) {
            CardInfo subClassCardInfo = classObject.RequiredClassesTree.FirstOrDefault()?.FirstOrDefault();

            if(subClassCardInfo != null && subClassCardInfo.GetComponent<CustomUnityCard>() != null) {
                if(!subClassCardInfo.GetComponent<CustomUnityCard>().OverrideClassName.IsNullOrWhiteSpace()) {
                    return subClassCardInfo.GetComponent<CustomUnityCard>().OverrideClassName;
                } else {
                    return subClassCardInfo.cardName;
                }
            } else if(subClassCardInfo != null) {
                return subClassCardInfo.cardName;
            } else {
                return "Class";
            }
        }


        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats) { }
    }
}
using ClassesManagerReborn;
using ClassesManagerReborn.Util;
using System.Collections.Generic;
using System.Linq;
using UnboundLib.Cards;
using UnityEngine;

namespace JARL.Bases {
    public abstract class CustomCardUnity : CustomCard {
        [Header("Class Value")]
        public bool AutomatedlyCreateClass = true;

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers) { }

        private void Start() {
            CreateClassText();
        }

        private class SetLocalPos : MonoBehaviour {
            private readonly Vector3 localpos = new Vector3(-50f, -50f, 0f);

            private void Update() {
                if(gameObject.transform.localPosition == localpos) return;
                gameObject.transform.localPosition = localpos;
                Destroy(this, 1f);
            }
        }

        protected override GameObject GetCardArt() {
            return cardInfo.cardArt;
        }

        protected override string GetDescription() {
            return cardInfo.cardDestription;
        }

        protected override CardInfo.Rarity GetRarity() {
            return cardInfo.rarity;
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

                ClassObject cardClassObject = ClassesRegistry.Get(cardInfo.sourceCard);
                LoggingUtils.LogInfo($"CardInfo: {cardInfo.name}, {cardInfo.cardName}");
                LoggingUtils.LogInfo($"IsClassObjectNull: {cardClassObject == null}");
                // Check if the card is a class or subclass
                if(cardClassObject != null && cardClassObject.type != CardType.NonClassCard) {
                    string className = cardClassObject.RequiredClassesTree.FirstOrDefault()?.FirstOrDefault()?.cardName ?? "Class";

                    if(subClasses.Any(subClassCardInfo => subClassCardInfo.name == cardInfo.name))
                        className = cardInfo.cardName;

                    ClassNameMono classNameMono = gameObject.AddComponent<ClassNameMono>();
                    classNameMono.className = className;
                }
            }
        }

        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats) { }
    }
}
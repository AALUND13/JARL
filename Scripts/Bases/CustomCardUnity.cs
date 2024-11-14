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
        private bool createdClassText = false;

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers) { }

        private void Update() {
            if(!createdClassText) {
                CreateClassText();
            }
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
                List<CardInfo> classes = ClassesRegistry.GetClassInfos(CardType.Entry).ToList();
                List<CardInfo> subClasses = ClassesRegistry.GetClassInfos(CardType.SubClass).ToList();
                List<CardInfo> classesCard = ClassesRegistry.GetClassInfos(~CardType.Entry).ToList();

                ClassObject cardClassObject = ClassesRegistry.Get(ModdingUtils.Utils.Cards.instance.GetCardWithObjectName(cardInfo.name));

                // Check if the card is a class or subclass
                if(classes.Any(classCardInfo => classCardInfo.cardName == cardInfo.cardName)
                   || (classesCard.Any(classCardInfo => classCardInfo.cardName == cardInfo.cardName) 
                   && cardClassObject.RequiredClassesTree.FirstOrDefault()?.Count() > 0)
                  ) {
                    string className = cardClassObject.RequiredClassesTree.FirstOrDefault()?.FirstOrDefault()?.cardName ?? "Class";

                    if(subClasses.Any(subClassCardInfo => subClassCardInfo.cardName == cardInfo.cardName))
                        className = cardInfo.cardName;

                    ClassNameMono classNameMono = gameObject.AddComponent<ClassNameMono>();
                    classNameMono.className = className;
                }
                createdClassText = true;
            }
        }

        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats) { }
    }
}
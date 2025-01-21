using BepInEx;
using ClassesManagerReborn;
using ClassesManagerReborn.Util;
using ModdingUtils.Extensions;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;

namespace JARL.Bases {
    public abstract class CustomCardUnity : CustomCard {
        [Header("Class Value")]
        public bool AutomatedlyCreateClass = true;
        public string OverrideClassName = "";

        [Header("Card Info")]
        public bool CanBeReassigned = true;

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block) {
            cardInfo.GetAdditionalData().canBeReassigned = CanBeReassigned;
        }

        private void Start() {
            CreateModText();
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

        private void CreateModText() {
            RectTransform[] allChildrenRecursive = gameObject.GetComponentsInChildren<RectTransform>();
            var edgeTransform = allChildrenRecursive.FirstOrDefault(obj => obj.gameObject.name == "EdgePart (2)");
            if(edgeTransform != null) {
                GameObject modNameObj = new GameObject("ModNameText");

                GameObject bottomLeftCorner = edgeTransform.gameObject;
                modNameObj.gameObject.transform.SetParent(bottomLeftCorner.transform);

                TextMeshProUGUI modText = modNameObj.gameObject.AddComponent<TextMeshProUGUI>();
                modText.text = GetModName().Sanitize();
                modNameObj.transform.localEulerAngles = new Vector3(0f, 0f, 135f);

                modNameObj.transform.localScale = Vector3.one;
                modNameObj.AddComponent<SetLocalPos>();
                modText.alignment = TextAlignmentOptions.Bottom;
                modText.alpha = 0.1f;
                modText.fontSize = 54;
            }
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

            if(subClassCardInfo != null && subClassCardInfo.GetComponent<CustomCardUnity>() != null) {
                if(!subClassCardInfo.GetComponent<CustomCardUnity>().OverrideClassName.IsNullOrWhiteSpace()) {
                    return subClassCardInfo.GetComponent<CustomCardUnity>().OverrideClassName;
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
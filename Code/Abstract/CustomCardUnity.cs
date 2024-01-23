using ClassesManagerReborn;
using ClassesManagerReborn.Util;
using System.Collections.Generic;
using System.Linq;
using UnboundLib.Cards;
using UnityEngine;

namespace JARL.Abstract
{
    public abstract class CustomCardUnity : CustomCard
    {
        [Header("Class Value")]
        public bool automatedlyCreateClass = true;

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            CreateClassText();
        }

        private class SetLocalPos : MonoBehaviour
        {
            private readonly Vector3 localpos = new Vector3(-50f, -50f, 0f);

            private void Update()
            {
                if (gameObject.transform.localPosition == localpos) return;
                gameObject.transform.localPosition = localpos;
                Destroy(this, 1f);
            }
        }

        protected override GameObject GetCardArt()
        {
            return cardInfo.cardArt;
        }

        protected override string GetDescription()
        {
            return cardInfo.cardDestription;
        }

        protected override CardInfo.Rarity GetRarity()
        {
            return cardInfo.rarity;
        }

        protected override CardInfoStat[] GetStats()
        {
            return cardInfo.cardStats;
        }

        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return cardInfo.colorTheme;
        }

        protected override string GetTitle()
        {
            return cardInfo.cardName;
        }

        /// <summary>
        /// Creates and displays a TextMeshProUGUI object representing the class name on a canvas.
        /// </summary>
        private void CreateClassText()
        {
            if (automatedlyCreateClass)
            {
                // Retrieve lists of class information for different card types
                List<CardInfo> classes = ClassesRegistry.GetClassInfos(CardType.Entry).ToList();
                List<CardInfo> subClasses = ClassesRegistry.GetClassInfos(CardType.SubClass).ToList();
                List<CardInfo> classesCard = ClassesRegistry.GetClassInfos(~CardType.Entry).ToList();

                // Get the ClassObject associated with the card
                ClassObject cardClassObject = ClassesRegistry.Get(ModdingUtils.Utils.Cards.instance.GetCardWithName(cardInfo.cardName));

                // Check if the card is a class or subclass
                if (classes.Any(classCardInfo => classCardInfo.cardName == cardInfo.cardName) || (classesCard.Any(classCardInfo => classCardInfo.cardName == cardInfo.cardName) && cardClassObject.RequiredClassesTree.FirstOrDefault()?.Count() > 0))
                {
                    // Determine the class name
                    string className = cardClassObject.RequiredClassesTree.FirstOrDefault()?.FirstOrDefault()?.cardName ?? "Class";

                    // If the card is a subclass, set the class name to the subclass name
                    if (subClasses.Any(subClassCardInfo => subClassCardInfo.cardName == cardInfo.cardName))
                        className = cardInfo.cardName;

                    // Create and display the class name text
                    ClassNameMono classNameMono = gameObject.AddComponent<ClassNameMono>();
                    classNameMono.className = className;
                }
            }
        }

        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
        }
    }

}
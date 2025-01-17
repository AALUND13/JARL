using System.Collections.Generic;
using UnboundLib.Cards;
using UnityEngine;

namespace JARL {
    internal class CardResgester : MonoBehaviour {
        public List<GameObject> Cards;
        public List<GameObject> HiddenCards;

        public static Dictionary<string, CardInfo> ModCards = new Dictionary<string, CardInfo>();

        internal void RegisterCards() {
            foreach(var Card in Cards) {
                CardInfo cardInfo = Card.GetComponent<CardInfo>();

                CustomCard.RegisterUnityCard(Card, JustAnotherRoundsLibrary.ModInitials, cardInfo.cardName, true, null);
                ModCards.Add(cardInfo.cardName, cardInfo);
            }
            foreach(var Card in HiddenCards) {
                CardInfo cardInfo = Card.GetComponent<CardInfo>();

                CustomCard.RegisterUnityCard(Card, JustAnotherRoundsLibrary.ModInitials, cardInfo.cardName, false, null);
                ModdingUtils.Utils.Cards.instance.AddHiddenCard(cardInfo);
                ModCards.Add(cardInfo.cardName, cardInfo);
            }
        }
    }
}
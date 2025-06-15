using BepInEx;  
using BepInEx.Bootstrap;
using JARL.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;

namespace JARL {
    public class CardResgester : MonoBehaviour {
        public List<GameObject> Cards;

        public static Dictionary<Type, CardResgester> CardRegistries = new Dictionary<Type, CardResgester>();
        public Dictionary<string, CardInfo> ModCards = new Dictionary<string, CardInfo>();

        private void SetupCard(CustomCard customCard) {
            if(customCard == null) return;

            customCard.cardInfo = customCard.GetComponent<CardInfo>();
            customCard.gun = customCard.GetComponent<Gun>();
            customCard.cardStats = customCard.GetComponent<ApplyCardStats>();
            customCard.statModifiers = customCard.GetComponent<CharacterStatModifiers>();
            customCard.block = customCard.gameObject.GetOrAddComponent<Block>();
            
            customCard.SetupCard(customCard.cardInfo, customCard.gun, customCard.cardStats, customCard.statModifiers, customCard.block);
        }

        public void RegisterCards<T>(string name = "") where T : BaseUnityPlugin {
            // Get the instance of the mod
            PluginInfo plugin = Chainloader.PluginInfos.FirstOrDefault(x => x.Value.Instance is T).Value;
            CardRegistries.Add(typeof(T), this);

            string modName = name == "" ? plugin.Metadata.Name : name;

            foreach(var Card in Cards) {
                CardInfo cardInfo = Card.GetComponent<CardInfo>();
                CustomUnityCard customCard = Card.GetComponent<CustomUnityCard>();
                if(cardInfo == null) {
                    UnityEngine.Debug.LogError($"[{modName}][Card] {Card.name} does not have a 'CardInfo' component");
                    continue;
                } else if(customCard == null) {
                    UnityEngine.Debug.LogError($"[{modName}][Card] {cardInfo.cardName} does not have a 'CustomUnityCard' component");
                    continue;
                }
                
                try {
                    SetupCard(customCard);
                } catch(Exception e) {
                    UnityEngine.Debug.LogError($"[{modName}][Card] {cardInfo.cardName} failed to setup the card: {e}");
                    continue;
                }
                customCard.RegisterUnityCard((registerCardInfo) => {
                    try {
                        customCard.Register(registerCardInfo);
                    } catch(Exception e) {
                        UnityEngine.Debug.LogError($"[{modName}][Card] {registerCardInfo.cardName} failed to execute the 'Register' method: {e}");
                    }
                });

                UnityEngine.Debug.Log($"[{modName}][Card] Registered Card: {cardInfo.cardName}");
                ModCards.Add(cardInfo.cardName, cardInfo);
            }

        }

        public static CardResgester GetCardResgester<T>() where T : BaseUnityPlugin {
            return GetCardResgester(typeof(T));
        }
        public static CardResgester GetCardResgester(Type type) {
            if (type == typeof(BaseUnityPlugin)) {
                return null;
            }

            if(CardRegistries.ContainsKey(type)) {
                return CardRegistries[type];
            }
            return null;
        }
    }
}
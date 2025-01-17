using ClassesManagerReborn;
using System.Collections;

namespace JARL {
    internal class ArmorPiercing : ClassHandler {
        public override IEnumerator Init() {
            ClassesRegistry.Register(CardResgester.ModCards["Armor Piercing"].GetComponent<CardInfo>(), CardType.NonClassCard, 4);
            yield break;
        }
    }
}

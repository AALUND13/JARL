using UnityEngine;

namespace JARL.Utils {
    internal class BaseGameAssets {
        public static GameObject BasePlayer = Resources.Load<GameObject>("Player");
        public static HealthBar BaseHealthBar = BasePlayer.GetComponentInChildren<HealthBar>();
    }
}

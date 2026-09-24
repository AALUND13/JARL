using JARL.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace JARL.MonoBehaviours {
    public class CustomHealthBar : MonoBehaviour {
        private const float DRAG = 25f;
        private const float SPRING = 25f;

        [Header("References")]
        public Image HPFill;
        public Image WhiteFill;

        private float currentHealth;
        public float Health { get => currentHealth; set => SetHealth(value); }
        private float currentMaxHealth;
        public float MaxHealth { get => currentMaxHealth; set => SetMaxHealth(value); }

        private float sinceDamage;
        private float hpCur;
        private float hpVel;
        private float whiteFillCur;
        private float whiteFillVel;
        private float whiteFIllTarget;


        private void Awake() {
            HealthBar healthBar = Instantiate(BaseGameAssets.BaseHealthBar.gameObject, transform).GetComponent<HealthBar>();
            HPFill = healthBar.hp;
            WhiteFill = healthBar.white;
            Destroy(healthBar);
        }

        private void Update() {
            float healthPercentage = currentHealth / currentMaxHealth;
            sinceDamage += TimeHandler.deltaTime;

            hpVel = FRILerp.Lerp(hpVel, (healthPercentage - hpCur) * SPRING, DRAG);
            whiteFillVel = FRILerp.Lerp(whiteFillVel, (whiteFIllTarget - whiteFillCur) * SPRING, DRAG);

            hpCur += hpVel * TimeHandler.deltaTime;
            whiteFillCur += whiteFillVel * TimeHandler.deltaTime;
            HPFill.fillAmount = hpCur;

            WhiteFill.fillAmount = whiteFillCur;
            if (sinceDamage > 0.5f) {
                whiteFIllTarget = healthPercentage;
            }
        }

        public void SetValues(float health, float maxHealth) {
            SetMaxHealth(maxHealth);
            SetHealth(health);
        }

        public void SetMaxHealth(float maxHealth) {
            currentMaxHealth = maxHealth;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
        }

        public void SetHealth(float health) {
            if(health < currentHealth) sinceDamage = 0f;
            currentHealth = Mathf.Clamp(health, 0, currentMaxHealth);
        }

        public void SetColor(Color color) {
            HPFill.color = color;
        }
    }
}

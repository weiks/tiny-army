using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hammerplay.TinyArmy
{

    public class HealthBar : MonoBehaviour
    {
        [SerializeField]
        private Image healthBar;

        private void OnEnable() {
            Enemy.OnEnemyDamage += OnEnemyDamageHealth;
            healthBar.fillAmount = 1;
        }

        private void OnEnemyDamageHealth(float initialHealth, float health) {
            healthBar.fillAmount = health / initialHealth;
        }

        private void OnDisable() {
            Enemy.OnEnemyDamage -= OnEnemyDamageHealth;
        }

    }
}

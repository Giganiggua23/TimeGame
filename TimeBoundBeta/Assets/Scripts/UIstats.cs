using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIstats : MonoBehaviour
{
    public PlayerMovement PM;

    [SerializeField] private TextMeshProUGUI HealthTMP;
    [SerializeField] private TextMeshProUGUI GunDamageTMP;
    [SerializeField] private TextMeshProUGUI SpeedTMP;
    [SerializeField] private TextMeshProUGUI GunReloadTMP;

    [SerializeField] private Slider slider;

    public int Speed, Health, GunDamage, GunReload;   // Ограничения  currentHealth     = maxHealth;

    void FixedUpdate()
    {
        HealthTMP.text = Health.ToString();
        GunDamageTMP.text = GunDamage.ToString();
        SpeedTMP.text = Speed.ToString();
        GunReloadTMP.text = GunReload.ToString();

        slider.maxValue = PM.maxHealth;
        slider.value = PM.currentHealth;

    }
}

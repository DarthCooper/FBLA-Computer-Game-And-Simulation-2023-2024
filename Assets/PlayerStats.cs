using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class PlayerStats : NetworkBehaviour
{
    Health health;
    public Slider healthSlider;

    public float stamina = 100;
    [HideInInspector] public float maxStamina;
    public Slider staminaSlider;
    bool buildingStamina;
    public float staminaRebuildDelay = 1f;
    float maxStaminaRebuildDelay = 1f;
    public float staminaRebuildMultiplier = 3f;

    private void Awake()
    {
        health = GetComponent<Health>();
        maxStamina = stamina;
        maxStaminaRebuildDelay = staminaRebuildDelay;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isOwned) { return; }
        if (!healthSlider)
        {
            GameObject healthSliderObject = GameObject.Find("HealthBar");
            if (healthSliderObject)
            {
                healthSlider = healthSliderObject.GetComponent<Slider>();
            }
        }
        if (!staminaSlider)
        {
            GameObject staminaSliderObject = GameObject.Find("StaminaBar");
            if (staminaSliderObject)
            {
                staminaSlider = staminaSliderObject.GetComponent<Slider>();
            }
        }
        if (healthSlider)
        {
            healthSlider.value = health.health / health.maxHealth;
        }
        if (staminaSlider)
        {
            staminaSlider.value = stamina / maxStamina;
        }

        if (staminaRebuildDelay > 0f)
        {
            staminaRebuildDelay -= Time.deltaTime;
        } else if(stamina < maxStamina)
        {
            buildingStamina = true;
        }
        if(buildingStamina && stamina < maxStamina)
        {
            stamina += Time.deltaTime * staminaRebuildMultiplier;
        }

        if(stamina > maxStamina)
        {
            stamina = maxStamina;
        }
    }

    private void OnDisable()
    {
        if (healthSlider)
        {
            healthSlider.value = 0;
        }
    }

    public void spendStamina(float staminaDrained)
    {
        stamina -= staminaDrained;
        buildingStamina = false;
        staminaRebuildDelay = maxStaminaRebuildDelay;
    }
}

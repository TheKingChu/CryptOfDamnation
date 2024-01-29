using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 
/// </summary>
public class Health : MonoBehaviour
{
    public Slider healthbarSlider;
    public TextMeshProUGUI healthbarValueText;
    public int maxHealth, currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        // sets the text
        healthbarValueText.text = currentHealth.ToString() + "/" + maxHealth.ToString();
        // sets the slider values
        healthbarSlider.value = currentHealth;
        healthbarSlider.maxValue = maxHealth;
    }
}

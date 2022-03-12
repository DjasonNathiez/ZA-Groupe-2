using UnityEngine;
using UnityEngine.UI;

public class Mannequin : MonoBehaviour
{
    public Slider healthSlider;

    private void Update()
    {
        healthSlider.maxValue = GetComponent<AIBrain>().maxHealth;
        healthSlider.value = GetComponent<AIBrain>().currentHealth;
    }
}

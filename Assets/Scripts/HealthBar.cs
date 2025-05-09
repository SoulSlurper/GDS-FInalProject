using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public void SetMaxValue(float max)
    {
        slider.maxValue = max;
    }

    public void SetValue(float value) 
    {
        slider.value = value;
    }

    public void SetActiveState(bool active)
    {
        gameObject.SetActive(active);
    }
}

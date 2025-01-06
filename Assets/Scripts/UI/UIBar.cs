using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBar : MonoBehaviour
{
    public Slider _slider;

    public void SetMaxBar(int val)
    {
        _slider.maxValue = val;
    }

    public void SetBar(int val)
    {
        _slider.value = val;
    }
}

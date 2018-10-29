using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(Slider), typeof(CanvasGroup))]
public class SelectionMeter : MonoBehaviour {

    private Slider slider;
    private CanvasGroup cg;

    public float Value { get { return slider.value; } set { slider.value = value; } }
    public float Alpha { get { return cg.alpha; } set { cg.alpha = value; } }

    private void Awake()
    {
        slider = GetComponent<Slider>();
        cg = GetComponent<CanvasGroup>();
    }
}

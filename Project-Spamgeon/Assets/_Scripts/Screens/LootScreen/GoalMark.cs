using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalMark : MonoBehaviour {

    [SerializeField] private Image image;
    [SerializeField] private Color achievedColor;
    [SerializeField] private Color unachievedColor;

    [SerializeField] private Vector3 achieveSize;
    [SerializeField] private float achieveSwellTime;
    private Coroutine cr_achieve;

    public void SetImage(Sprite sprite)
    {
        image.sprite = sprite;
    }

    public void Achieve()
    {
        CoroutineManager.BeginCoroutine(CoroutineManager.ShrinkScaleFrom(image.transform, achieveSize, Vector3.one, achieveSwellTime), ref cr_achieve, this);
        image.color = achievedColor;
    }

    public void Unachieve()
    {
        image.color = unachievedColor;
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class InputTester : MonoBehaviour {

    public Text text;
    public int count;

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    // Use this for initialization
    void Start () {
        InputGrabber.Instance.TabEvent += Instance_TabEvent;
	}

    private void Instance_TabEvent(object sender, InputGrabber.TabEventArgs e)
    {
        count++;
        text.text = count.ToString();
    }

    // Update is called once per frame
    void Update () {
		
	}
}

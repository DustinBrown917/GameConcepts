using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloorLabel : MonoBehaviour {

    private Text text;

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    // Use this for initialization
    void Start () {
        GameManager.DungeonDepthChanged += GameManager_DungeonDepthChanged;
	}

    private void GameManager_DungeonDepthChanged(object sender, GameManager.DungeonDepthChangedArgs e)
    {
        text.text = "Floor: " + GameManager.CurrentDungeonDepth.ToString();
    }
}

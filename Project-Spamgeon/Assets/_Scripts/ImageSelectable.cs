using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageSelectable : SingleButtonSelectable {

    [SerializeField] private Image image;

	public void SetImageSprite(Sprite sprite)
    {
        image.sprite = sprite;
    }
}

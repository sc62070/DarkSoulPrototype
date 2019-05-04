using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bar : MonoBehaviour {

    public RectTransform fill;

    public void SetValue(float v) {
        fill.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, v * GetComponent<RectTransform>().rect.width);
        fill.anchoredPosition = Vector2.zero;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeeingBar : MonoBehaviour
{
    [SerializeField] SpriteRenderer fillBar;
    [SerializeField] SpriteRenderer totalBar;
    float totalWidth;
    float totalHeight;

    void Start()
    {
        if (totalBar == null) fillBar = GameObject.Find("Pee Bar").GetComponent<SpriteRenderer>();
        totalWidth = totalBar.size.x;
        totalHeight = totalBar.size.y;
    }
    public void UpdatePeeingBar(float completeness, Color color)
    {
        if (completeness > 1)
        {
            completeness = 1;
            fillBar.color = color;
        }
        else
        {
            if (color != null) fillBar.color = Color.black;
        }
        fillBar.size = new Vector2(totalWidth * completeness, totalHeight);
    }
}

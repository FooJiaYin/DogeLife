using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action : MonoBehaviour
{
    public KeyCode input;
    public float score;
    Renderer renderer;
    private void Start()
    {
        renderer = gameObject.GetComponent<Renderer>();
        renderer.enabled = false;
    }
    public void enableRenderer(bool enabled)
    {
        renderer.enabled = enabled;
    }
}

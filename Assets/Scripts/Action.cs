using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Action : MonoBehaviour
{
    public enum Reaction
    {
        Lay,
        Shake,
        Bark
    }

    public Reaction reaction;
    public KeyCode input;
    public int score;
    TextMeshPro cmd;
    Renderer renderer;
    private void Start()
    {
        renderer = gameObject.GetComponent<Renderer>();
        renderer.enabled = false;
        cmd = gameObject.transform.Find("cmd").GetComponent<TextMeshPro>();
        cmd.SetText(input.ToString());
    }
    public void enableRenderer(bool enabled)
    {
        renderer.enabled = enabled;
        cmd.gameObject.SetActive(enabled);
    }
}

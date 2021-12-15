using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManController : MonoBehaviour
{
    // Start is called before the first frame update
    public int activeAction;
    public int valueGain;
    public bool active = false;
    public Action[] actions;
    public float minIdleInterval = 1;
    public float maxIdleInterval = 2;
    public float waitingInterval = 1;
    float nextTime;
    void Start()
    {
        nextTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextTime)
        {
            if (active)
            {
                unsetActiveAction();
                nextTime += Random.Range(minIdleInterval, maxIdleInterval);
            }
            else
            {
                setActiveAction();
                nextTime += waitingInterval;
            }
        }

    }
    public void setActiveAction()
    {
        active = true;
        if (actions.Length > 0)
        {
            activeAction = Random.Range(0, actions.Length);
            actions[activeAction].enableRenderer(true);
        }
    }
    public void unsetActiveAction()
    {
        active = false;
        if (actions.Length > 0)
        {
            actions[activeAction].enableRenderer(false);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (active && other.tag == "Player")
        {
            player otherplayer = other.GetComponent<player>();
            if (otherplayer.waitingMan != this)
            {
                otherplayer.waitingMan = this;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (active && other.tag == "Player")
        {
            other.GetComponent<player>().SetEmptyMan();
        }
    }
}

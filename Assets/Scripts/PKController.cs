using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class PKController : NetworkBehaviour
{
    [SerializeField] PKRankingBoard rankingDisplay;
    [SerializeField] TMP_Text statusText;
    public GameObject wall;
    public float interval;
    public float duration;
    public float sensitivity;
    float startTime;
    float nextDetectTime;
    [SyncVar(hook = nameof(setStatus))]
    bool active = false;
    bool countDown = false;
    public List<player> players;
    float[] totalVolume;
    // private static PKController _instance;

    // public static PKController Instance { get { return _instance; } }

    void Start()
    {

        // if (_instance != null && _instance != this)
        // {
        //     Destroy(this.gameObject);
        // }
        // else
        // {
        //     _instance = this;
        // }
        wall.SetActive(false);
        //RpcSetRank("Wait to start");
    }

    void Update()
    {
        if (!isServer) return;
        if (active)
        {
            if (Time.time > startTime + duration)
            {
                active = false;
                statusText.text = "preparing...";
                wall.SetActive(false);
                startTime = Time.time + interval;
                if (players.Count == 0) return;
                if (players.Count == 1)
                {
                    if (totalVolume[0] > 10)
                    {
                        players[0].SetLevel(5);
                    }
                    else
                    {
                        players[0].AddSocialValue(-1);
                    }
                }
                else
                {
                    for (int i = 0; i < players.Count; i++)
                    {
                        if (i == 0) players[i].SetLevel(5);
                        else players[i].AddSocialValue(-1);
                    }
                }
            }
            else
            {
                if (Time.time > nextDetectTime)
                {
                    nextDetectTime = Time.time + sensitivity;

                    GetRank();
                }
            }
            statusText.text = "playing " + (duration - Time.time + startTime).ToString("f2");
        }
        else
        {
            if (Time.time > startTime)
            {
                active = true;
                statusText.text = "playing " + (duration - Time.time + startTime).ToString("f2");
                wall.SetActive(true);
                startTime = Time.time;
                totalVolume = new float[players.Count];
                nextDetectTime = Time.time + sensitivity;
                countDown = false;
            }
            else
            {
                statusText.text = "preparing " + (startTime - Time.time).ToString("f2");
                if (!countDown)
                {
                    SoundManager.Instance.PlayCountDownSoundEffect();
                    countDown = true;
                }
            }
        }
    }

    void setStatus(bool oldStatus, bool newStatus)
    {
        if (newStatus)
        {
            wall.SetActive(true);
        }
        else
        {
            wall.SetActive(false);
        }
    }

    private void GetRank()
    {
        // players = players.OrderBy(p => p.volume).ToArray();
        players.Sort((p1, p2) => p1.volumeValue > p2.volumeValue ? -1 : 1);
        string rankingText = "";
        for (int i = 0; i < players.Count; i++)
        {
            totalVolume[i] += players[i].volumeValue;
            rankingText += "" + players[i].name + ": " + totalVolume[i].ToString("0.0") + " ";

        }
        if (players.Count > 0) RpcSetRank(rankingText);
    }

    [ClientRpc]
    public void RpcSetRank(string rankingText)
    {
        rankingDisplay.rankDisplay.text = rankingText;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isServer) return;
        if (other.gameObject.tag == "Player")
        {
            player p = other.gameObject.GetComponent<player>();
            if (p.Level < 4) return;
            players.Add(p);
            //rankingDisplay.gameObject.SetActive(true);
            rankingDisplay.canvasGroup.alpha = 1;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!isServer) return;
        if (other.tag == "Player")
        {
            players.Remove(other.GetComponent<player>());
            //rankingDisplay.gameObject.SetActive(false);
            rankingDisplay.canvasGroup.alpha = 0;
        }
    }
}

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
        if (active)
        {
            statusText.text = "playing " + (startTime + duration - Time.time).ToString("f2");
            if (Time.time > startTime + duration)
            {
                if (!isServer) return;
                active = false;
                startTime = Time.time + interval;
                if (players.Count == 0) return;
                if (players.Count == 1)
                {
                    if (totalVolume[0] > 15)
                    {
                        players[0].SetLevel(5);
                    }
                    else
                    {
                        players[0].AddSocialValue(-1);
                        players[0].PlayHintAnimation(0, 0, 0, -1);
                    }
                }
                else
                {
                    for (int i = 0; i < players.Count; i++)
                    {
                        if (i == 0) players[i].SetLevel(5);
                        else
                        {
                            players[i].AddSocialValue(-1);
                            players[i].PlayHintAnimation(0, 0, 0, -1);
                        }
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
        }
        else
        {
            statusText.text = "preparing " + (startTime - Time.time).ToString("f2");
            if (!isServer) return;
            if (Time.time > startTime)
            {
                active = true;
                startTime = Time.time;
                totalVolume = new float[players.Count];
                countDown = false;
            }
            else
            {
                if (!countDown)
                {
                    if (!isServer) SoundManager.Instance.PlayCountDownSoundEffect();
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
            startTime = Time.time;
        }
        else
        {
            wall.SetActive(false);
            startTime = Time.time + interval;
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

        if (other.gameObject.tag == "Player")
        {
            player p = other.gameObject.GetComponent<player>();
            if (p.Level < 4) return;
            p.ShowRankingDisplay(true);
            if (!isServer) return;
            players.Add(p);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            player p = other.gameObject.GetComponent<player>();
            p.ShowRankingDisplay(false);
            if (!isServer) return;
            players.Remove(other.GetComponent<player>());
            //rankingDisplay.gameObject.SetActive(false);
        }
    }
}

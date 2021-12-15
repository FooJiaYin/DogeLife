using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PKController : NetworkBehaviour
{
    // Start is called before the first frame update
    Text rankingDisplay;
    public GameObject wall;
    public float interval;
    public float duration;
    public float sensitivity;
    float startTime;
    float nextDetectTime;
    [SyncVar(hook = nameof(setStatus))]
    bool active = false;
    public List<player> players;
    float[] totalVolume;
 
    void Start()
    {
        rankingDisplay = GameObject.Find("RankingDisplay").GetComponent<Text>();
        wall.SetActive(false);
        RpcSetRank("Not started");
    }

    // Update is called once per frame
    void Update()
    {
        if (!isServer) return;
        if(active) {
            if (Time.time > startTime + duration) {
                active = false;
                wall.SetActive(false);
                RpcSetRank("Not started");
                startTime = Time.time + interval;
                if(players.Count <= 1) return;
                for (int i = 0; i < players.Count; i++)
                {
                    if (i < players.Count/5)
                    {
                        players[i].AddVictoryValue(-2);
                    }
                    else if (i < players.Count*2/5)
                    {
                        players[i].AddVictoryValue(-1);
                    }
                    else if (i < players.Count*3/5)
                    {
                        players[i].AddVictoryValue(0);
                    }
                    else if (i < players.Count*4/5)
                    {
                        players[i].AddVictoryValue(1);
                    }
                    else
                    {
                        players[i].AddVictoryValue(2);
                    }
                }
            } else {
                if (Time.time > nextDetectTime) {
                    nextDetectTime = Time.time + sensitivity;
                    GetRank();
                }
            }
        } else {
            if (Time.time > startTime) {
                active = true;
                wall.SetActive(true);
                startTime = Time.time;
                totalVolume = new float[players.Count];
                nextDetectTime = Time.time + sensitivity;
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
            RpcSetRank("Not started");
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
            // Debug.Log(rankingDisplay.text);
        }
        // Debug.Log(rankingText);
        RpcSetRank(rankingText);
    }

    [ClientRpc]
    public void RpcSetRank(string rankingText)
    {
        rankingDisplay = GameObject.Find("RankingDisplay").GetComponent<Text>();
        rankingDisplay.text = rankingText;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isServer) return;
        if (other.gameObject.tag == "Player")
        {
            players.Add(other.gameObject.GetComponent<player>());
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            player otherplayer = other.GetComponent<player>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!isServer) return;
        if (other.tag == "Player")
        {
            players.Remove(other.GetComponent<player>());
        }
    }
}

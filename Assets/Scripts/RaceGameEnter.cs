using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceGameEnter : MonoBehaviour
{
    GameObject Player;
    bool opened = false;
    [SerializeField] RaceScene raceScene;

    void OnTriggerEnter2D(Collider2D other)
    {

        GameObject player = GameObject.FindWithTag("Player");
        player p = null;
        if (!opened)
        {
            if (player != null) p = player.GetComponent<player>();
            //if (p.Level != 5) return;
            p.isPlaying = false;
            raceScene.OpenScene();
            opened = true;
        }
    }

    public void SetRaceSceneClosed()
    {
        opened = false;
    }
}

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

        if (other.tag == "Player")
        {
            GameObject player = other.gameObject;
            player p = null;
            if (!opened)
            {
                if (player != null) p = player.GetComponent<player>();
                if (!p.IsLocalPlayer || p.Level != 5) return;
                p.isPlaying = false;
                raceScene.OpenScene(p);
                opened = true;
            }
        }
    }

    public void SetRaceSceneClosed()
    {
        opened = false;
    }
}

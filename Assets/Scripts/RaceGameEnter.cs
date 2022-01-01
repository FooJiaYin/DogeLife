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
        //player.transform.position = player.transform.position + Vector3.one * 2;
        if (!opened)
        {
            if (player != null) player.GetComponent<player>().isPlaying = false;
            raceScene.OpenScene();
            opened = true;
        }
    }

    public void SetRaceSceneClosed()
    {
        opened = false;
    }
}

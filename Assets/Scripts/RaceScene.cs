using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceScene : MonoBehaviour
{
    [SerializeField] CanvasGroup[] UIs;
    [SerializeField] GameObject MainCamera;
    [SerializeField] GameObject RaceCamera;
    [SerializeField] RaceUIManager raceUIManager;
    [SerializeField] RaceGameEnter raceEnter;
    [SerializeField] player mainPlayer;
    public void OpenScene()
    {
        this.gameObject.SetActive(true);
        foreach (CanvasGroup ui in UIs) { ui.alpha = 0; }
        MainCamera.SetActive(false);
        RaceCamera.SetActive(true);
        raceUIManager.OpenStartPanel();
    }

    public void CloseScene()
    {
        this.gameObject.SetActive(false);
        foreach (CanvasGroup ui in UIs) { ui.alpha = 1; }
        MainCamera.SetActive(true);
        RaceCamera.SetActive(false);
        raceEnter.SetRaceSceneClosed();
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) player.GetComponent<player>().isPlaying = true;
    }
}

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
    [SerializeField] RaceGameManager raceGameManager;
    player mainPlayer;
    public void OpenScene(player p)
    {
        raceGameManager.InitLives();
        this.gameObject.SetActive(true);
        foreach (CanvasGroup ui in UIs) { ui.alpha = 0; }
        MainCamera.SetActive(false);
        RaceCamera.SetActive(true);
        raceUIManager.OpenStartPanel();
        mainPlayer = p;
    }

    public void CloseScene(bool hasWon)
    {
        this.gameObject.SetActive(false);
        foreach (CanvasGroup ui in UIs) { ui.alpha = 1; }
        MainCamera.SetActive(true);
        RaceCamera.SetActive(false);
        raceEnter.SetRaceSceneClosed();
        if (mainPlayer != null) mainPlayer.isPlaying = true;
        if (hasWon && mainPlayer != null) mainPlayer.CmdAddMonument();
        else mainPlayer.SetLevel(4);
    }
}

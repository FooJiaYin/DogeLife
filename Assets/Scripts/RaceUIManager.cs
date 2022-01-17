using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class RaceUIManager : MonoBehaviour
{
    public GameObject[] lifeObjects;
    [SerializeField] GameObject MainPanel;
    [SerializeField] GameObject StartContent;
    [SerializeField] GameObject WinContent;
    [SerializeField] GameObject GameOverContent;
    [SerializeField] GameObject TryAgainContent;
    public RacePlayer player;
    public RaceCar car;
    public RaceScene raceScene;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (StartContent.activeSelf)
            {
                StartContent.SetActive(false);
                MainPanel.SetActive(false);
                car.StartCarDriving();
                SoundManager.Instance.PlayCarStartSoundEffect();
            }
            if (WinContent.activeSelf)
            {
                WinContent.SetActive(false);
                StartContent.SetActive(true);
                raceScene.CloseScene(true);
                //TODO: add monument
            }
            if (GameOverContent.activeSelf)
            {
                GameOverContent.SetActive(false);
                StartContent.SetActive(true);
                raceScene.CloseScene(false);
                // to level 4
            }
            if (TryAgainContent.activeSelf)
            {
                player.ResetPosition();
                car.ResetPosition();
                car.StartCarDriving();
                TryAgainContent.SetActive(false);
                MainPanel.SetActive(false);
                SoundManager.Instance.PlayCarStartSoundEffect();
            }

            Time.timeScale = 1;
        }
    }

    public void OpenStartPanel()
    {
        car.StopCarDriving();
        MainPanel.SetActive(true);
        StartContent.SetActive(true);
        player.ResetPosition();
        car.ResetPosition();
    }

    public void GameOver()
    {
        car.StopCarDriving();
        MainPanel.SetActive(true);
        GameOverContent.SetActive(true);
        SoundManager.Instance.PlayGameOverSoundEffect();
    }

    public void Win()
    {
        car.StopCarDriving();
        MainPanel.SetActive(true);
        WinContent.SetActive(true);
        SoundManager.Instance.PlayWinSoundEffect();
    }

    public void TryAgain()
    {
        car.StopCarDriving();
        MainPanel.SetActive(true);
        TryAgainContent.SetActive(true);
        SoundManager.Instance.PlayGameOverSoundEffect();
    }

    public void UpdateState(int life)
    {
        for (int i = 0; i < lifeObjects.Length; i++)
        {
            if (i < life) lifeObjects[i].SetActive(true);
            else lifeObjects[i].SetActive(false);
        }
    }
}

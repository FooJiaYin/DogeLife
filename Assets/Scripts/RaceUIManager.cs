using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceUIManager : MonoBehaviour
{
    private static RaceUIManager _instance = null;
    public static RaceUIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject newManager = new GameObject();
                _instance = newManager.AddComponent<RaceUIManager>();
            }
            return _instance;
        }
    }
    public GameObject[] lifeObjects;
    [SerializeField] GameObject MainPanel;
    [SerializeField] GameObject StartContent;
    [SerializeField] GameObject WinContent;
    [SerializeField] GameObject GameOverContent;
    [SerializeField] GameObject TryAgainContent;


    void Awake()
    {
        _instance = this;
        Time.timeScale = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartContent.SetActive(false);
            WinContent.SetActive(false);
            GameOverContent.SetActive(false);
            TryAgainContent.SetActive(false);
            MainPanel.SetActive(false);
            Time.timeScale = 1;
        }
    }
    public void GameOver()
    {
        Time.timeScale = 0;
        MainPanel.SetActive(true);
        GameOverContent.SetActive(true);
    }

    public void Win()
    {
        Time.timeScale = 0;
        MainPanel.SetActive(true);
        WinContent.SetActive(true);
    }

    public void TryAgain()
    {
        Time.timeScale = 0;
        MainPanel.SetActive(true);
        TryAgainContent.SetActive(true);

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

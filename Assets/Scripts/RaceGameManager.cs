using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceGameManager : MonoBehaviour
{
    private static RaceGameManager _instance = null;
    public static RaceGameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject newManager = new GameObject();
                _instance = newManager.AddComponent<RaceGameManager>();
                Debug.Log("create game manager");
            }
            return _instance;
        }
    }
    int lives = 3;

    [SerializeField] RacePlayer player;
    [SerializeField] RaceCar car;


    void Start()
    {
        lives = 3;
        _instance = this;
    }

    public void Lose()
    {
        lives--;
        RaceUIManager.Instance.UpdateState(lives);
        CheckGameState();
    }

    void CheckGameState()
    {
        if (lives <= 0)
        {
            RaceUIManager.Instance.GameOver();
        }
        else
        {
            player.Reset();
            car.Reset();
            RaceUIManager.Instance.TryAgain();
        }
    }
}

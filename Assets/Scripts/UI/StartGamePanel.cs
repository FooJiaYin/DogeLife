using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class StartGamePanel : MonoBehaviour
{
    [SerializeField] TMP_InputField m_nameInput;
    [SerializeField] Button m_startBtn;

    PlayerScoreBoard playerScoreBoard;
    NetworkManager networkManager;
    public string PlayerName;

    void Start()
    {
        m_startBtn.onClick.AddListener(StartGame);
        playerScoreBoard = FindObjectOfType<PlayerScoreBoard>();
        networkManager = FindObjectOfType<NetworkManager>();
    }

    void StartGame()
    {
        if (m_nameInput.text == "") return;
        PlayerName = m_nameInput.text;
        Debug.Log(PlayerName);
        playerScoreBoard.SetPlayerName(PlayerName);
        networkManager.StartClient();
        this.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartGame();
        }
    }
}

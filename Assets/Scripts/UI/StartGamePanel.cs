using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartGamePanel : MonoBehaviour
{
    [SerializeField] TMP_InputField m_nameInput;
    [SerializeField] Button m_startBtn;
    public CanvasGroup canvasGroup;

    public string PlayerName;

    void Start()
    {
        m_startBtn.onClick.AddListener(StartGame);
    }

    void StartGame()
    {
        PlayerName = m_nameInput.text;
        Debug.Log(PlayerName);
        this.gameObject.SetActive(false);
    }
}

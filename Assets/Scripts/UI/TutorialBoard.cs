using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBoard : MonoBehaviour
{
    [SerializeField] GameObject TutorialPanel;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePanel();
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            OpenPanel();
        }
    }

    void ClosePanel()
    {
        TutorialPanel.SetActive(false);
    }

    void OpenPanel()
    {
        TutorialPanel.SetActive(true);

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskBoard : MonoBehaviour
{

    [SerializeField] TaskElement[] tasks;
    int level = 1;
    int currIdx = -1;

    void Start()
    {
        SetTaskWithLevel(level);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            ShowNextHintDescription();
        }
    }

    public void SetTaskWithLevel(int newLevel)
    {
        Debug.Log("Hint:" + newLevel);
        if (newLevel < 0) return;
        level = newLevel;
        for (int i = 0; i < tasks.Length; i++) tasks[i].gameObject.SetActive(tasks[i].LevelisActive[newLevel - 1]);
    }

    void ShowNextHintDescription()
    {
        if (currIdx != -1)
        {
            tasks[currIdx].Description.SetActive(false);
        }

        while (true)
        {
            currIdx = (currIdx + 2) % 6 - 1;
            if (currIdx == -1) break;
            else if (tasks[currIdx].LevelisActive[level - 1])
            {
                tasks[currIdx].Description.SetActive(true);
                break;
            }
        }
    }
}

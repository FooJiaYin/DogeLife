using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelHintBoard : MonoBehaviour
{
    const int numOfLevel = 5;
    [SerializeField] GameObject hintPanel;
    [SerializeField] Text title;
    [SerializeField] Text levelTitle;
    [SerializeField] Text content;

    // Update is called once per frame
    void Start()
    {
        hintPanel.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            hintPanel.SetActive(false);
        }
    }

    public void ShowHintBoard(int level, int levelUpOrDown)
    {
        if (level < 0 || level > numOfLevel) return;
        if (levelUpOrDown > 0)
        {
            title.text = (level == 1) ? "WELCOME" : "LEVEL UP!!";
            levelTitle.text = "Level " + level;
            content.text = GetLevelUpHint(level);
        }
        else
        {
            title.text = (level == 0) ? "GAME OVER" : "LEVEL DOWN";
            levelTitle.text = (level == 0) ? "you are died..." : "Level" + level;
            content.text = GetLevelDownHint(level);
        }
        hintPanel.SetActive(true);
    }

    string GetLevelUpHint(int level)
    {
        switch (level)
        {
            case 1:
                return "Find healthy food to eat,\nbad food makes me sick : (\nMake me full to level up!";
            case 2:
                return "If I pee at there, then it's my PLACE!\nGather 6 PLACE to level up!\nBTW... I still need food to keep alive";
            case 3:
                return "I want to play with people there!\nGet 6 LOVE from them to level up!\nNOTICE! I still need space and food!!";
            case 4:
                return "I'm the best barking dog in the world.\nI'll become GOD if I win the game!\nFood and space are still important : )";
            case 5:
                return "I LOVE car chasing!  \nIf I succeed then my statue will be put in the garden!";
            default:
                return "";
        }
    }

    string GetLevelDownHint(int level)
    {
        switch (level)
        {
            case 0:
                return "I'm died QQ I'm sick and so hungry...";
            case 1:
                return "I need more food to gain energy";
            case 2:
                return "I lost all my places... I'm not a good dog";
            case 3:
                return "I lost all my heart given by people QQ I can't PK with other dog...";
            case 4:
                return "I failed to jump onto the car, I need to try harder";
            default:
                return "";
        }
    }
}

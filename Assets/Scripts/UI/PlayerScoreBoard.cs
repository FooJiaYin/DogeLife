using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreBoard : MonoBehaviour
{
    const float FoodValueMax = 20;
    const float HealthValueMax = 20;
    const int BarMaxWidth = 150;
    const int BarMaxHeight = 12;
    public string playerName;

    [SerializeField] Image picture;
    [SerializeField] Sprite[] sprites;
    [SerializeField] Text PlayerNameDisplay;
    [SerializeField] Text foodValueDisplay;
    [SerializeField] Text healthValueDisplay;
    [SerializeField] Text levelValueDisplay;
    [SerializeField] Text placeValueDisplay;
    [SerializeField] Text socialValueDisplay;

    [SerializeField] RectTransform foodBarDisplay;
    [SerializeField] RectTransform healthBarDisplay;

    [SerializeField] Image foodBarColor;
    [SerializeField] Image healthBarColor;
    public StartGamePanel startGamePanel;

    float foodValue = 10;

    void Start()
    {
        startGamePanel = FindObjectOfType<StartGamePanel>();
    }

    public float FoodValue
    {
        get { return foodValue; }
        set
        {
            foodValue = value;
            SetFoodValue();
        }
    }

    float healthValue = 20;
    public float HealthValue
    {
        get { return healthValue; }
        set
        {
            healthValue = value;
            SetHealthValue();
        }
    }
    int placeValue = 0;
    public int PlaceValue
    {
        get { return placeValue; }
        set
        {
            placeValue = value;
            placeValueDisplay.text = placeValue.ToString();
        }
    }

    int socialValue = 0;
    public int SocialValue
    {
        get { return socialValue; }
        set
        {
            socialValue = value;
            socialValueDisplay.text = socialValue.ToString();
        }
    }

    int level = 1;
    public int Level
    {
        get { return level; }
        set
        {
            level = value;
            SetLevelValue();
        }
    }

    public void SetPlayerName(string name)
    {
        playerName = name;
        PlayerNameDisplay.text = name;
    }

    void SetFoodValue()
    {
        foodValueDisplay.text = foodValue + " / " + HealthValueMax;
        foodBarDisplay.sizeDelta = new Vector2(BarMaxWidth * foodValue / FoodValueMax, BarMaxHeight);
        if (foodValue < 3)
        {
            foodBarColor.color = Color.red;
        }
        else if (foodValue < 12)
        {
            foodBarColor.color = Color.magenta;
        }
        else
        {
            foodBarColor.color = Color.black;
        }
    }

    void SetHealthValue()
    {
        healthValueDisplay.text = healthValue + " / " + HealthValueMax;
        healthBarDisplay.sizeDelta = new Vector2(BarMaxWidth * healthValue / HealthValueMax, BarMaxHeight);
        if (healthValue < 3)
        {
            healthBarColor.color = Color.red;
        }
        else
        {
            healthBarColor.color = Color.black;
        }
    }

    void SetLevelValue()
    {
        picture.sprite = sprites[level - 1];
        levelValueDisplay.text = "LV " + level;
    }
}

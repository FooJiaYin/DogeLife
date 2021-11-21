using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class player : NetworkBehaviour
{
    int level = 1;
    float foodValue = 10;
    float socialValue = 0;
    float placeValue = 0;
    int victoryValue = 3;
    public Color placeColor = Color.yellow;
    public ManController waitingMan;
    ManController emptyMan;
    public PlaceController enteredPlace;
    PlaceController emptyPlace;
    float startPTime;
    public float movespeed = 5f;
    Rigidbody2D rigidbody2D;
    Vector2 movement;
    [Header("UI")]
    public Text foodValueDisplay;
    public Text placeValueDisplay;
    public Text socialValueDisplay;
    public Text victoryValueDisplay;
    public Text levelDisplay;

    public override void OnStartLocalPlayer()
    {
        // Camera.main.transform.SetParent(transform);
        // Camera.main.transform.localPosition = new Vector3(0, 0, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        emptyMan = GameObject.Find("Empty").GetComponent<ManController>();
        waitingMan = emptyMan;
        emptyPlace = GameObject.Find("Empty").GetComponent<PlaceController>();
        enteredPlace = emptyPlace;
        foodValueDisplay = GameObject.Find("FoodValueDisplay").GetComponent<Text>();
        placeValueDisplay = GameObject.Find("PlaceValueDisplay").GetComponent<Text>();
        socialValueDisplay = GameObject.Find("SocialValueDisplay").GetComponent<Text>();
        victoryValueDisplay = GameObject.Find("VictoryValueDisplay").GetComponent<Text>();
        levelDisplay = GameObject.Find("LevelDisplay").GetComponent<Text>();
        SetLevel(1);
    }

    // Update is called once per frame
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        if (level >= 2 && enteredPlace != emptyPlace)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                startPTime = Time.time;
                Debug.Log("start pee" + startPTime);
            }
            if (Input.GetKey(KeyCode.P) && ((Time.time - startPTime) > enteredPlace.ptime))
            {
                enteredPlace.setOwner(this);
                Debug.Log("pee done" + Time.time);
                SetEmptyPlace();
            }
        }
        if (level >= 3 && waitingMan != emptyMan)
        {
            if (Input.GetKey(waitingMan.actions[waitingMan.activeAction].input))
            {
                if (waitingMan.active)
                {
                    AddSocialValue(waitingMan.actions[waitingMan.activeAction].score);
                    waitingMan.unsetActiveAction();
                }
                SetEmptyMan();
            }
        }
    }
    private void FixedUpdate()
    {
        rigidbody2D.MovePosition(rigidbody2D.position + movement * movespeed * Time.fixedDeltaTime);
    }
    public void SetEmptyMan()
    {
        waitingMan = emptyMan;
    }
    public void SetEmptyPlace()
    {
        enteredPlace = emptyPlace;
    }

    public void AddFoodValue(float value)
    {
        if (!isLocalPlayer) return;
        foodValue += value;
        foodValueDisplay.text = this.foodValue + " ";
        if (foodValue >= 20 && level == 1)
        {
            SetLevel(2);
        }
        else if (foodValue < 20 && level > 1)
        {
            SetLevel(1);
        }
    }
    public void AddPlaceValue(float value)
    {
        if (!isLocalPlayer) return;
        placeValue += value;
        placeValueDisplay.text = placeValue + " ";
        if (placeValue >= 20 && level == 2)
        {
            SetLevel(3);
        }
        else if (placeValue < 20 && level > 2)
        {
            SetLevel(2);
        }
    }
    public void AddSocialValue(float value)
    {
        if (!isLocalPlayer) return;
        socialValue += value;
        socialValueDisplay.text = socialValue + " ";
        if (socialValue >= 20 && level == 3)
        {
            SetLevel(4);
        }
        else if (socialValue < 20 && level > 3)
        {
            SetLevel(3);
        }
    }
    public void AddVictoryValue(int value)
    {
        if (!isLocalPlayer) return;
        victoryValue += value;
        victoryValueDisplay.text = victoryValue + " ";
        if (victoryValue >= 6)
        {
            SetLevel(5);
        }
        if (victoryValue == 0 && level > 3)
        {
            SetLevel(3);
        }
    }
    public void SetLevel(int value)
    {
        if (!isLocalPlayer) return;
        level = value;
        if (level < 4)
        {
            victoryValue = 3;
            victoryValueDisplay.text = "-";
        }
        if (level < 3)
        {
            socialValue = 0;
            socialValueDisplay.text = "-";
        }
        if (level < 2)
        {
            placeValue = 0;
            placeValueDisplay.text = "-";
        }
        switch (value)
        {
            case 1:
                levelDisplay.text = "1 溫飽狗狗";
                break;
            case 2:
                levelDisplay.text = "2 安全狗狗";
                placeValueDisplay.text = "0";
                break;
            case 3:
                levelDisplay.text = "3 人氣狗狗";
                socialValueDisplay.text = "0";
                break;
            case 4:
                levelDisplay.text = "4 老大狗狗";
                victoryValueDisplay.text = "3";
                break;
            case 5:
                levelDisplay.text = "5 上帝狗狗";
                break;
        }
    }

}

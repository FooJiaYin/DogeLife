using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class player : NetworkBehaviour
{
    [SyncVar]
    public string name;
    [SyncVar(hook = nameof(updateLevel))]
    int level = 1;
    float foodValue = 10;
    float socialValue = 0;
    float placeValue = 0;
    [SyncVar(hook = nameof(setVictoryValue))]
    int victoryValue = 3;
    public uint netId;
    
    [SyncVar]
    public float volumeValue = 0;
    Volume volume;
    [SyncVar]
    public Color placeColor;
    public ManController waitingMan;
    ManController emptyMan;
    public PlaceController enteredPlace;
    PlaceController emptyPlace;
    float startPTime;
    public float movespeed = 5f;
    Rigidbody2D rigidbody2D;
    Vector2 movement;
    [Header("UI")]
    public Text volumeValueDisplay;
    public Text foodValueDisplay;
    public Text placeValueDisplay;
    public Text socialValueDisplay;
    public Text victoryValueDisplay;
    public Text levelDisplay;
    public SpriteRenderer spriteRenderer;
    public Sprite[] sprites;

    public override void OnStartLocalPlayer()
    {
        // Camera.main.transform.SetParent(transform);
        // Camera.main.transform.localPosition = new Vector3(0, 0, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        emptyMan = GameObject.Find("Empty").GetComponent<ManController>();
        waitingMan = emptyMan;
        emptyPlace = GameObject.Find("Empty").GetComponent<PlaceController>();
        enteredPlace = emptyPlace;
        volume = GameObject.Find("Volume").GetComponent<Volume>();
        volumeValueDisplay = GetComponentInChildren<Text>();
        foodValueDisplay = GameObject.Find("FoodValueDisplay").GetComponent<Text>();
        placeValueDisplay = GameObject.Find("PlaceValueDisplay").GetComponent<Text>();
        socialValueDisplay = GameObject.Find("SocialValueDisplay").GetComponent<Text>();
        victoryValueDisplay = GameObject.Find("VictoryValueDisplay").GetComponent<Text>();
        levelDisplay = GameObject.Find("LevelDisplay").GetComponent<Text>();
        string[] names = {"Tom", "Bob", "Alice", "Mikasa", "Alan", "John", "Mary", "Patrick"};
        Color[] colors = { Color.yellow, Color.green, Color.blue, Color.red, Color.cyan, Color.magenta };
        if (isLocalPlayer) { 
            name = names[Random.Range(0, names.Length)];
            placeColor = colors[Random.Range(0, colors.Length)];
            CmdSetName(name);
            CmdSetColor(placeColor);
            netId = gameObject.GetComponent<NetworkIdentity>().netId;
        }
        SetLevel(1);
        updateLevel(1, level);
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
                enteredPlace.setOwner(this, netId);
                Debug.Log("pee done" + Time.time);
                Debug.Log("NetId " + netId);
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
        SetLocalVolume();
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

    public void SetLocalVolume()
    {
        if (!isLocalPlayer) return;
        CmdSetVolume(volume.volume);
    }

    [Command(requiresAuthority = false)]
    public void CmdSetVolume(float value)
    {
        volumeValue = volume.volume;
        ChangeVolumeValue(volume.volume);
    }

    [Command(requiresAuthority = false)]
    public void CmdSetName(string name)
    {
        this.name = name;
    }

    [Command(requiresAuthority = false)]
    public void CmdSetColor(Color color)
    {
        this.placeColor = color;
    }

    public void ChangeVolumeValue(float value) 
    {
        volumeValueDisplay.text = value + "";
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
        // if (!isLocalPlayer) return;
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
    void setVictoryValue(int oldValue, int newValue)
    {
        if (!isLocalPlayer) return;
        victoryValueDisplay.text = newValue + " ";
        if (newValue >= 6)
        {
            SetLevel(5);
        }
        if (newValue == 0 && level > 3)
        {
            SetLevel(3);
        }
    }
    public void SetLevel(int value)
    {
        if (!isLocalPlayer) return;
        updateLevel(level, value);
        level = value;
        CmdSetLevel(value);
    }

    [Command(requiresAuthority = false)]
    public void CmdSetLevel(int value)
    {
        Debug.Log("Cmd set level");
        level = value;
    }

    public void updateLevel(int oldLevel, int newLevel)
    {
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
        switch (newLevel)
        {
            case 1:
                levelDisplay.text = "1 溫飽狗狗";
                spriteRenderer.sprite = sprites[0];
                break;
            case 2:
                levelDisplay.text = "2 安全狗狗";
                placeValueDisplay.text = "0";
                spriteRenderer.sprite = sprites[1];
                break;
            case 3:
                levelDisplay.text = "3 人氣狗狗";
                socialValueDisplay.text = "0";
                spriteRenderer.sprite = sprites[2];
                break;
            case 4:
                levelDisplay.text = "4 老大狗狗";
                victoryValueDisplay.text = "3";
                spriteRenderer.sprite = sprites[3];
                break;
            case 5:
                levelDisplay.text = "5 上帝狗狗";
                spriteRenderer.sprite = sprites[4];
                break;
        }
    }
}

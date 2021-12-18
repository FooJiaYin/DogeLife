using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Cinemachine;
using TMPro;

public class player : NetworkBehaviour
{
    const int FOOD_MAX = 20;
    const int HEALTH_MAX = 20;
    [SyncVar]
    public string name;
    [SyncVar(hook = nameof(updateLevel))]
    int level = 1;
    float foodValue = 10;
    float healthValue = 20;
    int placeValue = 0;
    int socialValue = 0;
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
    bool isPeeing = false;
    [SerializeField] PeeingBar peeingBar;
    public float movespeed = 5f;
    Rigidbody2D rigidbody2D;
    Vector2 movement;
    [Header("UI Score board")]
    public Text volumeValueDisplay;
    public Text victoryValueDisplay;
    public PlayerScoreBoard playerScoreBoard;
    public Text nameDisplay;

    [Header("Player Display")]
    public TextMeshPro playerNameText;
    public SpriteRenderer spriteRenderer;
    public Sprite[] sprites;
    CinemachineVirtualCamera VirtualCamera;

    [SerializeField] PlayerHintBox hintBox;
    [SerializeField] Animation hintBoxAnimation;

    public override void OnStartLocalPlayer()
    {
        GameObject vcam = GameObject.FindWithTag("Virtual Camera");
        if (vcam != null) VirtualCamera = vcam.GetComponent<CinemachineVirtualCamera>();
        VirtualCamera.Follow = this.transform;
    }

    void Start()
    {
        rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        emptyMan = GameObject.Find("EmptyManAndPlace").GetComponent<ManController>();
        waitingMan = emptyMan;
        emptyPlace = GameObject.Find("EmptyManAndPlace").GetComponent<PlaceController>();
        enteredPlace = emptyPlace;
        volume = GameObject.Find("Volume").GetComponent<Volume>();
        volumeValueDisplay = GetComponentInChildren<Text>();
        playerScoreBoard = GameObject.Find("UI - Score board").GetComponent<PlayerScoreBoard>();
        victoryValueDisplay = GameObject.Find("VictoryValue").GetComponent<Text>();
        nameDisplay = GameObject.Find("PlayerNameDisplay").GetComponent<Text>();
        if (peeingBar == null) peeingBar = GameObject.Find("Pee Bar").GetComponent<PeeingBar>();

        Color[] colors = { Color.yellow, Color.green, Color.blue, Color.red, Color.cyan, Color.magenta };
        if (isLocalPlayer)
        {
            InitPlayerName();
            placeColor = colors[Random.Range(0, colors.Length)];
            CmdSetColor(placeColor);
            netId = gameObject.GetComponent<NetworkIdentity>().netId;
        }
        SetLevel(1);
        updateLevel(1, level);
    }

    void InitPlayerName()
    {
        string[] names = { "Tom", "Bob", "Alice", "Mikasa", "Alan", "John", "Mary", "Patrick" };
        if (name == "") name = names[Random.Range(0, names.Length)];
        CmdSetName(name);
        playerNameText.text = name;
        nameDisplay.text = name;
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        if (level >= 2 && enteredPlace != emptyPlace)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                startPTime = Time.time;
                isPeeing = true;
                peeingBar.gameObject.SetActive(true);
                Debug.Log("start pee" + startPTime);

            }
            if (Input.GetKeyUp(KeyCode.P))
            {
                if (Time.time - startPTime > enteredPlace.ptime)
                {
                    enteredPlace.setOwner(this, netId);
                    Debug.Log("pee done" + Time.time);
                    Debug.Log("NetId " + netId);
                    SetEmptyPlace();
                    isPeeing = false;
                    peeingBar.gameObject.SetActive(false);
                }
                else
                {
                    isPeeing = false;
                    peeingBar.UpdatePeeingBar(0);
                    peeingBar.gameObject.SetActive(false);
                }

            }
            if (isPeeing) peeingBar.UpdatePeeingBar((Time.time - startPTime) / enteredPlace.ptime);

        }
        if (level >= 3 && waitingMan != emptyMan)
        {
            if (Input.GetKey(waitingMan.actions[waitingMan.activeAction].input))
            {
                if (waitingMan.active)
                {
                    int addScore = waitingMan.actions[waitingMan.activeAction].score;
                    AddSocialValue(addScore);
                    PlayHintAnimation(0, 0, 0, addScore);
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
        if (volume == null)
        {
            Debug.Log("volume is gone!");
            return;
        }
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
        if (foodValue > FOOD_MAX) foodValue = FOOD_MAX;
        playerScoreBoard.FoodValue = foodValue;
        if (foodValue >= 20 && level == 1)
        {
            SetLevel(2);
        }
        else if (foodValue < 10 && level > 1)
        {
            SetLevel(1);
        }
    }

    public void AddHealthValue(float value)
    {
        if (!isLocalPlayer) return;
        healthValue += value;
        if (healthValue > HEALTH_MAX) healthValue = HEALTH_MAX;
        playerScoreBoard.HealthValue = healthValue;
        if (healthValue <= 0)
        {
            Debug.Log("Game Over");
        }
    }
    public void AddPlaceValue(int value)
    {
        if (!isLocalPlayer) return;
        placeValue += value;
        playerScoreBoard.PlaceValue = placeValue;
        if (placeValue >= 5 && level == 2)
        {
            SetLevel(3);
        }
        else if (placeValue < 5 && level > 2)
        {
            SetLevel(2);
        }
    }
    public void AddSocialValue(int value)
    {
        if (!isLocalPlayer) return;
        socialValue += value;
        playerScoreBoard.SocialValue = socialValue;
        if (socialValue >= 5 && level == 3)
        {
            SetLevel(4);
        }
        else if (socialValue == 0 && level > 3)
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
            playerScoreBoard.SocialValue = socialValue;
        }
        if (level < 2)
        {
            placeValue = 0;
            playerScoreBoard.PlaceValue = placeValue;
        }

        playerScoreBoard.Level = newLevel;
        switch (newLevel)
        {
            case 1:
                spriteRenderer.sprite = sprites[0];
                break;
            case 2:
                spriteRenderer.sprite = sprites[1];
                break;
            case 3:
                // socialValueDisplay.text = "0";
                spriteRenderer.sprite = sprites[2];
                break;
            case 4:
                victoryValueDisplay.text = "3";
                spriteRenderer.sprite = sprites[3];
                break;
            case 5:
                spriteRenderer.sprite = sprites[4];
                break;
        }
    }

    public void PlayHintAnimation(int eatValue, int healthValue, int placeValue, int socialValue)
    {
        hintBox.UpdateValue(eatValue, healthValue, placeValue, socialValue);
        hintBoxAnimation.Play();
    }
}

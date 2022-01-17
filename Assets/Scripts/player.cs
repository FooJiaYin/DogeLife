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
    const int FOOD_DECREASE_TIME = 10; // every 10 seconds -1
    float timePassed = 0f;
    public bool isPlaying = true;
    [SyncVar(hook = nameof(UpdateName))]
    public string name;
    [SyncVar(hook = nameof(UpdateLevel))]
    public int level = 1;
    public int Level { get { return level; } }
    public bool IsLocalPlayer { get { return isLocalPlayer; } }
    float foodValue = 10;
    float healthValue = 20;

    // [SyncVar(hook = nameof(UpdatePlaceValue))]
    int placeValue = 0;
    int socialValue = 0;
    public uint netId;


    [SyncVar(hook = nameof(UpdateVolume))]
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
    public PlayerScoreBoard playerScoreBoard;
    public TaskBoard playerTaskBoard;
    public LevelHintBoard levelHintBoard;

    [Header("Player Display")]
    public TextMeshPro playerNameText;
    [SerializeField] GameObject volumeDisplay;
    [SerializeField] TMP_Text volumeValueDisplay;
    PKRankingBoard rankingDisplay;

    public SpriteRenderer spriteRenderer;
    public Sprite[] sprites;
    CinemachineVirtualCamera VirtualCamera;
    NetworkManager networkManager;
    public StartGamePanel startGamePanel;

    [SerializeField] PlayerHintBox hintBox;
    [SerializeField] Animation hintBoxAnimation;
    [SerializeField] Animator playerAnimator;

    public override void OnStartLocalPlayer()
    {
        GameObject vcam = GameObject.FindWithTag("Virtual Camera");
        if (vcam != null) VirtualCamera = vcam.GetComponent<CinemachineVirtualCamera>();
        VirtualCamera.Follow = this.transform;
    }

    void Start()
    {
        onSceneLoaded();

        Color[] colors = { Color.yellow, Color.green, Color.blue, Color.red, Color.cyan, Color.magenta };
        if (isLocalPlayer)
        {
            InitPlayerName();
            placeColor = colors[Random.Range(0, colors.Length)];
            CmdSetColor(placeColor);
            netId = gameObject.GetComponent<NetworkIdentity>().netId;
        }
        SetLevel(1);
        UpdateLevel(1, level);
    }
    public void onSceneLoaded()
    {
        rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        emptyMan = GameObject.Find("EmptyManAndPlace").GetComponent<ManController>();
        emptyPlace = GameObject.Find("EmptyManAndPlace").GetComponent<PlaceController>();
        waitingMan = emptyMan;
        enteredPlace = emptyPlace;
        volume = GameObject.Find("Volume").GetComponent<Volume>();
        playerScoreBoard = GameObject.Find("UI - Score board").GetComponent<PlayerScoreBoard>();
        playerTaskBoard = GameObject.Find("UI - Tasks board").GetComponent<TaskBoard>();
        levelHintBoard = GameObject.Find("UI - Level Hint").GetComponent<LevelHintBoard>();
        networkManager = FindObjectOfType<NetworkManager>();
        startGamePanel = playerScoreBoard.startGamePanel;
        rankingDisplay = FindObjectOfType<PKRankingBoard>();
    }
    void InitPlayerName()
    {
        //string[] names = { "Tom", "Bob", "Alice", "Mikasa", "Alan", "John", "Mary", "Patrick" };
        //if (name == "") name = names[Random.Range(0, names.Length)];
        name = playerScoreBoard.playerName;
        CmdSetName(name);
        playerNameText.text = name;
        playerScoreBoard.SetPlayerName(name);
    }

    void Update()
    {
        if (!isPlaying)
        {
            movement = Vector2.zero;
            return;
        }
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        if (level >= 2 && enteredPlace != emptyPlace)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                startPTime = Time.time;
                isPeeing = true;
                peeingBar.gameObject.SetActive(true);
                SoundManager.Instance.PlayPeeingSoundEffect();

            }
            if (Input.GetKeyUp(KeyCode.P))
            {
                SoundManager.Instance.StopPeeingSoundEffect();
                if (Time.time - startPTime > enteredPlace.ptime)
                {
                    enteredPlace.setOwner(this, netId);
                    SetEmptyPlace();
                    isPeeing = false;
                    peeingBar.gameObject.SetActive(false);
                    SoundManager.Instance.PlayFinishPeeingSoundEffect();
                }
                else
                {
                    isPeeing = false;
                    peeingBar.UpdatePeeingBar(0, Color.black);
                    peeingBar.gameObject.SetActive(false);
                }

            }
            if (isPeeing) peeingBar.UpdatePeeingBar((Time.time - startPTime) / enteredPlace.ptime, placeColor);

        }
        else
        {
            if (Input.GetKeyUp(KeyCode.P))
            {
                peeingBar.gameObject.SetActive(false);
            }
        }
        if (level >= 3 && waitingMan != emptyMan)
        {
            if (Input.GetKey(waitingMan.actions[waitingMan.activeAction].input))
            {
                if (waitingMan.active)
                {
                    Action action = waitingMan.actions[waitingMan.activeAction];
                    AddSocialValue(action.score);
                    switch (action.reaction)
                    {
                        case Action.Reaction.Lay:
                            playerAnimator.SetTrigger("Lay");
                            Debug.Log("Lay");
                            if (level > 3) SoundManager.Instance.PlayBarkSound2Effect();
                            break;
                        case Action.Reaction.Shake:
                            playerAnimator.SetTrigger("Shake");
                            Debug.Log("Shake");
                            if (level > 3) SoundManager.Instance.PlayBarkSound2Effect();
                            break;
                        case Action.Reaction.Bark:
                            if (level > 3) SoundManager.Instance.PlayBarkSound2Effect();
                            else SoundManager.Instance.PlayBarkSoundEffect();
                            break;

                    }
                    if (level > 3) SoundManager.Instance.PlayBarkSound2Effect();
                    PlayHintAnimation(0, 0, 0, action.score);
                    waitingMan.unsetActiveAction();
                }
                SetEmptyMan();
            }
        }
        SetLocalVolume();
        timePassed += Time.deltaTime;
        if (timePassed > FOOD_DECREASE_TIME)
        {
            timePassed = 0;
            AddFoodValue(-1);
            // AddHealthValue(-1);
        }
    }
    private void FixedUpdate()
    {
        rigidbody2D.MovePosition(rigidbody2D.position + movement * movespeed * Time.fixedDeltaTime);
        if (movement != Vector2.zero) playerAnimator.SetBool("Moving", true);
        else playerAnimator.SetBool("Moving", false);
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
        if (!isLocalPlayer || isServer) return;
        CmdSetVolume(volume.volume);
    }

    [Command(requiresAuthority = false)]
    public void CmdSetVolume(float value)
    {
        volumeValue = value;
    }

    [Command(requiresAuthority = false)]
    public void CmdSetName(string name)
    {
        this.name = name;
    }

    void UpdateName(string oldName, string newName)
    {
        playerNameText.text = newName;
        if (isLocalPlayer) playerScoreBoard.SetPlayerName(newName);
    }

    [Command(requiresAuthority = false)]
    public void CmdSetColor(Color color)
    {
        this.placeColor = color;
    }

    void UpdateVolume(float oldVolume, float newVolume)
    {
        if (level >= 4) volumeValueDisplay.text = oldVolume.ToString("f2");
        else volumeValueDisplay.text = "";
    }

    public void AddFoodValue(float value)
    {
        if (!isLocalPlayer) return;
        if (value > 0) SoundManager.Instance.PlayFoodEatenSoundEffect();
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
        else if (foodValue <= 0)
        {
            GameOver();
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
            GameOver();
        }
    }
    public void AddPlaceValue(int value)
    {
        if (!isLocalPlayer) return;
        placeValue += value;
        Debug.Log("AddPlaceValue:" + value);
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

    // [Command(requiresAuthority = false)]
    // public void CmdSetPlaceValue(int value)
    // {
    //     Debug.Log("Cmd set placeValue");
    //     placeValue = value;
    // }

    // void UpdatePlaceValue(int oldValue, int newValue)
    // {
    //     Debug.Log("UpdatePlaceValue function");
    //     if (!isLocalPlayer) return;
    //     placeValue = newValue;
    //     Debug.Log("UpdatePlaceValue" + newValue);
    //     playerScoreBoard.PlaceValue = newValue;
    //     if (placeValue >= 5 && level == 2)
    //     {
    //         SetLevel(3);
    //     }
    //     else if (placeValue < 5 && level > 2)
    //     {
    //         SetLevel(2);
    //     }
    // }


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
    public void SetLevel(int value)
    {
        if (isServer || isLocalPlayer)
        {
            UpdateLevel(level, value);
            // level = value;
        }
        else if (!isLocalPlayer) return;
        CmdSetLevel(value);
    }

    [Command(requiresAuthority = false)]
    public void CmdSetLevel(int value)
    {
        Debug.Log("Cmd set level");
        level = value;
    }

    public void UpdateLevel(int oldLevel, int newLevel)
    {
        Debug.Log("Update level " + oldLevel + " -> " + newLevel);
        playerAnimator.SetInteger("Level", newLevel);
        playerAnimator.SetTrigger("ChangeLevel");
        if (isLocalPlayer)
        {
            level = newLevel;
            if (newLevel < 3)
            {
                socialValue = 0;
                playerScoreBoard.SocialValue = socialValue;
            }
            if (newLevel < 2)
            {
                // placeValue = 0;
                playerScoreBoard.PlaceValue = 0;
                ResetPlaces();
                peeingBar.gameObject.SetActive(false);

            }
            volumeDisplay.gameObject.SetActive(newLevel == 4);
            playerTaskBoard.SetTaskWithLevel(newLevel);
            playerScoreBoard.Level = newLevel;
            if (newLevel >= oldLevel)
            {
                levelHintBoard.ShowHintBoard(newLevel, 1);
                if (newLevel != 1) SoundManager.Instance.PlayLevelUpSoundEffect();
            }
            else
            {
                levelHintBoard.ShowHintBoard(newLevel, -1);
                SoundManager.Instance.PlayGameOverSoundEffect();
            }
        }

    }

    public void PlayHintAnimation(int eatValue, int healthValue, int placeValue, int socialValue)
    {
        hintBox.UpdateValue(eatValue, healthValue, placeValue, socialValue);
        if (hintBoxAnimation.IsPlaying("HintBoxAnimation")) hintBoxAnimation.Stop();
        hintBoxAnimation.Play();
    }

    public void ShowRankingDisplay(bool isShow)
    {
        if (!isLocalPlayer) return;
        rankingDisplay.canvasGroup.alpha = isShow ? 1 : 0;
    }

    void GameOver()
    {
        if (!isLocalPlayer) return;
        ResetPlaces();
        networkManager.StopClient();
        startGamePanel.gameObject.SetActive(true);
    }

    void ResetPlaces()
    {
        // Debug.Log("ResetPlaces");
        PlaceController[] places = FindObjectsOfType<PlaceController>();
        foreach (PlaceController place in places)
        {
            // Debug.Log("place " + place.gameObject.name + " " + place.ownerId + "==" + netId);
            if (place.ownerId == netId)
            {
                place.setOwner(null, 0);
            }
        }
    }

    [Command]
    public void CmdAddMonument()
    {
        MonumentSpawner monumentSpawner = FindObjectOfType<MonumentSpawner>();
        Debug.Log("CmdAddMonument");
        monumentSpawner.SpawnMonument(name);
    }
}

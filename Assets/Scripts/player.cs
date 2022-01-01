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
    [SyncVar]
    public string name;
    [SyncVar(hook = nameof(updateLevel))]
    int level = 1;
    public int Level { get { return level; } }
    float foodValue = 10;
    float healthValue = 20;
    int placeValue = 0;
    int socialValue = 0;
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
    public PlayerScoreBoard playerScoreBoard;
    public TaskBoard playerTaskBoard;
    public LevelHintBoard levelHintBoard;

    [Header("Player Display")]
    public TextMeshPro playerNameText;
    [SerializeField] GameObject volumeDisplay;
    [SerializeField] TMP_Text volumeValueDisplay;

    public SpriteRenderer spriteRenderer;
    public Sprite[] sprites;
    CinemachineVirtualCamera VirtualCamera;

    [SerializeField] PlayerHintBox hintBox;
    [SerializeField] Animation hintBoxAnimation;
    [SerializeField] Animator playerAnimator;

    public override void OnStartLocalPlayer()
    {
        GameObject vcam = GameObject.FindWithTag("Virtual Camera");
        if (vcam != null) VirtualCamera = vcam.GetComponent<CinemachineVirtualCamera>();
        VirtualCamera.Follow = this.transform;
    }
    private static player _instance;
    public static player Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
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
        updateLevel(1, level);
    }
    public void onSceneLoaded()
    {
        Debug.Log("player setup");
        rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        emptyMan = GameObject.Find("EmptyManAndPlace").GetComponent<ManController>();
        emptyPlace = GameObject.Find("EmptyManAndPlace").GetComponent<PlaceController>();
        waitingMan = emptyMan;
        enteredPlace = emptyPlace;
        volume = GameObject.Find("Volume").GetComponent<Volume>();
        playerScoreBoard = GameObject.Find("UI - Score board").GetComponent<PlayerScoreBoard>();
        Debug.Log(playerScoreBoard.name);
        playerTaskBoard = GameObject.Find("UI - Tasks board").GetComponent<TaskBoard>();
        levelHintBoard = GameObject.Find("UI - Level Hint").GetComponent<LevelHintBoard>();

    }
    void InitPlayerName()
    {
        string[] names = { "Tom", "Bob", "Alice", "Mikasa", "Alan", "John", "Mary", "Patrick" };
        if (name == "") name = names[Random.Range(0, names.Length)];
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
                    Debug.Log("pee done" + Time.time);
                    Debug.Log("NetId " + netId);
                    SetEmptyPlace();
                    isPeeing = false;
                    peeingBar.gameObject.SetActive(false);
                    SoundManager.Instance.PlayFinishPeeingSoundEffect();
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
                    Action action = waitingMan.actions[waitingMan.activeAction];
                    AddSocialValue(action.score);
                    switch (action.reaction)
                    {
                        case Action.Reaction.Lay:
                            playerAnimator.SetTrigger("Lay");
                            Debug.Log("Lay");
                            break;
                        case Action.Reaction.Shake:
                            playerAnimator.SetTrigger("Shake");
                            Debug.Log("Shake");
                            break;
                        case Action.Reaction.Bark:
                            SoundManager.Instance.PlayBarkSoundEffect();
                            break;

                    }
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
        volumeValueDisplay.text = value.ToString("f2");
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
        else if (foodValue <= 0)
        {
            Debug.Log("Game Over");
            //TODO: Game Over
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
            //TODO: Game Over
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
        playerAnimator.SetInteger("Level", value);
        playerAnimator.SetTrigger("ChangeLevel");
    }

    public void updateLevel(int oldLevel, int newLevel)
    {
        if (level < 4)
        {
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

        //playerAnimator.SetInteger("Level", newLevel);
        playerScoreBoard.Level = newLevel;
        playerTaskBoard.SetTaskWithLevel(newLevel);
        spriteRenderer.sprite = sprites[newLevel - 1];
        volumeDisplay.gameObject.SetActive(newLevel == 4);
        if (newLevel >= oldLevel)
        {
            levelHintBoard.ShowHintBoard(newLevel, 1);
            if (newLevel != 1) SoundManager.Instance.PlayLevelUpSoundEffect();
        }
        else levelHintBoard.ShowHintBoard(newLevel, -1);
    }

    public void PlayHintAnimation(int eatValue, int healthValue, int placeValue, int socialValue)
    {
        hintBox.UpdateValue(eatValue, healthValue, placeValue, socialValue);
        if (hintBoxAnimation.IsPlaying("HintBoxAnimation")) hintBoxAnimation.Stop();
        hintBoxAnimation.Play();
    }
}

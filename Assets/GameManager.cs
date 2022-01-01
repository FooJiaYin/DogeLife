using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class GameManager : NetworkBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }
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
    // 以後再寫Inspector
    public GameObject food;
    public ManController mans;
    public GameObject PKArea;
    float[] nextFoodSwpanTimes;
    float[] nextManSwpanTimes;
    foodSpawner[] foodSpawners;
    manSpawner[] manSpawners;
    // Start is called before the first frame update
    void getSpawners()
    {
        foodSpawners = FindObjectsOfType<foodSpawner>();
    }

    // [ClientRpc]
    void SpawnFood(GameObject food, int foodTypeIndex, int foodSpawnerIndex)
    {
        var newFood = foodSpawners[foodSpawnerIndex].SpawnFood(food, foodTypeIndex);
        // NetworkClient.RegisterPrefab(newFood);
        if (newFood != null) NetworkServer.Spawn(newFood);
    }
    void SpawnMan(GameObject man)
    {
        var manSpawnerIndex = Random.Range(0, manSpawners.Length);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void Start()
    {
        _instance = this;
        setup();
        NetworkServer.Spawn(PKArea);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        setup();
    }

    public void setup()
    {
        Debug.Log("Setup");
        getSpawners();
        int numOfFood = (int)FoodType.NumOfFood;
        nextFoodSwpanTimes = new float[numOfFood];
        for (int i = 0; i < numOfFood; i++)
        {
            nextFoodSwpanTimes[i] = Time.time;
        }
    }


    // Update is called once per frame
    void Update()
    {
        // Spawn only at the server
        if (!isServer) return;

        for (int i = 0; i < (int)FoodType.NumOfFood; i++)
        {
            if (Time.time > nextFoodSwpanTimes[i])
            {
                var foodSpawnerIndex = Random.Range(0, foodSpawners.Length);
                SpawnFood(food, i, foodSpawnerIndex);
                nextFoodSwpanTimes[i] += food.GetComponent<Food>().foodSpawnInterval;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
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
        // Debug.Log(newFood.transform.position);
        // NetworkClient.RegisterPrefab(newFood);
        if (newFood != null) NetworkServer.Spawn(newFood);
    }
    void SpawnMan(GameObject man)
    {
        var manSpawnerIndex = Random.Range(0, manSpawners.Length);
    }
    void Start()
    {
        Instance = this;
        getSpawners();
        int numOfFood = (int)FoodType.NumOfFood;
        nextFoodSwpanTimes = new float[numOfFood];
        for (int i = 0; i < numOfFood; i++)
        {
            nextFoodSwpanTimes[i] = Time.time;
            //Debug.Log(nextFoodSwpanTimes[i]);
        }
        if (isServer) NetworkServer.Spawn(PKArea);
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

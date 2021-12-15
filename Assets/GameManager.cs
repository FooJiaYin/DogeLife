using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    // 以後再寫Inspector
    public GameObject[] foods;
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
    void SpawnFood(GameObject food, int foodSpawnerIndex)
    {
        var newFood = foodSpawners[foodSpawnerIndex].SpawnFood(food);
        // Debug.Log(newFood.transform.position);
        // NetworkClient.RegisterPrefab(newFood);
        NetworkServer.Spawn(newFood);
    }
    void SpawnMan(GameObject man)
    {
        var manSpawnerIndex = Random.Range(0, manSpawners.Length);
    }
    void Start()
    {
        Instance = this;
        getSpawners();
        nextFoodSwpanTimes = new float[foods.Length];
        for (int i = 0; i < foods.Length; i++)
        {
            nextFoodSwpanTimes[i] = Time.time;
            Debug.Log(nextFoodSwpanTimes[i]);
        }
        if(isServer) NetworkServer.Spawn(PKArea);
    }


    // Update is called once per frame
    void Update()
   {
        // Spawn only at the server
        if (!isServer) return;

        for (int i = 0; i < foods.Length; i++)
        {
            if (Time.time > nextFoodSwpanTimes[i])
            {
                var foodSpawnerIndex = Random.Range(0, foodSpawners.Length);
                SpawnFood(foods[i], foodSpawnerIndex);
                nextFoodSwpanTimes[i] += foods[i].GetComponent<food>().foodSpawnInterval;
            }
        }
    }
}

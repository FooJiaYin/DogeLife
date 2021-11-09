using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    // 以後再寫Inspector
    public GameObject[] foods;
    public ManController mans;
    float[] nextFoodSwpanTimes;
    float[] nextManSwpanTimes;
    foodSpawner[] foodSpawners;
    manSpawner[] manSpawners;
    // Start is called before the first frame update
    void getSpawners()
    {
        foodSpawners = FindObjectsOfType<foodSpawner>();
    }

    void SpawnFood(GameObject food)
    {
        var foodSpawnerIndex = Random.Range(0, foodSpawners.Length);
        foodSpawners[foodSpawnerIndex].SpawnFood(food);
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
    }


    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < foods.Length; i++)
        {
            if (Time.time > nextFoodSwpanTimes[i])
            {
                SpawnFood(foods[i]);
                nextFoodSwpanTimes[i] += foods[i].GetComponent<food>().foodSpawnInterval;
            }
        }
    }
}

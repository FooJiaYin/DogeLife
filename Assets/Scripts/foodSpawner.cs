using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class foodSpawner : MonoBehaviour
{
    public void SpawnFood(GameObject food)
    {
        Instantiate(food, this.transform);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

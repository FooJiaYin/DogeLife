using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class foodSpawner : MonoBehaviour
{
    public GameObject SpawnFood(GameObject food)
    {
        return Instantiate(food, this.transform.position, this.transform.rotation, this.transform);
    }
}

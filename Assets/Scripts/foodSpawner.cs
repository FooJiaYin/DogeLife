using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class foodSpawner : MonoBehaviour
{
    GameObject m_food;

    public GameObject SpawnFood(GameObject food)
    {
        if (m_food == null)
        {
            m_food = Instantiate(food, this.transform.position, this.transform.rotation, this.transform);
            return m_food;
        }
        else
        {
            return null;
        }
    }
}

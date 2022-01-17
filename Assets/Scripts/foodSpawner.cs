using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class foodSpawner : MonoBehaviour
{
    GameObject m_food;
    bool hasPlayer = false;

    public GameObject SpawnFood(GameObject food, int foodTypeIndex)
    {
        if (m_food == null && !hasPlayer)
        {
            m_food = Instantiate(food, this.transform.position, this.transform.rotation);
            m_food.GetComponent<Food>().SetFoodType(foodTypeIndex);
            Debug.Log("SpawnFood");
            return m_food;
        }
        else
        {
            return null;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            hasPlayer = true;

        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            hasPlayer = false;
        }
    }
}

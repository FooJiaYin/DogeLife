using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceEndLine : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Car")
        {
            RaceCar car = other.gameObject.GetComponent<RaceCar>();
            car.movement = Vector2.zero;
        }
        else if (other.tag == "Player")
        {
            Time.timeScale = 0;
            Debug.Log("stop");
        }

    }
}

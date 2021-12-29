using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceEndLine : MonoBehaviour
{
    [SerializeField] RaceGameManager gameManager;
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
            car.curr_movement = Vector2.zero;
            RaceGameManager.Instance.Lose();

        }
    }
}

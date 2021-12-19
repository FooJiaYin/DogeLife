using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceCar : MonoBehaviour
{
    [SerializeField] Rigidbody2D rigidbody2D;
    public Vector2 Position { get { return rigidbody2D.position; } }
    public Vector2 movement = new Vector2(5f, 0);
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        rigidbody2D.MovePosition(rigidbody2D.position + movement * Time.fixedDeltaTime);
    }

    void OnTriggerStay2D(Collider2D other)
    {

        RacePlayer player = other.gameObject.GetComponent<RacePlayer>();
        if (player == null) return;
        if (player.Position.y >= Position.y)
        {
            //TODO: WIN
            Debug.Log("Win");
        }
    }
}

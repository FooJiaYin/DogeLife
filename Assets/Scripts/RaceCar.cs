using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceCar : MonoBehaviour
{
    [SerializeField] Rigidbody2D rigidbody2D;
    public Vector2 Position { get { return rigidbody2D.position; } }
    public Vector2 movement;
    public Vector2 curr_movement;
    Vector2 startPos;

    void Start()
    {
        startPos.x = rigidbody2D.position.x;
        startPos.y = rigidbody2D.position.y;
        curr_movement = movement;

    }
    private void FixedUpdate()
    {
        rigidbody2D.MovePosition(rigidbody2D.position + curr_movement * Time.fixedDeltaTime);
    }

    void OnTriggerStay2D(Collider2D other)
    {

        RacePlayer player = other.gameObject.GetComponent<RacePlayer>();
        if (player == null) return;
        if (player.Position.y >= Position.y)
        {
            Debug.Log("Win");
            RaceGameManager.Instance.Win();
        }
    }

    public void Reset()
    {
        rigidbody2D.position = startPos;
        curr_movement = movement;
    }
}

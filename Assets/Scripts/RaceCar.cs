using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceCar : MonoBehaviour
{
    [SerializeField] Rigidbody2D rigidbody2D;
    public Vector2 Position { get { return rigidbody2D.position; } }
    public Vector2 movement;
    public Vector2 curr_movement = Vector2.zero;
    public Vector2 startPos;

    void Start()
    {
        StopCarDriving();
    }
    private void FixedUpdate()
    {
        rigidbody2D.MovePosition(rigidbody2D.position + curr_movement * Time.fixedDeltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        RacePlayer player = other.gameObject.GetComponent<RacePlayer>();
        if (player == null) return;
        if (player.Position.y >= Position.y)
        {
            RaceGameManager.Instance.Win();
        }
    }

    public void ResetPosition()
    {
        rigidbody2D.position = startPos;
        Debug.Log("Reset car" + rigidbody2D.position);

    }

    public void StartCarDriving()
    {
        curr_movement = movement;
    }
    public void StopCarDriving()
    {
        curr_movement = Vector2.zero;
    }
}

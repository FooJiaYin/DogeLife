using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacePlayer : MonoBehaviour
{
    public enum runStatus
    {
        keyK, keyL, Empty
    }

    int lives = 3;
    public int Lives { get { return lives; } }
    runStatus status = runStatus.Empty;
    Vector2 movement = Vector2.zero;
    [SerializeField] Rigidbody2D rigidbody2D;

    public Vector2 Position { get { return rigidbody2D.position; } }
    Vector2 startPos;

    void Start()
    {
        startPos.x = rigidbody2D.position.x;
        startPos.y = rigidbody2D.position.y;
        Debug.Log("player" + startPos);

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) Jump();
        else if (Input.GetKeyDown(KeyCode.K)) UpdateRunStatus(runStatus.keyK);
        else if (Input.GetKeyDown(KeyCode.L)) UpdateRunStatus(runStatus.keyL);

    }

    void UpdateRunStatus(runStatus newStatus)
    {
        if (newStatus != status) movement += new Vector2(8f, 0);
        status = newStatus;
    }

    private void FixedUpdate()
    {
        Vector2 newPos = rigidbody2D.position + movement * Time.fixedDeltaTime;
        if (newPos.y > 1.5) newPos.y = 1.5f;
        rigidbody2D.MovePosition(newPos);
        movement -= new Vector2(2f, 1f);
        if (movement.x < 0) movement.x = 0;
        if (movement.y < 0) movement.y = 0;

    }

    private void Jump()
    {
        movement += new Vector2(3f, 15f);
        if (movement.y > 15) movement.y = 15f;
    }

    public void Reset()
    {
        rigidbody2D.position = startPos;
    }
}

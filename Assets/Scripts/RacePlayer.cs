using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacePlayer : MonoBehaviour
{
    public enum runStatus
    {
        keyR, keyN, Empty
    }

    int lives = 3;
    public int Lives { get { return lives; } }
    runStatus status = runStatus.Empty;
    Vector2 movement = Vector2.zero;
    [SerializeField] Rigidbody2D rigidbody2D;

    public Vector2 Position { get { return rigidbody2D.position; } }
    public Vector2 startPos;
    [SerializeField] Animator playerAnimator;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) Jump();
        else if (Input.GetKeyDown(KeyCode.R))
        {
            UpdateRunStatus(runStatus.keyR);
            playerAnimator.SetBool("Walking", true);
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            UpdateRunStatus(runStatus.keyN);
            playerAnimator.SetBool("Walking", false);
        }
    }

    void UpdateRunStatus(runStatus newStatus)
    {
        if (newStatus != status) movement += new Vector2(8f, 0);
        status = newStatus;
    }

    private void FixedUpdate()
    {
        Vector2 newPos = rigidbody2D.position + movement * Time.fixedDeltaTime;
        if (newPos.y > startPos.y + 1.5f) newPos.y = startPos.y + 1.5f;
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

    public void ResetPosition()
    {
        rigidbody2D.position = startPos;
        Debug.Log("Reset player" + startPos);
    }
}

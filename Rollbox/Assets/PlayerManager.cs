using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager main;

    Rigidbody2D rb;
    Vector3 initPos;

    [Tooltip("Max speed the player can be moving on level complete platform to win.")]
    public float maxSpeedToCompleteLevel;

    private void Awake()
    {
        main = this;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initPos = rb.position;
    }

    public void ResetPlayer()
    {
        rb.position = initPos;
        rb.angularVelocity = 0;
        rb.velocity = Vector3.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10)
        {
            if (CurrentLevelManager.main != null)
            {
                CurrentLevelManager.main.mustReset = true;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 11)
        {
            if (rb.velocity.magnitude < maxSpeedToCompleteLevel)
            {
                if (CurrentLevelManager.main != null)
                {
                    CurrentLevelManager.main.levelComplete = true;
                }
            }
        }
    }
}

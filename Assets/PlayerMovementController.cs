using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;

public class PlayerMovementController : NetworkBehaviour
{
    Vector2 movement = Vector2.zero;
    Vector2 lastMovement = Vector2.zero;
    Rigidbody2D rb;
    Animator animator;

    public float speed = 1f;
    public GameObject PlayerModel;

    public bool canMove = true;

    public bool beingKnockedBack;

    public bool desiredDash;

    public float dashTime = 0.5f;
    public float dashSpeed;
    bool canDash = true;

    public float TimeBetweenDashes = 1f;

    private void Awake()
    {
        PlayerModel.SetActive(false);
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if(SceneManager.GetActiveScene().name == "Game")
        {
            if(PlayerModel.activeSelf == false)
            {
                SetSpawnPosition();
                PlayerModel.SetActive(true);
            }

            if(isOwned)
            {
                if(desiredDash)
                {
                    Dash();
                }
                Movement();
            }
        }
    }

    public void SetSpawnPosition()
    {
        transform.position = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), 0);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if(isOwned)
        {
            movement = context.ReadValue<Vector2>().normalized;
            if(movement != Vector2.zero )
            {
                lastMovement = movement;
            }
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if(canDash)
        {
            desiredDash |= context.ReadValueAsButton();
        }
    }

    public void PlayAnim()
    {
        animator.SetFloat("X", movement.x);
        animator.SetFloat("Y", movement.y);
    }

    public void Movement()
    {
        if(canMove)
        {
            if(movement != Vector2.zero)
            {
                PlayAnim();
            }
            rb.velocity = movement * speed * Time.fixedDeltaTime;
        }
    }

    public void Dash()
    {
        desiredDash = false;
        canDash = false;
        GetComponent<Health>().TakeKnockback(dashTime, lastMovement * dashSpeed);
        Invoke("ReEnableCanDash", TimeBetweenDashes);
    }

    public void ReEnableCanDash()
    {
        canDash = true;
    }

    public void disableMovement(float time)
    {
        canMove = false;
        Invoke("ReEnableMovement", time);
    }

    void ReEnableMovement()
    {
        canMove = true;
    }
}

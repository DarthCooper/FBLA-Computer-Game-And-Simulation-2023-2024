using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;

public class PlayerMovementController : NetworkBehaviour, IDataPersistence
{
    Vector2 movement = Vector2.zero;
    Vector2 lastMovement = Vector2.zero;
    Rigidbody2D rb;
    Animator animator;

    public bool aiming;
    bool canChangeAim = true;

    public bool sprinting;

    public float speed = 1f;
    public float aimingSpeed;
    public float runSpeed;
    public GameObject PlayerModel;

    public bool canMove = true;

    public bool beingKnockedBack;

    public bool desiredDash;

    public float dashTime = 0.5f;
    public float dashSpeed;
    bool canDash = true;

    public float TimeBetweenDashes = 1f;

    public GameObject attackSprite;

    private void Awake()
    {
        PlayerModel.SetActive(false);
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void LoadData(GameData data)
    {
        attackSprite.SetActive(false);
        Time.timeScale = 1f;
        this.transform.position = data.playerPosition;
    }

    public void SaveData(ref GameData data)
    {
        attackSprite.SetActive(false);
        Time.timeScale = 1f;
        if (!isOwned) { return; }
        data.playerPosition = this.transform.position;
    }

    private void FixedUpdate()
    {
        if(SceneManager.GetActiveScene().name == "Game")
        {
            if(PlayerModel.activeSelf == false)
            {
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

    public void OnAim(InputAction.CallbackContext context)
    {
        if(context.ReadValueAsButton() && canChangeAim)
        {
            aiming = !aiming;
            if(aiming && sprinting)
            {
                sprinting = false;
            } 
            canChangeAim = false;
            Invoke("ChangeAim", 0.2f);
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        sprinting = context.ReadValueAsButton();
        if(sprinting && aiming)
        {
            aiming = false;
        }
    }

    public void ChangeAim()
    {
        canChangeAim = true;
    }

    public void PlayAnim(Vector2 direction)
    {
        animator.SetFloat("X", direction.x);
        animator.SetFloat("Y", direction.y);
    }

    public void Movement()
    {
        if(canMove)
        {
            var currentSpeed = speed;
            if(movement != Vector2.zero && !aiming)
            {
                if(sprinting && GetComponent<PlayerStats>().stamina > 0.5f)
                {
                    GetComponent<PlayerStats>().spendStamina(0.5f);
                    currentSpeed = runSpeed;
                }
                PlayAnim(lastMovement);

            }else if(aiming)
            {
                currentSpeed = aimingSpeed;
                Vector3 pos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                PlayAnim(pos - transform.position);
            }
            rb.velocity = movement * currentSpeed * Time.fixedDeltaTime;
        }
    }

    public void Dash()
    {
        if(GetComponent<PlayerStats>().stamina > 25)
        {
            desiredDash = false;
            canDash = false;
            GetComponent<Health>().TakeKnockback(dashTime, lastMovement * dashSpeed);
            GetComponent<PlayerStats>().spendStamina(25f);
            Invoke("ReEnableCanDash", TimeBetweenDashes);
        }
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

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
    Rigidbody2D rb;

    public float speed = 1f;
    public GameObject PlayerModel;

    private void Awake()
    {
        PlayerModel.SetActive(false);
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
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
                print(movement);
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
            movement = context.ReadValue<Vector2>();
        }
    }

    public void Movement()
    {
        rb.velocity = movement * speed;
    }
}

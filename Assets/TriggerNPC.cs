using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerNPC : NPC
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            ExecuteStep();
        }
    }
}

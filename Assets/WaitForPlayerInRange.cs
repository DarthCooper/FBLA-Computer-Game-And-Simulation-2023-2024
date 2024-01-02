using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForPlayerInRange : NPCStep
{
    public float acceptableRange = 5f;

    public GameObject localPlayer;
    public GameObject[] players;

    public bool stopMovement = true;

    public override void Execute()
    {
        if (npc == null)
        {
            npc = GetComponentInParent<NPC>();
        }
        localPlayer = GameObject.Find("LocalGamePlayer");
        players = GameObject.FindGameObjectsWithTag("Player");
        if(stopMovement)
        {
            npc.StopMovement();
        }
    }

    public void Update()
    {
        foreach (var player in players)
        {
            if(Vector3.Distance(npc.transform.position, player.transform.position) <= acceptableRange)
            {
                npc.EndStep();
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForPlayerInRange : NPCStep
{
    public float acceptableRange = 5f;

    public GameObject player;

    public bool stopMovement = true;

    public override void Execute()
    {
        if (npc == null)
        {
            npc = GetComponentInParent<NPC>();
        }
        player = GameObject.Find("LocalGamePlayer");
        if(stopMovement)
        {
            npc.StopMovement();
        }
    }

    public void Update()
    {
        if(Vector3.Distance(npc.transform.position, player.transform.position) <= acceptableRange)
        {
            npc.EndStep();
        }
    }
}

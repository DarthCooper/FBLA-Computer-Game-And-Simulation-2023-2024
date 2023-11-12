using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Readable : MonoBehaviour
{
    public string TextToDisplay;
    public Interactable interactable;

    public Transform interactingPlayer;

    public void onInteract()
    {
        Transform closest = GetClosest();
        if(closest.GetComponent<NetworkIdentity>().isLocalPlayer)
        {
            closest.GetComponent<PlayerInteract>().Read(TextToDisplay);
            interactingPlayer = closest;
        }
    }

    public void OnEndInteract()
    {
        if(interactingPlayer)
        {
            interactingPlayer.GetComponent<PlayerInteract>().ResetCanRead();
        }
    }

    public Transform GetClosest()
    {
        float maxDistance = 10000;
        Transform Target = null;
        foreach (var target in GameObject.FindGameObjectsWithTag("Player"))
        {
            float Distance = Vector2.Distance(transform.position, target.transform.position);
            if (Distance < maxDistance)
            {
                maxDistance = Distance;
                Target = target.transform;
            }
        }
        return Target;
    }
}

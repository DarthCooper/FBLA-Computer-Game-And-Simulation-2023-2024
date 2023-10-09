using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class ReviveMarker : NetworkBehaviour
{
    public GameObject player;
    public Interactable interactable;
    public float maxReviveTime = 5f; //In Seconds
    public float currentReviveTime = 0f;

    [SyncVar(hook = "ChangeRevive")]public bool reviving;

    public Slider progressBar;

    private void Awake()
    {
        interactable = GetComponent<Interactable>();
    }

    [Command(requiresAuthority = false)]
    public void startRevive(bool reviving)
    {
        ChangeRevive(this.reviving, reviving);
    }

    public void ChangeRevive(bool oldValue, bool newValue)
    {
        if(isServer)
        {
            reviving = newValue;
        }
        if(isClient)
        {
            reviving = newValue;
        }
    }

    public void StartRevive()
    {
        print("Interact");
        startRevive(true);
        interactable.beenInteractedWith = true;
    }

    public void EndRevive()
    {
        startRevive(false);
        print("EndInteract");
        interactable.beenInteractedWith = false;
    }

    private void Update()
    {
        if(reviving && currentReviveTime < maxReviveTime)
        {
            currentReviveTime += Time.deltaTime;
        }else if(currentReviveTime > 0 && !reviving)
        {
            currentReviveTime -= Time.deltaTime;
        }

        progressBar.value = currentReviveTime / maxReviveTime;

        if(currentReviveTime > maxReviveTime && reviving)
        {
            Revive();
        }
    }

    public void Revive()
    {
        player.GetComponent<Health>().Heal(1);
    }
}

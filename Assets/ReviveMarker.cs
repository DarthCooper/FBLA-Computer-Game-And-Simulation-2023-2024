using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReviveMarker : MonoBehaviour
{
    public GameObject player;
    public Interactable interactable;
    public float maxReviveTime = 5f; //In Seconds
    public float currentReviveTime = 0f;

    public bool reviving;

    public Slider progressBar;

    private void Awake()
    {
        interactable = GetComponent<Interactable>();
    }

    public void StartRevive()
    {
        reviving = true;
        interactable.beenInteractedWith = true;
    }

    public void EndRevive()
    {
        reviving = false;
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

        if(currentReviveTime > maxReviveTime)
        {
            Revive();
        }
    }

    public void Revive()
    {
        player.GetComponent<Health>().Heal(1);
    }
}

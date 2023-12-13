using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialoguePage : MonoBehaviour
{
    public TMP_Text messageText;

    public TMP_Text speakerText;

    public NPC npc;

    public void SetMessage(string message)
    {
        messageText.text = message;
        GetClosest();
    }

    public void OnClick()
    {
        npc.currentStep.Finish();
    }

    public virtual void GetClosest()
    {
        GameObject player = GameObject.Find("LocalGamePlayer");
        float maxDistance = 10000;
        foreach (var target in GameObject.FindGameObjectsWithTag("NPC"))
        {
            float Distance = Vector2.Distance(player.transform.position, target.transform.position);
            if (Distance < maxDistance)
            {
                maxDistance = Distance;
                npc = target.GetComponent<NPC>();
            }
        }
    }
}

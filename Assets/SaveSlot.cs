using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    [Header("Profile")]

    [SerializeField] private string profileID = "";

    [Header("Content")]

    [SerializeField] private GameObject noDataContent;
    [SerializeField] private GameObject hasDataContent;

    public void SetData(GameData data)
    {
        if(data == null)
        {
            noDataContent.SetActive(true);
            hasDataContent.SetActive(false);
        }else
        {
            noDataContent.SetActive(false);
            hasDataContent.SetActive(true);

            //Display Important info here. This will be player look, progress, etc.
        }
    }

    public string GetProfileID() { return profileID; }

    public void SetButton(bool interactable)
    {
        GetComponent<Button>().interactable = interactable;
    }
}

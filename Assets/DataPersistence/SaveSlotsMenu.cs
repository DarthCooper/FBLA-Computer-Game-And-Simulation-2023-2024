using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSlotsMenu : MonoBehaviour
{
    private SaveSlot[] saveSlots;

    public bool isLoadingGame = false;

    public void Awake()
    {
        saveSlots = this.GetComponentsInChildren<SaveSlot>();
    }

    public void OnSaveSlotClicked(SaveSlot slot)
    {
        DataPersistenceManager.instance.ChangeSelectedProfileId(slot.GetProfileID());
        if(!isLoadingGame)
        {
            DataPersistenceManager.instance.NewGame();
        }else
        {
            DataPersistenceManager.instance.LoadGame();
        }
    }

    public void ActivateMenu(bool isLoadingGame)
    {
        Dictionary<string, GameData> profilesGameData = DataPersistenceManager.instance.GetAllProfilesGameData();

        this.isLoadingGame = isLoadingGame;

        foreach(SaveSlot saveSlot in saveSlots)
        {
            GameData profileData = null;
            profilesGameData.TryGetValue(saveSlot.GetProfileID(), out profileData);
            saveSlot.SetData(profileData);
            if(profileData == null && isLoadingGame)
            {
                saveSlot.SetButton(false);
            }else
            {
                saveSlot.SetButton(true);
            }
        }
    }
}

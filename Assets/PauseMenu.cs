using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public void OnQuit()
    {
        Application.Quit();
        OnSaveGame();
    }

    public void OnLoadGame()
    {
        Manager.Instance.ReloadLevel();
    }

    public void OnSaveGame()
    {
        DataPersistenceManager.instance.SaveGame();
    }

    public void OnReturnToMenu()
    {

    }
}

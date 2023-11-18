using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSaver : MonoBehaviour, IDataPersistence
{
    public string SceneToLoad = "Opening";

    public string currentScene;

    public void LoadData(GameData data)
    {
        SceneToLoad = data.lastScene;
    }

    void Update()
    {
        if(currentScene != SceneManager.GetActiveScene().name && Manager.Instance.settings.isSavable)
        {
            currentScene = SceneManager.GetActiveScene().name;
        }
    }

    public void SaveData(ref GameData data)
    {
        print("Saving");
        data.lastScene = currentScene;
    }
}

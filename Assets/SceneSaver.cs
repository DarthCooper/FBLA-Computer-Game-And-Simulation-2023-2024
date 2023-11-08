using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSaver : MonoBehaviour, IDataPersistence
{
    public string SceneToLoad;

    public void LoadData(GameData data)
    {
        SceneToLoad = data.lastScene;
    }

    public void Update()
    {
        if(SceneManager.GetActiveScene().name != SceneToLoad)
        {
            SceneToLoad = SceneManager.GetActiveScene().name;
        }
    }

    public void SaveData(ref GameData data)
    {
        data.lastScene = SceneToLoad;  
    }
}

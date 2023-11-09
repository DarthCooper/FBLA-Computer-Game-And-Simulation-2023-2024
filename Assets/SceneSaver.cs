using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSaver : MonoBehaviour, IDataPersistence
{
    public string SceneToLoad = "Opening";

    public void LoadData(GameData data)
    {
        SceneToLoad = data.lastScene;
    }

    public void SaveData(ref GameData data)
    {
        data.lastScene = SceneManager.GetActiveScene().name;  
    }
}

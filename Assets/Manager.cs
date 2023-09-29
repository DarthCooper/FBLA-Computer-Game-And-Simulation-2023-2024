using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
    }
}

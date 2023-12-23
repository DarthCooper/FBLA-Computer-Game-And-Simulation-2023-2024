using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSwitcher : MonoBehaviour
{

    public CinemachineVirtualCamera VirtualCamera;

    public List<string> keys = new List<string>();
    public List<Transform> values = new List<Transform>();

    private Dictionary<string, Transform> cameraPoints = new Dictionary<string, Transform>();

    private void Awake()
    {
        for (int i = 0; i < keys.Count; i++)
        {
            cameraPoints.Add(keys[i], values[i]);
        }
        cameraPoints.Add("You", GameObject.Find("LocalGamePlayer").transform);
    }

    public void SwitchCamera(string key)
    {
        VirtualCamera.Follow = cameraPoints[key];
    }

    public void setValue(string key, Transform value)
    {
        cameraPoints[key] = value;
    }
}

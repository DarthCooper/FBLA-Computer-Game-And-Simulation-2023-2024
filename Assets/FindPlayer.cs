using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FindPlayer : MonoBehaviour
{
    CinemachineVirtualCamera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        if(cam.Follow == null)
        {
            if(GameObject.Find("LocalGamePlayer"))
            {
                cam.Follow = GameObject.Find("LocalGamePlayer").transform;
            }
        }
    }
}

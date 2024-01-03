using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceSwitcher : MonoBehaviour
{
    public GameObject[] faces;

    public void SwitchFace(string emotion)
    {
        foreach (var face in faces)
        {
            face.SetActive(false);
            if(face.name == emotion)
            {
                face.SetActive(true);
            }
        }
    }
}

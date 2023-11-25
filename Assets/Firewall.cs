using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firewall : MonoBehaviour
{
    bool canInteract;
    public void OnInteract()
    {
        if(canInteract)
        {
            ComputerPuzzle.Instance.EnableComputer();
            canInteract = false;
        }
    }

    public void OnEndInteract()
    {
        canInteract = true;
    }
}

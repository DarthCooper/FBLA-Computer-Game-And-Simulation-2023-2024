using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReadPage : MonoBehaviour
{
    public TMP_Text messageText;

    public void SetMessage(string message)
    {
        messageText.text = message;
    }
}

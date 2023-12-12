using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialUI : MonoBehaviour
{
    public string action;
    [TextArea] public string actionResponse;
    public TMP_Text actionText;

    public InputActionReference currentActionMap;

    public void Displaytext()
    {
        if (currentActionMap == null) { return; }
        var control = currentActionMap.action.controls[0];
        string buttonName = InputControlPath.ToHumanReadableString(control.path, InputControlPath.HumanReadableStringOptions.OmitDevice);
        if (!String.IsNullOrEmpty(buttonName))
        {
            buttonName = string.Concat(buttonName[0].ToString().ToUpper(), buttonName.Substring(1));
        }
        actionText.text = "Click " + buttonName + " " + actionResponse;
    }
}

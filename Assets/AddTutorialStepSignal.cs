using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AddTutorialStepSignal : MonoBehaviour
{
    public GameObject parent;

    public Component signal;

    public int signalIndex;

    public bool SignalNot;

    public UnityEvent onFinish;

    public void addSignal()
    {
        if(signal is EnableTutorialSignal)
        {
            var script = parent.AddComponent<EnableTutorialSignal>();
            script.goOnDisable = SignalNot;
            script.onFinish = onFinish;
        }
        if(signal is ComponentTutorialSignal)
        {
            var script = parent.AddComponent<ComponentTutorialSignal>();
            script.component = parent.GetComponent<Image>();
            script.goOnDisable = SignalNot;
            script.onFinish = onFinish;
        }
    }
}

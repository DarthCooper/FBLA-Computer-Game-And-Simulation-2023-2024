using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class TutorialStepSignal : MonoBehaviour
{
    public UnityEvent onFinish;

    public void finishStep()
    {
        if(!TutorialManager.instance) { return; }
        if(!TutorialManager.instance.currentStep) { return; }
        TutorialManager.instance.currentStep.FinishTutorialStep(this);
        onFinish.Invoke();
    }
}

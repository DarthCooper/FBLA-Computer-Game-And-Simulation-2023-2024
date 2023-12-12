using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialStepSignal : MonoBehaviour
{
    public void finishStep()
    {
        TutorialManager.instance.currentStep.FinishTutorialStep(this);
    }
}

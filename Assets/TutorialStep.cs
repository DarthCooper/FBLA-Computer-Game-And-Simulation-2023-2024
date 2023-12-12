using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialStep : MonoBehaviour
{
    private bool isFinished = false;
    public string tutorialID;
    private int stepIndex = 0;

    TutorialUI stepUI;

    private void Awake()
    {
        stepUI = GetComponent<TutorialUI>();
    }

    public void InitializeTutorialStep(string tutorialID, int stepIndex)
    {
        this.tutorialID = tutorialID;
        this.stepIndex = stepIndex;
        stepUI.Displaytext();
    }

    public void FinishTutorialStep(TutorialStepSignal signal)
    {
        Destroy(signal);
        if (!isFinished)
        {
            TutorialManager.instance.AdvanceCurrentTutorial();
            isFinished = true;
            Destroy(gameObject);
        }
    }
}

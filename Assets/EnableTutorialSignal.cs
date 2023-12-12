using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableTutorialSignal : TutorialStepSignal
{
    public bool goOnDisable;
    private void OnEnable()
    {
        if(!goOnDisable)
        {
            finishStep();
        }
    }

    private void OnDisable()
    {
        if(!goOnDisable)
        {
            finishStep();
        }
    }
}

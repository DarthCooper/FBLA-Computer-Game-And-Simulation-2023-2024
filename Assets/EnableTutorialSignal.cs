using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableTutorialSignal : TutorialStepSignal
{
    public bool doNothing = false;
    public bool goOnDisable;
    private void OnEnable()
    {
        if(doNothing) { return; }
        if(!goOnDisable)
        {
            finishStep();
        }
    }

    private void OnDisable()
    {
        if (doNothing) { return; }
        if (goOnDisable)
        {
            finishStep();
        }
    }
}

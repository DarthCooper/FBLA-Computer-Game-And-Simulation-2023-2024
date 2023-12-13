using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WatchPlayerInput : TutorialStepSignal
{
    public InputActionReference actionReference;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(actionReference.action.triggered || actionReference.action.ReadValue<float>() > 0)
        {
            finishStep();
        }
    }
}

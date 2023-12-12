using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    public GameObject tutorialPrefab;

    public TutorialDataSO[] tutorials;

    public TutorialDataSO currentTutorial;
    public TutorialStep currentStep;
    int tutorialIndex = 0;

    public Transform tutorialParent;

    private void Awake()
    {
        instance = this;
    }

    public void RunTutorial(string key)
    {
        if(currentTutorial != null) { return; }
        foreach (var tutorial in tutorials)
        {
            if(tutorial.displayName == key)
            {
                currentTutorial = tutorial;
                tutorialIndex = 0;
                spawnTutorialStep();
            }
        }
    }

    public void spawnTutorialStep()
    {
        TutorialStep step = Instantiate(currentTutorial.TutorialStepPrefabs[tutorialIndex], tutorialParent).GetComponent<TutorialStep>();
        step.InitializeTutorialStep(currentTutorial.id, tutorialIndex);
        currentStep = step;
    }

    public void AdvanceCurrentTutorial()
    {
        tutorialIndex++;
        if(tutorialIndex < currentTutorial.TutorialStepPrefabs.Length)
        {
            spawnTutorialStep();
        }else
        {
            FinishCurrentTutorial();
        }
    }

    public void FinishCurrentTutorial()
    {
        currentTutorial = null;
    }
}

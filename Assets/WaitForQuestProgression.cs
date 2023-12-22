using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForQuestProgression : NPCStep
{
    public QuestStep currentStep;
    public string questID;

    QuestStep lastStep;

    private void Awake()
    {
        findStep();
    }

    void findStep()
    {
        if (!QuestManager.instance || !GetComponentInParent<QuestPoint>()) { return; }
        currentStep = QuestManager.instance.getQuestStep(GetComponentInParent<QuestPoint>().questInfoForPoint.id, questID);
    }

    private void Update()
    {
        if(currentStep == null)
        {
            findStep();
        }
        if((currentStep == null && lastStep) || currentStep.isFinished)
        {
            Finish();
        }
        if(currentStep != null)
        {
            lastStep = currentStep;
        }
    }

    public override void Finish()
    {
        npc.EndStep();
        print("finished");
    }
}

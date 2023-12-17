using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestSlot : MonoBehaviour
{
    public string questName;
    public QuestStep step;
    public bool usesArrow;
    public string description;
    public string maxProgress;

    public TMP_Text NameText;
    public TMP_Text progressText;

    public void SetValues()
    {
        NameText.text = questName;
        if(step != null)
        {
            progressText.text = step.progress;
            usesArrow = step.usesArrow;
        }
        else
        {
            progressText.text = maxProgress;
        }
    }

    private void Update()
    {
        if (step != null)
        {
            progressText.text = step.progress;
            usesArrow = step.usesArrow;
        }
        else
        {
            progressText.text = maxProgress;
        }
    }

    public void SelectQuest()
    {
        Journal journal = GetComponentInParent<Journal>();
        journal.SelectQuest(this);
    }
}

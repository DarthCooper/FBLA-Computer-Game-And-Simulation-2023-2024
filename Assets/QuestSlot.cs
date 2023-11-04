using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestSlot : MonoBehaviour
{
    public string questName;
    public QuestStep step;
    public string description;

    public TMP_Text NameText;
    public TMP_Text progressText;

    public void SetValues()
    {
        NameText.text = questName;
        if(step != null)
        {
            progressText.text = step.progress;
        }
    }
}

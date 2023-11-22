using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class QuestPoint : MonoBehaviour
{
    [Header("Quest")]
    [SerializeField] private QuestInfoSO questInfoForPoint;

    [Header("Config")]
    [SerializeField] private bool startPoint = true;
    [SerializeField] private bool finishPoint = true;

    private bool playerIsNear = false;
    private string questId;
    public QuestState currentQuestState;

    private QuestIcon questIcon;

    private void Awake()
    {
        questId = questInfoForPoint.id;
        questIcon = GetComponentInChildren<QuestIcon>();
    }

    private void Start()
    {
        Manager.Instance.questEvents.onQuestStateChange += QuestStateChange;
    }

    private void OnDisable()
    {
        Manager.Instance.questEvents.onQuestStateChange -= QuestStateChange;
    }

    public void StartQuest()
    {
        if(GetComponent<Interactable>())
        {
            if(!playerIsNear || GetComponent<Interactable>().beenInteractedWith)
            {
                return;
            }
        }
        if (currentQuestState.Equals(QuestState.CAN_START) && startPoint)
        {
            print("StartingQuest");
            Manager.Instance.questEvents.StartQuest(questId);
            if(GetComponent<Interactable>() && !finishPoint)
            {
                GetComponent<Interactable>().beenInteractedWith = true;
            }
        }else if(currentQuestState.Equals(QuestState.CAN_FINISH) && finishPoint)
        {
            Manager.Instance.questEvents.FinishQuest(questId);
            if (GetComponent<Interactable>())
            {
                GetComponent<Interactable>().beenInteractedWith = true;
            }
        }
    }

    private void QuestStateChange(Quest quest, QuestState state)
    {
        if(quest.info.id.Equals(questId))
        {
            quest.state = state;
            currentQuestState = quest.state;
            if(questIcon)
            {
                questIcon.SetState(currentQuestState, startPoint, finishPoint);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            playerIsNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsNear = false;
        }
    }
}

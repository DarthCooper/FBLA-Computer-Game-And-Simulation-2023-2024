using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Pathfinding;
using Mirror;
using UnityEngine.Events;

public class NPC : NetworkBehaviour
{
    public string NPCName;

    public string[] prerequisites; 

    public NPCStep[] steps;

    public int currentStepIndex = 0;

    public GameObject speechBubble;
    public TMP_Text speechText;

    public NPCStep currentStep;

    Seeker seeker;
    Rigidbody2D rb;

    public bool canMove = false;

    public Transform target;

    public float speed = 200f;
    public float nextWaypointDistance = 3f;

    Path path;
    int currentWaypoint = 0;
    public bool reachedEndOfPath = false;

    public float targetTolerance = 1f;

    public Transform[] waypoints;

    public UnityEvent OnFinishSteps;

    public QuestPoint quest;

    [HideInInspector]public bool canRun = true;

    bool Finished = true;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        quest = GetComponent<QuestPoint>();

        if (!isServer) { return; }
        InvokeRepeating("UpdatePath", 0f, .5f);
    }

    void UpdatePath()
    {
        if (target == null) { return; }
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    private void Update()
    {
        move();
    }

    private void Awake()
    {
        checkIfRequirementsMet();
    }

    public void move()
    {
        if (path == null) { return; }
        if (currentWaypoint >= path.vectorPath.Count) { return; }
        if (target == null) { return; }
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.fixedDeltaTime;


        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
        if (canMove)
        {
            rb.AddForce(force);

            float distToTarget = Vector2.Distance(rb.position, target.position);
            if (distToTarget <= targetTolerance)
            {
                if(currentStepIndex >= steps.Length)
                {
                    StopMovement();
                }else
                {
                    EndStep();
                }
            }
        }

    }

    public bool checkIfRequirementsMet()
    {
        foreach (var pre in prerequisites)
        {
            if (NPCManager.NPCsCompleted.ContainsKey(pre))
            {
                if (!NPCManager.NPCsCompleted[pre])
                {
                    DisableOnFinish();
                    return false;
                }
                else
                {
                    EnableOnFinish();
                }
            }
            else
            {
                DisableOnFinish();
                return false;
            }
        }
        return true;
    }

    public void ExecuteStep()
    {
        CmdExecuteStep();
    }

    [Command(requiresAuthority = false)]
    public void CmdExecuteStep()
    {
        if(isServer)
        {
            ServerExecuteStep();
        }
    }

    [Server]
    public void ServerExecuteStep()
    {
        RpcExecuteStep();
    }

    [ClientRpc]
    public void RpcExecuteStep()
    {
        if (!Finished)
        {
            return;
        }
        Finished = false;
        if (!checkIfRequirementsMet()) { return; }
        if (!canRun) { return; }
        if (currentStep)
        {
            Destroy(currentStep.gameObject);
        }
        if (currentStepIndex < steps.Length)
        {
            InsatiateStep(steps[currentStepIndex]);
            currentStep.Execute();
        }
    }

    public void InsatiateStep(NPCStep step)
    {
        currentStep = Instantiate(step);
        currentStep.transform.parent = this.transform;
        currentStep.SetNpc(this);
    }

    public void EndStep()
    {
        if(isServer)
        {
            RpcEndStep();
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdChangeStep()
    {
        ChangeCurrentStep(currentStepIndex, currentStepIndex++);
    }

    public void ChangeCurrentStep(int oldValue, int newValue)
    {
        if(Finished) { return; }
        Finished = true;
        Invoke(nameof(ResetFinish), 1f);
        currentStepIndex = newValue;
        if (currentStepIndex >= steps.Length)
        {
            OnFinishSteps.Invoke();
            NPCManager.CompleteNPC(NPCName);
        }
        ExecuteStep();
    }

    public void ResetFinish()
    {
        Finished = false;
    }

    [Command(requiresAuthority = false)]
    public void CmdEndStep()
    {
        if(isServer)
        {
            ServerEndStep();
        }
    }

    [Server]
    public void ServerEndStep()
    {
        RpcEndStep();
    }

    [ClientRpc(includeOwner = true)]
    public void RpcEndStep()
    {
        if (Finished)
        {
            return;
        }
        Finished = true;
        currentStepIndex++;
        if (currentStepIndex >= steps.Length)
        {
            OnFinishSteps.Invoke();
            NPCManager.CompleteNPC(NPCName);
            print("finished");
        }
        ExecuteStep();
    }

    public void DisplayText(string text)
    {
        speechBubble.SetActive(true);
        speechText.text = text;
    }

    public void HideText()
    {
        speechText.text = "";
        speechBubble.SetActive(false);
    }

    public void StartMovement(string targetName)
    {
        foreach(Transform waypoint in waypoints)
        {
            if(waypoint.name == targetName)
            {
                target = waypoint; 
                break;
            }
        }
        canMove = true;
    }

    public void StopMovement()
    {
        canMove = false;
        rb.velocity = Vector3.zero;
    }

    public void DisableOnFinish()
    {
        canRun = false;
        foreach(var component in GetComponentsInChildren<SpriteRenderer>())
        {
            component.enabled = false;
        }
        foreach (var component in GetComponentsInChildren<Collider2D>())
        {
            component.enabled = false;
        }
    }

    public void EnableOnFinish()
    {
        canRun = true;
        foreach (var component in GetComponentsInChildren<SpriteRenderer>())
        {
            component.enabled = true;
        }
        foreach (var component in GetComponentsInChildren<Collider2D>())
        {
            component.enabled = true;
        }
    }
}

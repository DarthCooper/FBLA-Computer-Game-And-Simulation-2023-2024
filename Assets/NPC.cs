using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Pathfinding;
using Mirror;

public class NPC : NetworkBehaviour
{
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

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

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

    public void move()
    {
        if (path == null) { return; }
        if (currentWaypoint >= path.vectorPath.Count) { return; }
        if (target == null) { return; }
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.fixedDeltaTime;


        if (canMove)
        {
            rb.AddForce(force);
        }

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        float distToTarget = Vector2.Distance(rb.position, target.position);
        if (distToTarget <= targetTolerance)
        {
            StopMovement();
            EndStep();
        }
    }


    public void ExecuteStep()
    {
        if(currentStep)
        {
            Destroy(currentStep.gameObject);
        }
        if(currentStepIndex < steps.Length)
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
        currentStepIndex++;
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
}

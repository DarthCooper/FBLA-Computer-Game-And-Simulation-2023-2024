using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldFollow : MonoBehaviour
{
    public Transform followTransform;

    public bool canParry = true;

    public float parryTime = 0.2f;

    private void Awake()
    {
        canParry = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(followTransform)
        {
            transform.position = followTransform.position;
            transform.rotation = followTransform.rotation;
        }
        if(parryTime > 0)
        {
            parryTime -= Time.deltaTime;
        }else
        {
            canParry = false;
        }
    }
}

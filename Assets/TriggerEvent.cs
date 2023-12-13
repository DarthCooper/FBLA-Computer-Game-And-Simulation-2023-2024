using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    public UnityEvent onEnter;
    public UnityEvent onStay;
    public UnityEvent onExit;

    public string acceptedTag;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        print(collision.gameObject.tag);
        if (collision.gameObject.tag == acceptedTag)
        {
            onEnter.Invoke();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.tag == acceptedTag)
        {
            onStay.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == acceptedTag)
        {
            onExit.Invoke();
        }
    }
}

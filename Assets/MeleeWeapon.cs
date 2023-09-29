using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;

public class MeleeWeapon : MonoBehaviour
{
    public UnityEvent onHit;
    public string[] hitableTags;

    public float knockBack;

    GameObject hitObject;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (var tag in hitableTags)
        {
            if(collision.gameObject.tag == tag)
            {
                hitObject = collision.gameObject;
                onHit.Invoke();
            }
        }
    }

    public void HitPlayer()
    {
        Time.timeScale = 0.1f;
        Invoke("ResetTime", 0.1250f);
        Invoke("ApplyKnockback", 0.05f);
    }

    public void ApplyKnockback()
    {
        Time.timeScale = 0.25f;
        Vector2 direction = (hitObject.transform.position - transform.position).normalized;
        Vector2 force = direction * knockBack;
        hitObject.GetComponent<PlayerMovementController>().TakeKnockback(0.0625f, force);
    }

    void ResetTime()
    {
        Time.timeScale = 1.0f;
    }
}

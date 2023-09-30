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

    public float damage;
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
        if(hitObject.GetComponent<Rigidbody2D>() != null)
        {
            hitObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;  
        }
        Time.timeScale = 0.1f;
        Invoke("ResetTime", 0.1250f);
        Invoke("ApplyKnockbackWithTimeScale", 0.05f);
        hitObject.GetComponent<Health>().TakeDamage(damage);
    }

    public void HitEnemy()
    {
        if (hitObject.GetComponent<Rigidbody2D>() != null)
        {
            hitObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
        ApplyKnockback();
        hitObject.GetComponent<Health>().TakeDamage(damage);
    }

    public void ApplyKnockbackWithTimeScale()
    {
        Time.timeScale = 0.25f;
        Vector2 direction = (hitObject.transform.position - transform.position).normalized;
        Vector2 force = direction * knockBack;
        hitObject.GetComponent<Health>().TakeKnockback(0.0625f, force);
    }

    public void ApplyKnockback()
    {
        Vector2 direction = (hitObject.transform.position - transform.position).normalized;
        Vector2 force = direction * knockBack;
        hitObject.GetComponent<Health>().TakeKnockback(0.25f, force);
    }

    void ResetTime()
    {
        Time.timeScale = 1.0f;
    }
}

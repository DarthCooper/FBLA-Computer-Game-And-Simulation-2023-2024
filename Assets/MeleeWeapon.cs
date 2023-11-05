using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

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
        ApplyKnockback();
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

    IEnumerator ApplyKnockbackWithTimeScale(float time)
    {
        yield return new WaitForSeconds(time);
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

    IEnumerator ResetTime(float time)
    {
        yield return new WaitForSeconds(time);
        Time.timeScale = 1.0f;
    }
}

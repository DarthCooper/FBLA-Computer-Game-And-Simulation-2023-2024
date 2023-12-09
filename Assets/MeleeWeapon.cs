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

    public bool canHit = true;

    public List<Collider2D> colliders = new List<Collider2D>();
    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (var tag in hitableTags)
        {
            if(collision.gameObject.tag == tag)
            {
                colliders.Add(collision);
                hitObject = collision.gameObject;
                onHit.Invoke();
            }
        }
    }

    public void OnDisable()
    {
        canHit = true;
    }

    public void HitPlayer()
    {
        foreach (var collision in colliders)
        {
            if (collision == null) { continue; }
            print(collision.gameObject.name + ": " + collision.gameObject.layer);
            if (collision.gameObject.layer == 10)
            {
                if(collision.gameObject.GetComponent<ShieldFollow>())
                {
                    if(collision.gameObject.GetComponent<ShieldFollow>().canParry)
                    {
                        GetComponentInParent<EnemyAI>().StunEnemy(5f);
                        Debug.Log("Stunned");
                    }
                }
                return;
            }
        }
        if (hitObject.GetComponent<Rigidbody2D>() != null)
        {
            hitObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;  
        }
        ApplyKnockback();
        hitObject.GetComponent<Health>().TakeDamage(damage);
        colliders.Clear();
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

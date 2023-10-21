using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
    public UnityEvent onHit;
    public string[] hitableTags;

    public string[] ignoreTags;

    public float speed;

    Rigidbody2D rb;

    public float knockBack;

    GameObject hitObject;

    public float damage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool hitRequiresResponse = false;
        print(collision.gameObject.name);
        foreach (var tag in hitableTags)
        {
            if (collision.gameObject.tag == tag)
            {
                hitRequiresResponse = true;
                hitObject = collision.gameObject;
                onHit.Invoke();
                HideObject();
            }
        }
        foreach (var tag in ignoreTags)
        {
            if(collision.gameObject.tag == tag)
            {
                hitRequiresResponse = true;
            }
        }
        if(!hitRequiresResponse)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = transform.up * speed;
    }

    public void HitPlayer()
    {
        if (hitObject.GetComponent<Rigidbody2D>() != null)
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
        Vector2 direction = (transform.position - hitObject.transform.position).normalized;
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

    public void HideObject()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().isKinematic = true;
    }
}

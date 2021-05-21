using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField] int damage = 100;
    Animator animator;

    public void Start()
    {
        animator = GetComponent<Animator>();
    }

    public int GetDamage()
    {
        return damage;
    }

    public void Hit()
    {
        StartCoroutine(ProcessHit());
    }

    private IEnumerator ProcessHit()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        animator.SetTrigger("Dead");
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}
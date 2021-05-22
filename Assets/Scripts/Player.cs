using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class Player : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] int health = 300;
    [SerializeField] AudioClip deathSound;
    [SerializeField] [Range(0, 1)] float deathSoundVolume = 0.25f;
    [SerializeField] AudioClip shootSound;
    [SerializeField] [Range(0, 1)] float shootSoundVolume = 0.25f;
    [SerializeField] AudioClip hitSound;
    [SerializeField] [Range(0, 1)] float hitSoundVolume = 0.25f;

    [Header("Projectile")]
    [SerializeField] GameObject laserPrefabL;
    [SerializeField] GameObject laserPrefabM;
    [SerializeField] GameObject laserPrefabR;
    [SerializeField] float laserSpeed = 10f;
    [SerializeField] float laserTimeGap = 0.1f;

    Animator animator;

    float xMin, xMax, yMin, yMax;

    bool isDying;

    Coroutine firingCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        SetUpMoveBoundaries();
        animator = GetComponent<Animator>();
        isDying = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDying == false)
        {
            Move();
            Fire();
        }
        else
        {
            StopCoroutine(firingCoroutine);
        }
    }   

    private void Move()
    {
        var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
        var newXPos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);
        var newYPos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);
        transform.position = new Vector2(newXPos, newYPos);        
    }

    private void Fire()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            firingCoroutine = StartCoroutine(FireContinuously());
        }
        
        if (Input.GetButtonUp("Fire1"))
        {
            StopCoroutine(firingCoroutine);
        }
    }

    IEnumerator FireContinuously()
    {
        while (true)
        {
            GameObject laserL = Instantiate(laserPrefabL, new Vector2(transform.position.x - (GetComponent<SpriteRenderer>().bounds.size.x/3), transform.position.y + (GetComponent<SpriteRenderer>().bounds.size.y/2)), Quaternion.identity) as GameObject;
            laserL.GetComponent<Rigidbody2D>().velocity = new Vector2(0, laserSpeed);

            GameObject laserM = Instantiate(laserPrefabM, transform.position, Quaternion.identity) as GameObject;
            laserM.GetComponent<Rigidbody2D>().velocity = new Vector2(0, laserSpeed * 2);

            GameObject laserR = Instantiate(laserPrefabR, new Vector2(transform.position.x + (GetComponent<SpriteRenderer>().bounds.size.x/3), transform.position.y + (GetComponent<SpriteRenderer>().bounds.size.y/2)), Quaternion.identity) as GameObject;
            laserR.GetComponent<Rigidbody2D>().velocity = new Vector2(0, laserSpeed);

            AudioSource.PlayClipAtPoint(shootSound, Camera.main.transform.position, shootSoundVolume);

            yield return new WaitForSeconds(laserTimeGap);
        }        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer;
        try
        {
            damageDealer = other.gameObject.GetComponent<DamageDealer>();
        }
        catch (NullReferenceException ex)
        {
            return;
        }

        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        if(damageDealer != null)
        {
            Destroy(GameObject.Find("Life (" + health / 100 + ")"));
            health -= damageDealer.GetDamage();
            damageDealer.Hit();
        }

        if ((health <= 0) && (isDying == false))
        {
            StartCoroutine(Kill());
        }
        else if (isDying == false)
        {
            AudioSource.PlayClipAtPoint(hitSound, Camera.main.transform.position, hitSoundVolume);
        }
    }
    public void HitByEnemy()
    {
        Destroy(GameObject.Find("Life (" + health / 100 + ")"));
        health -= 100;

        if ((health <= 0) && (isDying == false))
        {
            StartCoroutine(Kill());
        }
    }

    private IEnumerator Kill()
    {
        isDying = true;

        animator.SetTrigger("Dead");
        AudioSource.PlayClipAtPoint(deathSound, Camera.main.transform.position, deathSoundVolume);
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
        SceneManager.LoadScene("Game");

        isDying = false;
    }

    private void SetUpMoveBoundaries()
    {
        Camera gameCamera = Camera.main;
        float xpadding = GetComponent<SpriteRenderer>().bounds.size.x/3;
        float ypaddingS = GetComponent<SpriteRenderer>().bounds.size.y/3;
        float ypaddingN = GetComponent<SpriteRenderer>().bounds.size.y/3;

        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + xpadding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - xpadding;
        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + ypaddingS;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - ypaddingN;
    }
}
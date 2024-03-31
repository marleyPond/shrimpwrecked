using UnityEngine;
using System.Collections;
using System;

public class AnglerFishOrifice : Enemy
{

    Shrimp shrimp;

    float discoPelletSpeed = 3f;
    public Vector3 originalLoc;

    public bool kill;

    public Enemy owner;

    public bool eightShot = false;

    public void Awake()
    {
        BaseInit();
    }

    void Start()
    {
        originalLoc = transform.localPosition;
        currentHealth = maxHealth = 15;
        tag = ENEMY;
        loseOnRecieveDamageMethodOnActivation = false;
        OnRecieveDamage = HandleRBK;
    }

    bool HandleRBK()
    {
        if (currentHealth == 1)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
        }
        return true;
    }

    public override IEnumerator Move()
    {
        shrimp = FindObjectOfType<Shrimp>();
        if (rb != null)
        {
            Destroy(rb);
            rb = null;
            transform.parent = owner.transform;
            transform.localPosition = originalLoc;
        }
        while (isMoving)
        {
            if (kill)
            {
                currentHealth = 1;
                RecieveDamage(currentHealth, Vector3.up);
                kill = false;
            }
            if (eightShot)
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(1.5f, 3f));
                yield return StartCoroutine(ProcessShot());
            }
            else
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(3f, 5f));
                yield return StartCoroutine(ProcessShot());
            }

        }
    }

    IEnumerator ProcessShot()
    {
        bool waitingForShot = false;

        if (waitingForShot)
            yield return new WaitForSeconds(.25f);
        if (StaticBuddy.IsOutOfBounds(transform))
            waitingForShot = true;
        else
        {
            if (!eightShot)
            {
                FireDiscoPelletAtTarget(transform.position, shrimp.transform.position, true, discoPelletSpeed);
            }
            else
            {
                FireDiscoPelletAtTargetEightWays(transform.position, true, discoPelletSpeed / 2f);
            }
            waitingForShot = false;
        }
        
    }

    protected override IEnumerator OnEndOfDeathProcess()
    {
        transform.parent = null;
        if (owner != null)
            owner.RecieveDamage(1, Vector3.up);
        yield return null;
        gameObject.SetActive(false);
    }

    protected override void SetSpeedMod()
    {
        speedMod = 0;
    }
}

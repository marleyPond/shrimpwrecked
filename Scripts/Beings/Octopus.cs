using UnityEngine;
using System.Collections;
using System;

public class Octopus : Enemy
{
    int verDir = 1;
    bool switchingDir = false;

    float switchDirBaseMS = 0.25f;

    public void Awake()
    {
        BaseInit();
    }

    public void Start()
    {
        tag = ENEMY;
        OnDeathStart = EnemyOnDeathStart;
    }

    public override IEnumerator Move()
    {
        StartCoroutine(ProcessSineTravel());
        DetermineDirectionToFace();
        
        if (moveDir == 0)
        {
            while (isMoving)
            {
                rb.velocity = Vector2.left * -speedMod;
                rb.velocity = new Vector2(rb.velocity.x, 2f * verDir);
                yield return null;
                if (StaticBuddy.IsOutOfBoundsExtendedRightOnly(transform))
                    RecieveDamageMaximum();
            }
        }else if (moveDir == 2)
        {
            while (isMoving)
            {
                rb.velocity = Vector2.left * speedMod;
                rb.velocity = new Vector2(rb.velocity.x, 2f * verDir);
                yield return null;
                if (StaticBuddy.IsOutOfBoundsExtendedLeftOnly(transform))
                    RecieveDamageMaximum();
            }
        }else if (moveDir == 1)
        {
            while (isMoving)
            {
                rb.velocity = Vector2.left * speedMod;
                rb.velocity = new Vector2(2f * verDir, rb.velocity.x);
                yield return null;
                if (StaticBuddy.IsOutOfBoundsExtendedBottomOnly(transform))
                    RecieveDamageMaximum();
            }
        }else
        {
            while (isMoving)
            {
                rb.velocity = Vector2.left * -speedMod;
                rb.velocity = new Vector2(2f * verDir, rb.velocity.x);
                yield return null;
                if (StaticBuddy.IsOutOfBoundsExtendedTopOnly(transform))
                    RecieveDamageMaximum();
            }
        }
        
        switchingDir = false;
        animate = false;
        yield return null;
    }

    IEnumerator ProcessSineTravel()
    {
        switchingDir = true;
        float ms = switchDirBaseMS;
        while (switchingDir)
        {
            verDir *= -1;
            yield return new WaitForSeconds(ms);
            ms = switchDirBaseMS*2;
        }
    }

    protected override IEnumerator OnEndOfDeathProcess()
    {
        isMoving = false;
        animate = false;
        yield return null;
        animator.gameObject.SetActive(true);
        col.enabled = true;
        FindObjectOfType<Director>().ReturnOctopusToPool(this);
    }

    protected override void SetSpeedMod()
    {
        speedMod = 1.4f;
    }
}

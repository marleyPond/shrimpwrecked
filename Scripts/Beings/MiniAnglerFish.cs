using UnityEngine;
using System.Collections;
using System;

public class MiniAnglerFish : Enemy
{
    Shrimp shrimp;
    Transform pivot;

    public void Awake()
    {
        BaseInit();
        maxHealth = currentHealth = 1;
        pivot = transform.GetChild(0);
    }

    public void Start()
    {
        tag = ENEMY;
        OnDeathStart = EnemyOnDeathStart;
        spriteFacesRightDefault = false;
    }

    public override IEnumerator Move()
    {
        shrimp = FindObjectOfType<Shrimp>();
        goingRight = !goingRight;
        BeginTurn();
        while (isMoving)
        {
            StaticBuddy.FaceOther(pivot, shrimp.transform.position);
            rb.velocity = pivot.right * speedMod;
            if (goingRight && rb.velocity.x < 0)
            {
                goingRight = false;
                BeginTurn();
            }
            else if (goingRight == false && rb.velocity.x > 0)
            {
                goingRight = true;
                BeginTurn();
            }

            yield return null;
        }
        animate = false;
    }

    protected override IEnumerator OnEndOfDeathProcess()
    {
        isMoving = false;
        animate = false;
        yield return null;
        animator.gameObject.SetActive(true);
        col.enabled = true;
        FindObjectOfType<Director>().ReturnMiniAnglerFishToPool(this);
    }

    protected override void SetSpeedMod()
    {
        speedMod = 1.5f;
    }
}

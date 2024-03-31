using UnityEngine;
using System.Collections;
using System;

public class GenericFish : Enemy
{

    public void Awake()
    {
        BaseInit();
    }

    public void Start()
    {
        tag = ENEMY;
        OnDeathStart = EnemyOnDeathStart;
    }

    protected override void SetSpeedMod()
    {
        speedMod = 2.0f;
    }

    public override IEnumerator Move()
    {
        DetermineDirectionToFace();
        
        if (moveDir == 0)
        {
            transform.eulerAngles = Vector3.zero;
            while (isMoving)
            {
                rb.velocity = Vector2.left * -speedMod;
                yield return null;
                if (StaticBuddy.IsOutOfBoundsExtendedRightOnly(transform))
                    RecieveDamageMaximum();
            }
        }else if (moveDir == 2)
        {
            transform.eulerAngles = Vector3.zero;
            while (isMoving)
            {
                rb.velocity = Vector2.left * speedMod;
                yield return null;
                if (StaticBuddy.IsOutOfBoundsExtendedLeftOnly(transform))
                    RecieveDamageMaximum();
            }
        }else if(moveDir == 1)
        {
            transform.eulerAngles = new Vector3(0, 0, 90);
            while (isMoving)
            {
                rb.velocity = Vector2.up * -speedMod;
                yield return null;
                if (StaticBuddy.IsOutOfBoundsExtendedBottomOnly(transform))
                    RecieveDamageMaximum();
            }
        }else
        {
            transform.eulerAngles = new Vector3(0, 0, 270);
            while (isMoving)
            {
                rb.velocity = Vector2.up * speedMod;
                yield return null;
                if (StaticBuddy.IsOutOfBoundsExtendedTopOnly(transform))
                    RecieveDamageMaximum();
            }
        }
        animate = false;
        yield return null;
    }

    protected override IEnumerator OnEndOfDeathProcess()
    {
        isMoving = false;
        animate = false;
        yield return null;
        animator.gameObject.SetActive(true);
        col.enabled = true;
        FindObjectOfType<Director>().ReturnGenericFishToPool(this);
    }
}

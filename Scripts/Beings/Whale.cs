using UnityEngine;
using System.Collections;
using System;

public class Whale : Enemy
{
    public void Awake()
    {
        BaseInit();
        invinsible = true;
        spriteFacesRightDefault = false;
    }

    public void Start()
    {
        tag = ENEMY;
        OnDeathStart = EnemyOnDeathStart;
        animator.transform.localScale = new Vector3(3.5f, 3f, 1);
        animator.transform.localPosition = Vector3.zero;
        GetComponent<BoxCollider2D>().size = new Vector2(1.5f, 1);
    }

    protected override void SetSpeedMod()
    {
        speedMod = 1.0f;
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
                if (StaticBuddy.IsOutOfBoundsWhale(transform))
                    RecieveDamageMaximum();
            }
        }
        else if (moveDir == 2)
        {
            transform.eulerAngles = Vector3.zero;
            while (isMoving)
            {
                rb.velocity = Vector2.left * speedMod;
                yield return null;
                if (StaticBuddy.IsOutOfBoundsWhale(transform))
                    RecieveDamageMaximum();
            }
        }
        else if (moveDir == 1)
        {
            transform.eulerAngles = new Vector3(0, 0, 90);
            while (isMoving)
            {
                rb.velocity = Vector2.up * -speedMod;
                yield return null;
                if (StaticBuddy.IsOutOfBoundsWhale(transform))
                    RecieveDamageMaximum();
            }
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, 270);
            while (isMoving)
            {
                rb.velocity = Vector2.up * speedMod;
                yield return null;
                if (StaticBuddy.IsOutOfBoundsWhale(transform))
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
        FindObjectOfType<Director>().ReturnWhaleToPool(this);
    }
}

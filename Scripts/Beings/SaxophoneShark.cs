using UnityEngine;
using System.Collections;
using System;

public class SaxophoneShark : Enemy
{

    Shrimp shrimp;
    Transform bombSpawnTransform;
    Transform pivot;

    float deployMin = 2f,
        deployMax = 7f;
    bool isDeployingBombs = false;

    public void Awake()
    {
        BaseInit();
        maxHealth = currentHealth = 15;
        bombSpawnTransform = transform.GetChild(1);
        pivot = transform.GetChild(2);
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
        if (!isDeployingBombs)
            StartCoroutine(ProcessBombDeployment());
        goingRight = !goingRight;
        BeginTurn();
        while (isMoving)
        {
            StaticBuddy.FaceOther(pivot, shrimp.transform.position);
            rb.velocity = pivot.right * speedMod;
            if (goingRight && rb.velocity.x<0)
            {
                goingRight = false;
                BeginTurn();
            }else if(goingRight==false && rb.velocity.x>0)
            {
                goingRight = true;
                BeginTurn();
            }
            
            yield return null;
        }
        isDeployingBombs = false;
        animate = false;
    }

    IEnumerator ProcessBombDeployment()
    {
        isDeployingBombs = true;
        while (isDeployingBombs)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(deployMin, deployMax));
            if(isDeployingBombs)
            {
                am.PlaySoundEffect(am.fxPathDrop);
                GameObject g = GetBomb();
                g.transform.position = bombSpawnTransform.position;
                g.SetActive(true);
            }
        }
    }

    protected override IEnumerator OnEndOfDeathProcess()
    {
        isMoving = false;
        animate = false;
        yield return null;
        animator.gameObject.SetActive(true);
        col.enabled = true;
        FindObjectOfType<Director>().ReturnSaxophoneSharkToPool(this);
    }

    protected override void SetSpeedMod()
    {
        speedMod = 0.7f;
    }
}

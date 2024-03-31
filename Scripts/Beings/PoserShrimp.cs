using UnityEngine;
using System.Collections;
using System;

public class PoserShrimp : Enemy
{

    Shrimp shrimp;
    GameObject guitar;
    GameObject projectileSpawnPoint;
    Transform pivot;

    static public int MODE_TRACK = 0, MODE_ORBIT = 1, MODE_DISTANCE = 2;
    int tacticMode = MODE_TRACK;
    bool orbitRight = false;
    float orbitRadius = 2.5f,
        orbitMemory,
        distanceClose = 0.4f,
        distanceFar = 3.8f,
        distanceMarginOfError = 1.5f;

    float deployMin = 2f,
        deployMax = 5f;
    bool isShooting = false;
    bool isThinking = false;
    float responseTime = 0.2f;
    bool turnDelay = false;
    float turnDelayTime = 5f;

    public void Awake()
    {
        BaseInit();
        maxHealth = currentHealth = 15;
        guitar = transform.GetChild(0).gameObject;
        projectileSpawnPoint = transform.GetChild(1).gameObject;
        pivot = transform.GetChild(2);
        orbitMemory = orbitRadius;
    }

    public void Start()
    {
        tag = ENEMY;
        OnDeathStart = EnemyOnDeathStart;
        speedMod = 0.5f;
    }

    public override IEnumerator Move()
    {
        shrimp = FindObjectOfType<Shrimp>();
        if (!isShooting)
            StartCoroutine(ProcessShooting());
        if (!isThinking)
            StartCoroutine(ProcessThoughts());
        while (isMoving)
        {
            if (tacticMode==MODE_TRACK)
            {
                StaticBuddy.FaceOther(pivot, shrimp.transform.position);
                rb.velocity = pivot.right * speedMod;
            }else if (tacticMode == MODE_ORBIT)
            {
                if(!orbitRight)
                    transform.RotateAround(shrimp.transform.position, Vector3.forward, speedMod/2);
                else
                    transform.RotateAround(shrimp.transform.position, Vector3.forward, -speedMod / 2);
                Vector2 desiredPosition = (transform.position - shrimp.transform.position).normalized * 
                                                                                    orbitRadius + shrimp.transform.position;
                StaticBuddy.FaceOther(pivot, desiredPosition);
                rb.velocity = pivot.right * speedMod;
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }else if(tacticMode == MODE_DISTANCE)
            {
                StaticBuddy.FaceOther(pivot, shrimp.transform.position);
                rb.velocity = pivot.right * -speedMod;
            }

            if (!turnDelay)
            {
                if (goingRight && rb.velocity.x < 0)
                {
                    goingRight = false;
                    BeginTurn();
                    StartCoroutine(ProcessTurnDelay());
                }
                else if (goingRight == false && rb.velocity.x > 0)
                {
                    goingRight = true;
                    BeginTurn();
                    StartCoroutine(ProcessTurnDelay());
                }
            }
            yield return null;
        }
        isShooting = false;
        isThinking = false;
        animate = false;
    }

    IEnumerator ProcessTurnDelay()
    {
        turnDelay = true;
        yield return new WaitForSeconds(turnDelayTime);
        turnDelay = false;
    }

    protected IEnumerator ProcessShooting()
    {
        isShooting = true;
        bool waitingForShot = false;
        while (isShooting)
        {
            if(waitingForShot)
                yield return new WaitForSeconds(.25f);
            else
                yield return new WaitForSeconds(UnityEngine.Random.Range(deployMin, deployMax));
            if (StaticBuddy.IsOutOfBounds(transform))
                waitingForShot = true;
            else {
                
                FireProjectileAtTarget(projectileSpawnPoint.transform.position, shrimp.transform.position,
                    true, 2.5f
                    );
                waitingForShot = false;
                
            }
        }
    }

    protected IEnumerator ProcessThoughts()
    {
        isThinking = true;
        while (isThinking)
        {
            yield return new WaitForSeconds(responseTime);
            ConsiderTactics();
        }
    }

    protected void ConsiderTactics()
    {
        float distance = Vector2.Distance(transform.position, shrimp.transform.position);
        Mathf.Abs(distance);
        if (distance > UnityEngine.Random.Range(-distanceMarginOfError + distanceFar, distanceMarginOfError + distanceFar))
            tacticMode = MODE_TRACK;
        else if (distance <= UnityEngine.Random.Range(-distanceMarginOfError + distanceClose, distanceMarginOfError + distanceClose))
            tacticMode = MODE_DISTANCE;
        else if (tacticMode != MODE_ORBIT || UnityEngine.Random.Range(0, 100) < 5)
        {
            tacticMode = MODE_ORBIT;
            orbitRight = UnityEngine.Random.Range(0, 2) == 0;
            orbitRadius = UnityEngine.Random.Range(-distanceMarginOfError + orbitMemory, distanceMarginOfError + orbitMemory);
        }
        
    }

    protected override IEnumerator OnEndOfDeathProcess()
    {
        isMoving = false;
        animate = false;
        yield return null;
        animator.gameObject.SetActive(true);
        col.enabled = true;
        FindObjectOfType<Director>().ReturnPoserShrimpToPool(this);
    }

    protected override void SetSpeedMod()
    {
        speedMod = 2f;
    }

}

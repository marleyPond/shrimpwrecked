using UnityEngine;
using System.Collections;

public class Blowfish : Enemy {

    float verDir = .05f;
    float inflateDelta = 0.02f;
    float inflateWaitMin = 3, inflateWaitMax = 7;
    bool switchingDir = false;
    bool isInflating = false;
    bool canInflate = false;

    float switchDirBaseMS = 0.25f;

    public void Awake()
    {
        BaseInit();
    }

    public void Start()
    {
        tag = ENEMY;
        OnDeathStart = EnemyOnDeathStart;
        transform.localScale = new Vector3(0.5f, 0.5f, 1);
        spriteFacesRightDefault = true;
    }

    public override IEnumerator Move()
    {
        StartCoroutine(ProcessSineTravel());
        StartCoroutine(ProcessInflation());
        transform.localScale = new Vector3(0.5f, 0.5f, 1);

        if (moveDir == 0)
        {

            while (isMoving)
            {
                animator.sprite_layers[0].sr.flipX = false;
                rb.velocity = Vector2.left * -speedMod;
                rb.velocity = new Vector2(rb.velocity.x, 2f * verDir);
                yield return null;
                if (StaticBuddy.IsOutOfBoundsExtendedRightOnly(transform))
                    RecieveDamageMaximum();
            }
        }
        else if (moveDir == 2)
        {

            while (isMoving)
            {
                animator.sprite_layers[0].sr.flipX = true;
                rb.velocity = Vector2.left * speedMod;
                rb.velocity = new Vector2(rb.velocity.x, 2f * verDir);
                yield return null;
                if (StaticBuddy.IsOutOfBoundsExtendedLeftOnly(transform))
                    RecieveDamageMaximum();
            }
        }
        else if (moveDir == 1)
        {
            while (isMoving)
            {
                rb.velocity = Vector2.left * speedMod;
                rb.velocity = new Vector2(2f * verDir, rb.velocity.x);
                yield return null;
                if (StaticBuddy.IsOutOfBoundsExtendedBottomOnly(transform))
                    RecieveDamageMaximum();
            }
        }
        else
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
            ms = switchDirBaseMS * 2;
        }
    }

    IEnumerator ProcessInflation()
    {
        canInflate = true;
        float scaleTemp;
        while (canInflate)
        {
            isInflating = false;
            yield return new WaitForSeconds(Random.Range(inflateWaitMin, inflateWaitMax));
            isInflating = true;
            while (isInflating)
            {
                scaleTemp = Mathf.Min(transform.localScale.x + inflateDelta, 1);
                transform.localScale = new Vector3(scaleTemp, scaleTemp, 1);
                yield return new WaitForSeconds(0.025f);
                if(!canInflate)
                    break;
                if (scaleTemp == 1)
                {
                    isInflating = false;
                    ShootSpikes();
                    break;
                }
            }
            transform.localScale = new Vector3(0.5f, 0.5f, 1);
        }
        transform.localScale = new Vector3(0.5f, 0.5f, 1);
        isInflating = false;
    }

    void ShootSpikes()
    {
        FireSpikeAtTargetEightWays(transform.position, true, 6f);
    }

    protected override IEnumerator OnEndOfDeathProcess()
    {
        isMoving = false;
        animate = false;
        isInflating = false;
        canInflate = false;
        yield return null;
        transform.localScale = new Vector3(.5f, .5f, 1);
        animator.gameObject.SetActive(true);
        col.enabled = true;
        FindObjectOfType<Director>().ReturnBlowfishToPool(this);
    }

    protected override void SetSpeedMod()
    {
        speedMod = 0.55f;
    }

}

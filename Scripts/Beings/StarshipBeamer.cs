using UnityEngine;
using System.Collections;
using System;

public class StarshipBeamer : Enemy
{
    ClockworkBeast pilot;
    GameObject beam;
    Collider2D beamCol;

    Shrimp shrimp;

    bool getIntoVerticalPosition = false,
        bobbleAround = false,
        shootBeam = false;

    public void Awake()
    {
        BaseInit();
        maxHealth = currentHealth = 1;
        pilot = transform.GetChild(3).GetComponent<ClockworkBeast>();
        pilot.owner = this;
        beam = transform.GetChild(4).GetChild(0).gameObject;
        beamCol = beam.GetComponent<Collider2D>();
    }

    public void Start()
    {
        tag = ENEMY;
        OnDeathStart = EnemyOnDeathStart;
        OnRecieveDamage = MasterExploder;
        loseOnRecieveDamageMethodOnActivation = false;
    }

    bool MasterExploder()
    {
        GameObject g = Being.GetExplosion();
        g.transform.position = transform.position;
        g.SetActive(true);
        return true;
    }

    public override IEnumerator Move()
    {
        shrimp = FindObjectOfType<Shrimp>();
        pilot.gameObject.SetActive(true);
        pilot.BeginMoving(0);
        pilot.SetHealth(1);
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(GetIntoVerticalPosition());
        StartCoroutine(ShootBeam());
        while (isMoving)
        {
            if (StaticBuddy.IsOutOfBoundsExtendedLeftOnly(transform) ||
                StaticBuddy.IsOutOfBoundsExtendedRightOnly(transform))
                RecieveDamageMaximum();
            yield return null;
        }
        beam.transform.localScale = new Vector3(
                           0,
                           beam.transform.localScale.y,
                           beam.transform.localScale.z
                       );
        getIntoVerticalPosition = false;
        shootBeam = false;
        bobbleAround = false;
        animate = false;
    }

    IEnumerator GetIntoVerticalPosition()
    {
        if (!getIntoVerticalPosition)
        {
            getIntoVerticalPosition = true;
            while (getIntoVerticalPosition)
            {
                Vector3 v = Camera.main.ViewportToWorldPoint(
                    new Vector3(0, UnityEngine.Random.Range(.8f, .65f), 0)
                    );
                rb.velocity = Vector3.down * speedMod;
                if (transform.position.y > v.y)
                {
                    yield return null;
                }
                else
                    break;
            }
            StartCoroutine(BobbleAround());
            getIntoVerticalPosition = false;
        }
            
    }

    IEnumerator BobbleAround()
    {
        if (!bobbleAround)
        {
            bobbleAround = true;
            while (bobbleAround)
            {
                if (UnityEngine.Random.Range(0, 2) == 0)
                    rb.velocity = transform.right * speedMod * 2;
                else
                    rb.velocity = transform.right * -speedMod * 2;
                yield return new WaitForSeconds(0.25f);
            }
        }
    }

    IEnumerator ShootBeam()
    {
        if (!shootBeam)
        {
            shootBeam = true;
            AudioSource src = null;
            StartCoroutine(StaticBuddy.ChangeColorWhileEnabled(beam.GetComponent<SpriteRenderer>()));
            while (shootBeam)
            {
                StartCoroutine(StaticBuddy.ChangeColorWhileEnabled(beam.GetComponent<SpriteRenderer>()));
                yield return new WaitForSeconds(UnityEngine.Random.Range(2, 3));
                if (!shootBeam)
                    break;
                src = am.PlaySoundEffect(am.fxPathBeam, true);
                while (beam.transform.localScale.x < 4.5f)
                {
                    beam.transform.localScale = new Vector3(
                            Mathf.Min(beam.transform.localScale.x + 0.2f, 4.5f),
                            beam.transform.localScale.y,
                            beam.transform.localScale.z
                        );
                    yield return new WaitForSeconds(0.1f);
                    if (!shootBeam)
                        break;
                }
                yield return new WaitForSeconds(2f);
                if (!shootBeam)
                    break;
                while (beam.transform.localScale.x > 0)
                {
                    beam.transform.localScale = new Vector3(
                            Mathf.Max(beam.transform.localScale.x - 0.2f, 0),
                            beam.transform.localScale.y,
                            beam.transform.localScale.z
                        );
                    yield return new WaitForSeconds(0.1f);
                    if (!shootBeam)
                        break;
                }
                if (src != null)
                {
                    src.Stop();
                    src = null;
                }
            }
            if (src != null)
            {
                src.Stop();
                src = null;
            }
        }
        beam.transform.localScale = new Vector3
        (
            0f, beam.transform.localScale.y, beam.transform.localScale.z
        );
    }

    protected override IEnumerator OnEndOfDeathProcess()
    {
        isMoving = false;
        animate = false;
        yield return null;
        pilot.transform.parent = transform;
        pilot.transform.localPosition = pilot.GetComponent<ClockworkBeast>().originalLoc;
        r.color = new Color(r.color.r, r.color.g, r.color.b, 1);
        FindObjectOfType<Director>().ReturnBeamerToPool(this);
    }

    protected override void SetSpeedMod()
    {
        speedMod = 0.5f;
    }
}

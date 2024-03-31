using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StarshipDiscoPepper : Enemy
{
    ClockworkBeast pilot;
    List<Transform> guns;
    List<bool> gunState;

    Shrimp shrimp;

    bool getIntoVerticalPosition = false,
        bobbleAround = false,
        shootGuns = false;

    public void Awake()
    {
        BaseInit();
        maxHealth = currentHealth = 1;
        pilot = transform.GetChild(3).GetComponent<ClockworkBeast>();
        pilot.owner = this;
        guns = new List<Transform>();
        gunState = new List<bool>();
        guns.Add(transform.GetChild(5));
        guns.Add(transform.GetChild(6));
        gunState.Add(false);
        gunState.Add(false);
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
        StartCoroutine(BobbleAround());
        while (isMoving)
        {
            if (StaticBuddy.IsOutOfBoundsExtendedLeftOnly(transform) ||
                StaticBuddy.IsOutOfBoundsExtendedRightOnly(transform))
                RecieveDamageMaximum();
            yield return null;
        }
        shootGuns = false;
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
            ActivateGuns();
            getIntoVerticalPosition = false;
        }

    }

    void ActivateGuns()
    {
        shootGuns = true;
        for(int i = 0; i < guns.Count; i++)
        {
            StartCoroutine(OperateGun(i));
        }
    }

    IEnumerator OperateGun(int index)
    {
        while (shootGuns)
        {
            if (!gunState[index])
                StartCoroutine(WarmUpAndFire(index));
            StaticBuddy.FaceOther(guns[index], shrimp.transform.position);
            yield return null;
        }
        gunState[index] = false;
    }

    IEnumerator WarmUpAndFire(int index)
    {
        gunState[index] = true;
        while (shootGuns && gunState[index])
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(2, 3.5f));
            if(shootGuns && gunState[index])
                FireDiscoPelletAtTarget(guns[index].position, shrimp.transform.position, true, 3f);
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

    protected override IEnumerator OnEndOfDeathProcess()
    {
        isMoving = false;
        animate = false;
        yield return null;
        pilot.transform.parent = transform;
        pilot.transform.localPosition = pilot.GetComponent<ClockworkBeast>().originalLoc;
        r.color = new Color(r.color.r, r.color.g, r.color.b, 1);
        FindObjectOfType<Director>().ReturnDiscoPepperToPool(this);
    }

    protected override void SetSpeedMod()
    {
        speedMod = 1f;
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mothership : Enemy
{

    List<Transform> guns;
    List<bool> gunState;
    List<GameObject> beams;
    List<ClockworkBeast> pilots;

    Shrimp shrimp;

    int pilotHealth = 5;

    bool getIntoVerticalPosition = false,
        bobbleAround = false,
        shootGuns = false,
        shootBeam = false,
        deployShips;

    public void Awake()
    {
        BaseInit();
        maxHealth = currentHealth = 6;

        guns = new List<Transform>();
        gunState = new List<bool>();
        beams = new List<GameObject>();
        pilots = new List<ClockworkBeast>();

        for (int i = 2, index = 0; i < 8; i++, index++)
        {
            pilots.Add(transform.GetChild(i).GetComponent<ClockworkBeast>());
            pilots[index].owner = this;
            pilots[index].SetHealth(pilotHealth);
        }
        for (int i = 8; i < 12; i++)
        {
            guns.Add(transform.GetChild(i));
            gunState.Add(false);
        }
        for(int i = 12; i < 14; i++)
        {
            beams.Add(transform.GetChild(i).GetChild(0).gameObject);
        }

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
        if (currentHealth == 1)
        {
            GameObject g = Being.GetExplosion();
            g.transform.position = transform.position;
            g.SetActive(true);
        }
        return true;
    }

    public override IEnumerator Move()
    {
        shrimp = FindObjectOfType<Shrimp>();
        for(int i = 0; i < pilots.Count; i++)
        {
            pilots[i].gameObject.SetActive(true);
            pilots[i].BeginMoving(0);
            pilots[i].SetHealth(pilotHealth);
        }
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(GetIntoVerticalPosition());
        StartCoroutine(BobbleAround());
        while (isMoving)
        {
            yield return null;
        }
        shootGuns = false;
        bobbleAround = false;
        shootBeam = false;
        deployShips = false;
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
                    new Vector3(0, .75f, 0)
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
            ActivateBeams();
            StartCoroutine(ActivateShipDeployer());
            getIntoVerticalPosition = false;
        }

    }

    IEnumerator ActivateShipDeployer()
    {
        deployShips = true;
        while (deployShips)
        {
            yield return new WaitForSeconds(Random.Range(9, 15));
            StarshipDiscoPepper s = director.GetDiscoPepper();
            s.transform.position = transform.position;
            s.invinsible = true;
            float dist = 1f; 
            for (float i = 0.1f; i < 1.1f; i = i + 0.1f)
            {
                s.transform.position = Vector3.Lerp(
                        transform.position,
                        new Vector3(transform.position.x + dist, transform.position.y, transform.position.z),
                        i
                    );
                yield return new WaitForSeconds(0.05f);
            }
            s.invinsible = false;
            s.gameObject.SetActive(true);
            s.BeginMoving(3);
        }
    }

    void ActivateGuns()
    {
        shootGuns = true;
        for (int i = 0; i < guns.Count; i++)
        {
            StartCoroutine(OperateGun(i));
        }
    }

    void ActivateBeams()
    {
        shootBeam = true;
        for (int i = 0; i < beams.Count; i++)
        {
            StartCoroutine(OperateBeam(i));
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
            if (shootGuns && gunState[index])
                FireDiscoPelletAtTarget(guns[index].position, shrimp.transform.position, true, 3f);
        }
    }

    IEnumerator OperateBeam(int index)
    {
        while (shootBeam)
        {
            StartCoroutine(StaticBuddy.ChangeColorWhileEnabled(
                beams[index].GetComponent<SpriteRenderer>(),
                beams[index].transform.GetChild(0).GetComponent<Light>()
                ));
            AudioSource src = null;
            yield return new WaitForSeconds(Random.Range(4, 7));
            if (!shootBeam)
                break;
            src = am.PlaySoundEffect(am.fxPathBeam, true);
            while (beams[index].transform.localScale.x < 4.5f)
            {
                beams[index].transform.localScale = new Vector3(
                        Mathf.Min(beams[index].transform.localScale.x + 0.2f, 4.5f),
                        beams[index].transform.localScale.y,
                        beams[index].transform.localScale.z
                    );
                yield return new WaitForSeconds(0.1f);
                if (!shootBeam)
                    break;
            }
            yield return new WaitForSeconds(2f);
            if (!shootBeam)
                break;
            while (beams[index].transform.localScale.x > 0)
            {
                beams[index].transform.localScale = new Vector3(
                        Mathf.Max(beams[index].transform.localScale.x - 0.2f, 0),
                        beams[index].transform.localScale.y,
                        beams[index].transform.localScale.z
                    );
                yield return new WaitForSeconds(0.1f);
                if (!shootBeam)
                    break;
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

        beams[index].transform.localScale = new Vector3
        (
            0f, beams[index].transform.localScale.y, beams[index].transform.localScale.z
        );
    }

    IEnumerator BobbleAround()
    {
        if (!bobbleAround)
        {
            bobbleAround = true;
            float leftBound = Camera.main.ViewportToWorldPoint(new Vector3(0.15f,0 , 0)).x,
                rightBound = Camera.main.ViewportToWorldPoint(new Vector3(0.85f, 0, 0)).x;
            int mod = 1;
            while (bobbleAround)
            {
                float x = transform.position.x;
                if (x <= leftBound)
                {
                    rb.velocity = transform.right * speedMod;
                }
                else if (x >= rightBound)
                {
                    rb.velocity = transform.right * -speedMod;
                }
                else
                {
                    if (Random.Range(0, 2) == 0)
                        mod *= -1;
                    rb.velocity = transform.right * speedMod * mod;
                }
                yield return new WaitForSeconds(0.25f);
            }
        }
    }

    protected override IEnumerator OnEndOfDeathProcess()
    {
        isMoving = false;
        animate = false;
        shootBeam = false;
        shrimp.invinsible = true;
        yield return null;
        for (int i = 0; i < pilots.Count; i++)
        {
            pilots[i].transform.parent = transform;
            pilots[i].transform.localPosition = pilots[i].GetComponent<ClockworkBeast>().originalLoc;
        }
        r.color = new Color(r.color.r, r.color.g, r.color.b, 1);
        FindObjectOfType<Director>().ReturnMothershipToPool(this);
    }

    protected override void SetSpeedMod()
    {
        speedMod = 1f;
    }

}
